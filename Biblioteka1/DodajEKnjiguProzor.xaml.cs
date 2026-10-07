using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibrarySystem
{
     public partial class DodajEKnjigu : Window
     {
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
          private EKnigeProzor parentWindow;

          public DodajEKnjigu(EKnigeProzor parent)
          {
               InitializeComponent();
               parentWindow = parent;
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {
               try
               {
                    if (string.IsNullOrEmpty(NazivTextBox.Text) || string.IsNullOrEmpty(AutorTextBox.Text) || DatumIzdavanjaDatePicker.SelectedDate == null || string.IsNullOrEmpty(StatusDostupnostiTextBox.Text))
                    {
                         MessageBox.Show("Molimo vas da popunite sva polja.");
                         return;
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                         connection.Open();

                         string query = "INSERT INTO EKnjige (Naziv, Autor, DatumIzdavanja, StatusDostupnosti) VALUES (@Naziv, @Autor, @DatumIzdavanja, @StatusDostupnosti)";


                         using (SqlCommand command = new SqlCommand(query, connection))
                         {

                              command.Parameters.AddWithValue("@Naziv", NazivTextBox.Text);
                              command.Parameters.AddWithValue("@Autor", AutorTextBox.Text);
                              command.Parameters.AddWithValue("@DatumIzdavanja", DatumIzdavanjaDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd"));
                              command.Parameters.AddWithValue("@StatusDostupnosti", StatusDostupnostiTextBox.Text);


                              command.ExecuteNonQuery();
                         }

                         connection.Close();
                    }

                    this.Close();

                    parentWindow.PrikaziEKnjige();

                    MessageBox.Show("E-knjiga je uspešno dodata!");
               }
               catch (Exception ex)
               {
                    MessageBox.Show("Došlo je do greške prilikom dodavanja e-knjige: " + ex.Message);
               }
          }

          private void IzlazButton_Click(object sender, RoutedEventArgs e)
          {
               this.Close();
          }
     }
}
