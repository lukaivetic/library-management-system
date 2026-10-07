using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibrarySystem
{
     public partial class VratiKnjiguProzor : Window
     {
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";

          public VratiKnjiguProzor()
          {
               InitializeComponent();
          }

          private void VratiButton_Click(object sender, RoutedEventArgs e)
          {
               string nazivKnjige = NazivKnjigeTextBox.Text;
               string ime = ImeTextBox.Text;
               string prezime = PrezimeTextBox.Text;

               if (string.IsNullOrWhiteSpace(nazivKnjige) || string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime))
               {
                    MessageBox.Show("Molimo vas unesite sve podatke.");
                    return;
               }

               bool statusDostupnosti = ProveriStatusDostupnosti(nazivKnjige);

               if (statusDostupnosti)
               {
                    PromeniStatusKnjige(nazivKnjige, "Dostupna");
                    MessageBox.Show("Knjiga je uspešno vraćena.");
               }
               else
               {
                    MessageBox.Show("Knjiga nije iznajmljena ili ne postoji u bazi.");
               }
          }

          private bool ProveriStatusDostupnosti(string nazivKnjige)
          {
               bool statusDostupnosti = false;

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    string queryKnjige = "SELECT StatusDostupnosti FROM Knjige WHERE Naziv = @Naziv";
                    string queryEKnjige = "SELECT StatusDostupnosti FROM EKnjige WHERE Naziv = @Naziv";

                    SqlCommand commandKnjige = new SqlCommand(queryKnjige, connection);
                    SqlCommand commandEKnjige = new SqlCommand(queryEKnjige, connection);

                    commandKnjige.Parameters.AddWithValue("@Naziv", nazivKnjige);
                    commandEKnjige.Parameters.AddWithValue("@Naziv", nazivKnjige);

                    connection.Open();

                    using (SqlDataReader readerKnjige = commandKnjige.ExecuteReader())
                    {
                         if (readerKnjige.Read())
                         {
                              string status = readerKnjige["StatusDostupnosti"].ToString();
                              if (status == "Iznajmljena")
                              {
                                   statusDostupnosti = true;
                              }
                         }
                    }

                    using (SqlDataReader readerEKnjige = commandEKnjige.ExecuteReader())
                    {
                         if (readerEKnjige.Read())
                         {
                              string status = readerEKnjige["StatusDostupnosti"].ToString();
                              if (status == "Iznajmljena")
                              {
                                   statusDostupnosti = true;
                              }
                         }
                    }
               }

               return statusDostupnosti;
          }

          private void PromeniStatusKnjige(string nazivKnjige, string noviStatus)
          {
               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    string queryKnjige = "UPDATE Knjige SET StatusDostupnosti = @NoviStatus WHERE Naziv = @Naziv";
                    string queryEKnjige = "UPDATE EKnjige SET StatusDostupnosti = @NoviStatus WHERE Naziv = @Naziv";

                    SqlCommand commandKnjige = new SqlCommand(queryKnjige, connection);
                    SqlCommand commandEKnjige = new SqlCommand(queryEKnjige, connection);

                    commandKnjige.Parameters.AddWithValue("@NoviStatus", noviStatus);
                    commandKnjige.Parameters.AddWithValue("@Naziv", nazivKnjige);

                    commandEKnjige.Parameters.AddWithValue("@NoviStatus", noviStatus);
                    commandEKnjige.Parameters.AddWithValue("@Naziv", nazivKnjige);

                    connection.Open();
                    commandKnjige.ExecuteNonQuery();
                    commandEKnjige.ExecuteNonQuery();
               }
          }

          private void IzadjiButton_Click(object sender, RoutedEventArgs e)
          {
               this.Close();
          }
     }
}
