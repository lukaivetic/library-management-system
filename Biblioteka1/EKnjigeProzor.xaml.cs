using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibrarySystem
{
     public partial class EKnigeProzor : Window
     {
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
          private string originalQuery = "SELECT Naziv, Autor, DatumIzdavanja, StatusDostupnosti FROM EKnjige";

          public EKnigeProzor()
          {
               InitializeComponent();
               PrikaziEKnjige();
          }

          public void PrikaziEKnjige(string filter = "")
          {
               try
               {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                         connection.Open();
                         string query = originalQuery;

                         if (!string.IsNullOrEmpty(filter))
                         {
                              query += $" WHERE Naziv LIKE '%{filter}%' OR Autor LIKE '%{filter}%'";
                         }

                         SqlCommand command = new SqlCommand(query, connection);
                         SqlDataReader reader = command.ExecuteReader();

                  
                         EKnjigeListView.Items.Clear();

                         while (reader.Read())
                         {
                              EKnjiga eKnjiga = new EKnjiga
                              {
                                   Naziv = reader.GetString(0),
                                   Autor = reader.GetString(1),
                                   DatumIzdavanja = reader.GetDateTime(2),
                                   StatusDostupnosti = reader.GetString(3),
                              };
                              EKnjigeListView.Items.Add(eKnjiga);
                         }

                         reader.Close();
                    }
               }
               catch (Exception ex)
               {
                    MessageBox.Show("Greška prilikom prikazivanja e-knjiga: " + ex.Message);
               }
          }

          private void PretraziButton_Click(object sender, RoutedEventArgs e)
          {

               string filter = PretraziTextBox.Text;

               PrikaziEKnjige(filter);
          }

          private void DodajButton_Click(object sender, RoutedEventArgs e)
          {
               DodajEKnjigu dodajEKnjiguProzor = new DodajEKnjigu(this);
               dodajEKnjiguProzor.Show();
          }

          private void ObrisiButton_Click(object sender, RoutedEventArgs e)
          {

               if (EKnjigeListView.SelectedItem == null)
               {
                    MessageBox.Show("Molimo selektujte knjigu koju želite da obrišete.");
                    return;
               }

               EKnjiga selectedEKnjiga = (EKnjiga)EKnjigeListView.SelectedItem;
               if (selectedEKnjiga.StatusDostupnosti == "Iznajmljena")
               {
                    MessageBox.Show("Ne možete obrisati iznajmljenu knjigu.");
                    return;
               }


               MessageBoxResult result = MessageBox.Show("Da li ste sigurni da želite da obrišete izabranu knjigu?",
                                                         "Potvrda brisanja",
                                                         MessageBoxButton.YesNo,
                                                         MessageBoxImage.Question);
               if (result == MessageBoxResult.Yes)
               {

                    try
                    {
                         using (SqlConnection connection = new SqlConnection(connectionString))
                         {
                              connection.Open();
                              string query = $"DELETE FROM EKnjige WHERE Naziv = '{selectedEKnjiga.Naziv}'";
                              SqlCommand command = new SqlCommand(query, connection);
                              command.ExecuteNonQuery();
                         }


                         PrikaziEKnjige();
                         MessageBox.Show("Knjiga je uspešno obrisana.");
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show("Greška prilikom brisanja knjige: " + ex.Message);
                    }
               }
          }

          private void IzmeniButton_Click(object sender, RoutedEventArgs e)
          {

               if (EKnjigeListView.SelectedItem == null)
               {
                    MessageBox.Show("Molimo selektujte knjigu koju želite da izmenite.");
                    return;
               }

               string nazivKnjigeZaIzmenu = ((EKnjiga)EKnjigeListView.SelectedItem).Naziv;


               IzmeniEKnjigu izmeniEKnjiguProzor = new IzmeniEKnjigu(nazivKnjigeZaIzmenu);
               izmeniEKnjiguProzor.ShowDialog(); 


               PrikaziEKnjige();
          }

          private void IzlazButton_Click(object sender, RoutedEventArgs e)
          {

               MainWindow mainWindow = new MainWindow();
               this.Hide();
               mainWindow.Show();
               this.Close();
          }
     }

     public class EKnjiga
     {
          public string Naziv { get; set; }
          public string Autor { get; set; }
          public DateTime DatumIzdavanja { get; set; }
          public string StatusDostupnosti { get; set; }
     }
}
