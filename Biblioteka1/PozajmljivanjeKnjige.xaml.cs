using System.Data.SqlClient;
using System.Windows;
using System.Collections.ObjectModel;

namespace LibrarySystem
{
     public partial class PozajmljivanjeKnjige : Window
     {
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
          private ObservableCollection<Knjiga> Knjige { get; set; }

          public PozajmljivanjeKnjige(ObservableCollection<Knjiga> knjige)
          {
               InitializeComponent();
               Knjige = knjige; 
          }

          private void PozajmiButton_Click(object sender, RoutedEventArgs e)
          {
               if (string.IsNullOrWhiteSpace(NazivKnjigeTextBox.Text) ||
                   string.IsNullOrWhiteSpace(ImeTextBox.Text) ||
                   string.IsNullOrWhiteSpace(PrezimeTextBox.Text))
               {
                    MessageBox.Show("Molimo vas unesite sve podatke.");
                    return;
               }

               bool imaClanarinu = ProveriClanarinu();

               if (imaClanarinu)
               {
                    if (IznajmiKnjigu())
                    {
                         MessageBox.Show("Knjiga je uspešno iznajmljena.");

                         foreach (Knjiga knjiga in Knjige)
                         {
                              if (knjiga.Naziv == NazivKnjigeTextBox.Text)
                              {
                                   knjiga.StatusDostupnosti = "Iznajmljena";
                                   break;
                              }
                         }
                    }
                    else
                    {
                         MessageBox.Show("Knjiga ne postoji u bazi ili je već iznajmljena.");
                    }
               }
               else
               {
                    MessageBox.Show("Nemate uplaćenu članarinu.");
               }
          }

          private bool ProveriClanarinu()
          {
               bool imaClanarinu = false;

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    string query = "SELECT StatusClanarine FROM Clanovi WHERE Ime = @Ime AND Prezime = @Prezime";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Ime", ImeTextBox.Text);
                    command.Parameters.AddWithValue("@Prezime", PrezimeTextBox.Text);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                         string statusClanarine = reader["StatusClanarine"].ToString();
                         if (statusClanarine == "Uplacena")
                         {
                              imaClanarinu = true;
                         }
                    }
               }

               return imaClanarinu;
          }

          private bool IznajmiKnjigu()
          {
               bool iznajmljena = false;

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    string query = "UPDATE Knjige SET StatusDostupnosti = 'Iznajmljena' WHERE Naziv = @Naziv AND StatusDostupnosti = 'Dostupna'";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Naziv", NazivKnjigeTextBox.Text);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                         iznajmljena = true;
                    }
                    else
                    {
                         query = "UPDATE EKnjige SET StatusDostupnosti = 'Iznajmljena' WHERE Naziv = @Naziv AND StatusDostupnosti = 'Dostupna'";

                         command = new SqlCommand(query, connection);
                         command.Parameters.AddWithValue("@Naziv", NazivKnjigeTextBox.Text);

                         rowsAffected = command.ExecuteNonQuery();

                         if (rowsAffected > 0)
                         {
                              iznajmljena = true;
                         }
                    }
               }

               return iznajmljena;
          }

     }
}
