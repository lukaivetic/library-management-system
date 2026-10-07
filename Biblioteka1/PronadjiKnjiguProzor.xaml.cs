using Biblioteka1;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace LibrarySystem
{
     public partial class FindBookWindow : Window
     {
          public ObservableCollection<Knjiga> Knjige { get; set; }

          public FindBookWindow()
          {
               InitializeComponent();
               Knjige = new ObservableCollection<Knjiga>();
               GetKnjigeFromDatabase();
               BookListView.ItemsSource = Knjige;
          }

          public void GetKnjigeFromDatabase()
          {
               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "SELECT Naziv, Autor, DatumIzdavanja, StatusDostupnosti, VrstaKorica FROM Knjige";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                         Knjige.Add(new Knjiga
                         {
                              Naziv = reader["Naziv"].ToString(),
                              Autor = reader["Autor"].ToString(),
                              DatumIzdavanja = reader["DatumIzdavanja"] != DBNull.Value ? Convert.ToDateTime(reader["DatumIzdavanja"]) : DateTime.MinValue,
                              StatusDostupnosti = reader["StatusDostupnosti"].ToString(),
                              VrstaKorica = reader["VrstaKorica"].ToString()
                         });
                    }
               }
          }

          private void SearchButton_Click(object sender, RoutedEventArgs e)
          {
               string searchTerm = SearchTextBox.Text.ToLower();

               if (string.IsNullOrEmpty(searchTerm))
               {
                    BookListView.ItemsSource = Knjige;
               }
               else
               {
                    var filteredKnjige = Knjige.Where(knjiga =>
                        knjiga.Naziv.ToLower().Contains(searchTerm) ||
                        knjiga.Autor.ToLower().Contains(searchTerm) ||
                        knjiga.StatusDostupnosti.ToLower().Contains(searchTerm) ||
                        knjiga.VrstaKorica.ToLower().Contains(searchTerm)
                    ).ToList();

                    BookListView.ItemsSource = filteredKnjige;
               }
          }

          private void ExitButton_Click(object sender, RoutedEventArgs e)
          {
               MainWindow mainWindow = new MainWindow();
               mainWindow.Show();
               Close(); 
          }

          private void AddBookButton_Click(object sender, RoutedEventArgs e)
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

          private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
          {
               if (BookListView.SelectedItem != null)
               {
                    Knjiga selectedBook = (Knjiga)BookListView.SelectedItem;

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

          private void EditBookButton_Click(object sender, RoutedEventArgs e)
          {
               if (BookListView.SelectedItem != null)
               {
                    Knjiga selectedBook = (Knjiga)BookListView.SelectedItem;

                    IzmeniKnjiguProzor2 izmeniKnjiguProzor = new IzmeniKnjiguProzor2(selectedBook, this);
                    izmeniKnjiguProzor.ShowDialog();

                    if (izmeniKnjiguProzor.DialogResult == true)
                    {
                         int index = Knjige.IndexOf(selectedBook);
                         Knjige[index] = izmeniKnjiguProzor.NovaKnjiga;
                    }
               }
               else
               {
                    MessageBox.Show("Niste izabrali knjigu za izmenu.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
               }
          }
     }

     public class Knjiga
     {
          public string Naziv { get; set; }
          public string Autor { get; set; }
          public DateTime DatumIzdavanja { get; set; }
          public string StatusDostupnosti { get; set; }
          public string VrstaKorica { get; set; }
     }
}
