using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibrarySystem
{
     public partial class IzmeniBibliotekaraProzor : Window
     {
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
          private Bibliotekar selectedBibliotekar;
          private Bibliotekari parentWindow;

          public IzmeniBibliotekaraProzor(Bibliotekar bibliotekar, Bibliotekari parent)
          {
               InitializeComponent();
               selectedBibliotekar = bibliotekar;
               parentWindow = parent;


               ImeTextBox.Text = bibliotekar.Ime;
               PrezimeTextBox.Text = bibliotekar.Prezime;
               EmailTextBox.Text = bibliotekar.Email;
               TelefonTextBox.Text = bibliotekar.Telefon;
               AdresaTextBox.Text = bibliotekar.Adresa;
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {

               if (string.IsNullOrWhiteSpace(ImeTextBox.Text) || string.IsNullOrWhiteSpace(PrezimeTextBox.Text) ||
                   string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(TelefonTextBox.Text) ||
                   string.IsNullOrWhiteSpace(AdresaTextBox.Text))
               {
                    MessageBox.Show("Sva polja moraju biti popunjena!");
                    return;
               }


               try
               {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                         connection.Open();
                         string query = @"UPDATE Bibliotekari 
                                     SET Ime = @Ime, Prezime = @Prezime, Email = @Email, Telefon = @Telefon, Adresa = @Adresa 
                                     WHERE Ime = @OldIme AND Prezime = @OldPrezime";

                         SqlCommand command = new SqlCommand(query, connection);
                         command.Parameters.AddWithValue("@Ime", ImeTextBox.Text);
                         command.Parameters.AddWithValue("@Prezime", PrezimeTextBox.Text);
                         command.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                         command.Parameters.AddWithValue("@Telefon", TelefonTextBox.Text);
                         command.Parameters.AddWithValue("@Adresa", AdresaTextBox.Text);
                         command.Parameters.AddWithValue("@OldIme", selectedBibliotekar.Ime);
                         command.Parameters.AddWithValue("@OldPrezime", selectedBibliotekar.Prezime);

                         int rowsAffected = command.ExecuteNonQuery();

                         if (rowsAffected > 0)
                         {
                              MessageBox.Show("Podaci o bibliotekaru su uspešno izmenjeni.");

                              parentWindow.PrikaziBibliotekare();
                         }
                         else
                         {
                              MessageBox.Show("Greška prilikom izmene podataka o bibliotekaru.");
                         }
                    }
               }
               catch (Exception ex)
               {
                    MessageBox.Show("Greška prilikom izmene podataka: " + ex.Message);
               }

               this.Close();
          }

          private void IzlazButton_Click(object sender, RoutedEventArgs e)
          {
               this.Close();
          }
     }
}
