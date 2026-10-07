using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibrarySystem
{
     public partial class IzmeniEKnjigu : Window
     {
          private string nazivKnjigeZaIzmenu;
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";

          public IzmeniEKnjigu(string nazivKnjige)
          {
               InitializeComponent();
               nazivKnjigeZaIzmenu = nazivKnjige;
               UcitajInformacijeOKnjizi();
          }

          private void UcitajInformacijeOKnjizi()
          {
               try
               {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                         connection.Open();
                         string query = $"SELECT Naziv, Autor, DatumIzdavanja FROM EKnjige WHERE Naziv = '{nazivKnjigeZaIzmenu}'";
                         SqlCommand command = new SqlCommand(query, connection);
                         SqlDataReader reader = command.ExecuteReader();

                         if (reader.Read())
                         {
                              NazivTextBox.Text = reader.GetString(0);
                              AutorTextBox.Text = reader.GetString(1);
                              DatumIzdavanjaDatePicker.SelectedDate = reader.GetDateTime(2);
                         }

                         reader.Close();
                    }
               }
               catch (Exception ex)
               {
                    MessageBox.Show("Greška prilikom učitavanja informacija o knjizi: " + ex.Message);
               }
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {
               string noviNaziv = NazivTextBox.Text;
               string noviAutor = AutorTextBox.Text;
               DateTime? noviDatumIzdavanja = DatumIzdavanjaDatePicker.SelectedDate;

               if (noviNaziv == "" || noviAutor == "" || !noviDatumIzdavanja.HasValue)
               {
                    MessageBox.Show("Molimo popunite sve informacije o knjizi.");
                    return;
               }

               try
               {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                         connection.Open();
                         string query = $"UPDATE EKnjige SET Naziv = @Naziv, Autor = @Autor, DatumIzdavanja = @DatumIzdavanja WHERE Naziv = @StariNaziv";
                         SqlCommand command = new SqlCommand(query, connection);
                         command.Parameters.AddWithValue("@Naziv", noviNaziv);
                         command.Parameters.AddWithValue("@Autor", noviAutor);
                         command.Parameters.AddWithValue("@DatumIzdavanja", noviDatumIzdavanja.Value);
                         command.Parameters.AddWithValue("@StariNaziv", nazivKnjigeZaIzmenu);
                         command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Knjiga je uspešno izmenjena.");
                    this.Close();
               }
               catch (Exception ex)
               {
                    MessageBox.Show("Greška prilikom izmene knjige: " + ex.Message);
               }
          }
     }
}
