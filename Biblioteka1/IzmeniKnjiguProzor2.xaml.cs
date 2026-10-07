using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibrarySystem
{
     public partial class IzmeniKnjiguProzor2 : Window
     {
          private readonly Knjiga knjiga;
          private readonly FindBookWindow parentWindow;

          public Knjiga NovaKnjiga { get; private set; } 

          public IzmeniKnjiguProzor2(Knjiga knjiga, FindBookWindow parentWindow)
          {
               InitializeComponent();
               this.knjiga = knjiga;
               this.parentWindow = parentWindow;

               NazivTextBox.Text = knjiga.Naziv;
               AutorTextBox.Text = knjiga.Autor;
               DatumIzdavanjaDatePicker.SelectedDate = knjiga.DatumIzdavanja;
               VrstaKoricaTextBox.Text = knjiga.VrstaKorica;
          }

          private void SacuvajIzmene_Click(object sender, RoutedEventArgs e)
          {
               string naziv = NazivTextBox.Text;
               string autor = AutorTextBox.Text;
               DateTime datumIzdavanja = DatumIzdavanjaDatePicker.SelectedDate ?? DateTime.MinValue;
               string vrstaKorica = VrstaKoricaTextBox.Text;

               if (string.IsNullOrWhiteSpace(naziv) || string.IsNullOrWhiteSpace(autor) || string.IsNullOrWhiteSpace(vrstaKorica))
               {
                    MessageBox.Show("Molimo vas unesite sve podatke.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
               }

               if (datumIzdavanja == DateTime.MinValue)
               {
                    MessageBox.Show("Molimo vas unesite validan datum izdavanja.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
               }

               UpdateBookInDatabase(naziv, autor, datumIzdavanja, vrstaKorica);

 
               NovaKnjiga = new Knjiga
               {
                    Naziv = naziv,
                    Autor = autor,
                    DatumIzdavanja = datumIzdavanja,
                    VrstaKorica = vrstaKorica,
                    StatusDostupnosti = knjiga.StatusDostupnosti 
               };

               DialogResult = true;
          }

          private void UpdateBookInDatabase(string naziv, string autor, DateTime datumIzdavanja, string vrstaKorica)
          {
               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "UPDATE Knjige SET Naziv = @Naziv, Autor = @Autor, DatumIzdavanja = @DatumIzdavanja, VrstaKorica = @VrstaKorica WHERE Naziv = @OldNaziv AND Autor = @OldAutor";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Naziv", naziv);
                    command.Parameters.AddWithValue("@Autor", autor);
                    command.Parameters.AddWithValue("@DatumIzdavanja", datumIzdavanja);
                    command.Parameters.AddWithValue("@VrstaKorica", vrstaKorica);
                    command.Parameters.AddWithValue("@OldNaziv", knjiga.Naziv);
                    command.Parameters.AddWithValue("@OldAutor", knjiga.Autor);

                    try
                    {
                         connection.Open();
                         int rowsAffected = command.ExecuteNonQuery();

                         if (rowsAffected <= 0)
                         {
                              MessageBox.Show("Ažuriranje podataka nije uspelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                         }
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show("Greška prilikom ažuriranja podataka: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
               }
          }
     }
}
