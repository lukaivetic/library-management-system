using System;
using System.Windows;
using System.Data.SqlClient;

namespace LibrarySystem
{
     public partial class IzmeniClanarinuProzor : Window
     {
          public event EventHandler ClanarinaIzmenjena; 

          private Osoba izabraniClan;

          public IzmeniClanarinuProzor(Osoba selectedOsoba)
          {
               InitializeComponent();
               izabraniClan = selectedOsoba;
               ImeTextBox.Text = izabraniClan.Ime;
               PrezimeTextBox.Text = izabraniClan.Prezime;
               DatumUplateDatePicker.SelectedDate = izabraniClan.DatumUplate;
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {
               
               if (string.IsNullOrEmpty(ImeTextBox.Text) || string.IsNullOrEmpty(PrezimeTextBox.Text) || DatumUplateDatePicker.SelectedDate == null)
               {
                    MessageBox.Show("Molimo unesite sve podatke.");
                    return;
               }

               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "UPDATE Clanovi SET Ime = @Ime, Prezime = @Prezime, DatumUplate = @DatumUplate WHERE Ime = @OldIme AND Prezime = @OldPrezime";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Ime", ImeTextBox.Text);
                    command.Parameters.AddWithValue("@Prezime", PrezimeTextBox.Text);
                    command.Parameters.AddWithValue("@DatumUplate", DatumUplateDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@OldIme", izabraniClan.Ime);
                    command.Parameters.AddWithValue("@OldPrezime", izabraniClan.Prezime);

                    try
                    {
                         connection.Open();
                         int rowsAffected = command.ExecuteNonQuery();
                         if (rowsAffected > 0)
                         {
                              MessageBox.Show("Podaci o članarini su uspešno izmenjeni.");
                              ClanarinaIzmenjena?.Invoke(this, EventArgs.Empty);
                         }
                         else
                         {
                              MessageBox.Show("Greška prilikom izmene podataka o članarini.");
                         }
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show("Greška prilikom izmene podataka: " + ex.Message);
                    }
               }

               Close();
          }
     }
}
