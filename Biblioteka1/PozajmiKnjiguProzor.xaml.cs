using Biblioteka1;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace LibrarySystem
{
     public partial class PozajmiKnjiguProzor : Window
     {
          public ObservableCollection<Knjiga> Knjige { get; set; }
          public ObservableCollection<EKnjiga> EKnjige { get; set; }

          public PozajmiKnjiguProzor(ObservableCollection<Knjiga> knjige)
          {
               InitializeComponent();
               Knjige = knjige;
               BookDetailsListView.ItemsSource = Knjige;
               if (Knjige.Count == 0)
               {
                    GetKnjigeFromDatabase();
               }
          }

          private void GetKnjigeFromDatabase()
          {
               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string knjigeQuery = "SELECT Naziv, Autor, DatumIzdavanja, StatusDostupnosti, VrstaKorica FROM Knjige";

               string eKnjigeQuery = "SELECT Naziv, Autor, DatumIzdavanja, StatusDostupnosti, NULL AS VrstaKorica FROM EKnjige";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(knjigeQuery + " UNION ALL " + eKnjigeQuery, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                         string vrstaKorica = reader["VrstaKorica"] != DBNull.Value ? reader["VrstaKorica"].ToString() : "";
                         Knjige.Add(new Knjiga
                         {
                              Naziv = reader["Naziv"].ToString(),
                              Autor = reader["Autor"].ToString(),
                              DatumIzdavanja = Convert.ToDateTime(reader["DatumIzdavanja"]),
                              StatusDostupnosti = reader["StatusDostupnosti"].ToString(),
                              VrstaKorica = vrstaKorica
                         });
                    }
               }
          }

          private void SearchButton_Click(object sender, RoutedEventArgs e)
          {
               string searchTerm = BookNameTextBox.Text.ToLower();

               ObservableCollection<Knjiga> filteredKnjige = new ObservableCollection<Knjiga>(
                   Knjige.Where(knjiga =>
                       knjiga.Naziv.ToLower().Contains(searchTerm) ||
                       knjiga.Autor.ToLower().Contains(searchTerm)
                   ));

               BookDetailsListView.ItemsSource = filteredKnjige;
          }

          private void RejectButton_Click(object sender, RoutedEventArgs e)
          {
               MainWindow mainWindow = new MainWindow();
               mainWindow.Show();
               Close();
          }

          private void PozajmiKnjiguButton_Click(object sender, RoutedEventArgs e)
          {
               PozajmljivanjeKnjige pozajmljivanjeKnjige = new PozajmljivanjeKnjige(Knjige);
               pozajmljivanjeKnjige.Show();
          }

          private void VratiKnjiguButton_Click(object sender, RoutedEventArgs e)
          {
               VratiKnjiguProzor vratiKnjiguProzor = new VratiKnjiguProzor();
               vratiKnjiguProzor.Show();
          }

          private void IzmeniButton_Click(object sender, RoutedEventArgs e)
          {
               if (BookDetailsListView.SelectedItem != null && BookDetailsListView.SelectedItem is Knjiga selectedKnjiga)
               {
                    IzmeniKnjiguProzor izmeniKnjiguProzor = new IzmeniKnjiguProzor(selectedKnjiga, Knjige, EKnjige);
                    izmeniKnjiguProzor.Show();
               }
               else
               {
                    MessageBox.Show("Morate prvo izabrati knjigu koju želite da izmenite.");
               }
          }

          private void DodajButton_Click(object sender, RoutedEventArgs e)
          {
               DodajKnjiguProzor dodajKnjiguProzor = new DodajKnjiguProzor();
               if (dodajKnjiguProzor.ShowDialog() == true)
               {
                    Knjiga novaKnjiga = new Knjiga
                    {
                         Naziv = dodajKnjiguProzor.NovaKnjiga.Naziv,
                         Autor = dodajKnjiguProzor.NovaKnjiga.Autor,
                         DatumIzdavanja = dodajKnjiguProzor.NovaKnjiga.DatumIzdavanja,
                         StatusDostupnosti = dodajKnjiguProzor.NovaKnjiga.StatusDostupnosti,
                         VrstaKorica = dodajKnjiguProzor.NovaKnjiga.VrstaKorica
                    };

                    Knjige.Add(novaKnjiga);
                    AddBookToDatabase(novaKnjiga);
               }
          }

          private void ObrisiButton_Click(object sender, RoutedEventArgs e)
          {
               if (BookDetailsListView.SelectedItem != null)
               {
                    Knjiga selectedBook = (Knjiga)BookDetailsListView.SelectedItem;

                    if (selectedBook.StatusDostupnosti == "Dostupna")
                    {
                         Knjige.Remove(selectedBook);
                         DeleteBookFromDatabase(selectedBook);
                    }
                    else
                    {
                         MessageBox.Show("Knjiga nije dostupna za brisanje jer je iznajmljena.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
               }
               else
               {
                    MessageBox.Show("Niste izabrali knjigu za brisanje.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
               }
          }

          private void AddBookToDatabase(Knjiga novaKnjiga)
          {
               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "INSERT INTO Knjige (Naziv, Autor, DatumIzdavanja, StatusDostupnosti, VrstaKorica) VALUES (@Naziv, @Autor, @DatumIzdavanja, @StatusDostupnosti, @VrstaKorica)";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Naziv", novaKnjiga.Naziv);
                    command.Parameters.AddWithValue("@Autor", novaKnjiga.Autor);
                    command.Parameters.AddWithValue("@DatumIzdavanja", novaKnjiga.DatumIzdavanja);
                    command.Parameters.AddWithValue("@StatusDostupnosti", novaKnjiga.StatusDostupnosti);
                    command.Parameters.AddWithValue("@VrstaKorica", novaKnjiga.VrstaKorica);

                    connection.Open();
                    command.ExecuteNonQuery();
               }
          }

          private void DeleteBookFromDatabase(Knjiga selectedBook)
          {
               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "DELETE FROM Knjige WHERE Naziv = @Naziv AND Autor = @Autor";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Naziv", selectedBook.Naziv);
                    command.Parameters.AddWithValue("@Autor", selectedBook.Autor);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                         MessageBox.Show("Uspesno ste obrisali knjigu iz baze podataka!", "Brisanje knjige", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                         MessageBox.Show("Brisanje knjige nije uspelo!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
               }
          }
     }
}
