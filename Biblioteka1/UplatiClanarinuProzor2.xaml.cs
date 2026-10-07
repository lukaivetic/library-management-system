using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibrarySystem
{
     public partial class UplatiClanarinuProzor2 : Window
     {
          public event EventHandler ClanarinaUplacena;
          private Window previousWindow;

          public UplatiClanarinuProzor2(Window previousWindow)
          {
               InitializeComponent();
               this.previousWindow = previousWindow;
               this.Owner = previousWindow;
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {
               string ime = ImeTextBox.Text;
               string prezime = PrezimeTextBox.Text;
               string odabranaClanarina = ((ComboBoxItem)ClanarinaComboBox.SelectedItem)?.Content.ToString();
               if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime) || string.IsNullOrWhiteSpace(odabranaClanarina))
               {
                    MessageBox.Show("Morate popuniti sva polja pre uplate članarine.");
                    return;
               }

               string[] clanarinaParts = odabranaClanarina.Split('-');
               string tipClanarine = clanarinaParts[0].Trim(); 

               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "INSERT INTO Clanovi (Ime, Prezime, DatumUplate, StatusClanarine, TipClanarine) VALUES (@Ime, @Prezime, @DatumUplate, @StatusClanarine, @TipClanarine)";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Ime", ime);
                    command.Parameters.AddWithValue("@Prezime", prezime);
                    command.Parameters.AddWithValue("@DatumUplate", DateTime.Now);
                    command.Parameters.AddWithValue("@StatusClanarine", "Uplacena");
                    command.Parameters.AddWithValue("@TipClanarine", tipClanarine); 

                    try
                    {
                         connection.Open();
                         command.ExecuteNonQuery();
                         MessageBox.Show("Članarina je uspešno uplaćena!");

                         ClanarinaUplacena?.Invoke(this, EventArgs.Empty);
                         Close();
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show("Greška prilikom uplate članarine: " + ex.Message);
                    }
               }
          }

          private void ClanarinaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
          {
               ComboBox comboBox = sender as ComboBox;
               ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
               if (selectedItem != null)
               {
                    string cena = selectedItem.Tag.ToString();
                    CenaTextBlock.Text = cena + " dinara";
               }
          }

     }
}
