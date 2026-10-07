using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibrarySystem
{
     public partial class DodajBibliotekaraProzor : Window
     {
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
          private Bibliotekari bibliotekariWindow;

          public DodajBibliotekaraProzor(Bibliotekari bibliotekariWindow)
          {
               InitializeComponent();
               this.bibliotekariWindow = bibliotekariWindow;
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

               string query = "INSERT INTO Bibliotekari (Ime, Prezime, Email, Telefon, Adresa) VALUES (@Ime, @Prezime, @Email, @Telefon, @Adresa)";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Ime", ImeTextBox.Text);
                    command.Parameters.AddWithValue("@Prezime", PrezimeTextBox.Text);
                    command.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                    command.Parameters.AddWithValue("@Telefon", TelefonTextBox.Text);
                    command.Parameters.AddWithValue("@Adresa", AdresaTextBox.Text);

                    connection.Open();
                    command.ExecuteNonQuery();
               }

               MessageBox.Show("Uspešno ste uneli bibliotekara!");

               bibliotekariWindow.PrikaziBibliotekare();
               this.Close();
          }
     }
}
