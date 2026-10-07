using System;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibrarySystem
{
     public partial class IzmeniKnjiguProzor : Window
     {
          private readonly Knjiga knjiga;
          private readonly ObservableCollection<Knjiga> knjige;
          private readonly ObservableCollection<EKnjiga> eKnjige;

          public IzmeniKnjiguProzor(Knjiga knjiga, ObservableCollection<Knjiga> knjige, ObservableCollection<EKnjiga> eKnjige)
          {
               InitializeComponent();
               this.knjiga = knjiga;
               this.knjige = knjige;
               this.eKnjige = eKnjige;

               NazivTextBox.Text = knjiga.Naziv;
               AutorTextBox.Text = knjiga.Autor;
               DatumIzdavanjaDatePicker.SelectedDate = knjiga.DatumIzdavanja;
               VrstaKoricaTextBox.Text = knjiga.VrstaKorica;
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {
               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string queryKnjige = "UPDATE Knjige SET Naziv = @Naziv, Autor = @Autor, DatumIzdavanja = @DatumIzdavanja, VrstaKorica = @VrstaKorica WHERE Naziv = @Naziv";
               string queryEKnjige = "UPDATE EKnjige SET Naziv = @Naziv, Autor = @Autor, DatumIzdavanja = @DatumIzdavanja WHERE Naziv = @Naziv";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    try
                    {
                         connection.Open();

                         SqlCommand commandKnjige = new SqlCommand(queryKnjige, connection);
                         commandKnjige.Parameters.AddWithValue("@Naziv", NazivTextBox.Text);
                         commandKnjige.Parameters.AddWithValue("@Autor", AutorTextBox.Text);
                         commandKnjige.Parameters.AddWithValue("@DatumIzdavanja", DatumIzdavanjaDatePicker.SelectedDate);
                         commandKnjige.Parameters.AddWithValue("@VrstaKorica", VrstaKoricaTextBox.Text);

                         int rowsAffectedKnjige = commandKnjige.ExecuteNonQuery();

                         SqlCommand commandEKnjige = new SqlCommand(queryEKnjige, connection);
                         commandEKnjige.Parameters.AddWithValue("@Naziv", NazivTextBox.Text);
                         commandEKnjige.Parameters.AddWithValue("@Autor", AutorTextBox.Text);
                         commandEKnjige.Parameters.AddWithValue("@DatumIzdavanja", DatumIzdavanjaDatePicker.SelectedDate);

                         int rowsAffectedEKnjige = commandEKnjige.ExecuteNonQuery();


                         if (rowsAffectedKnjige > 0 || rowsAffectedEKnjige > 0)
                         {

                              var updatedKnjiga = knjige.FirstOrDefault(k => k.Naziv == knjiga.Naziv);
                              if (updatedKnjiga != null)
                              {
                                   updatedKnjiga.Autor = AutorTextBox.Text;
                                   updatedKnjiga.DatumIzdavanja = DatumIzdavanjaDatePicker.SelectedDate ?? DateTime.MinValue;
                                   updatedKnjiga.VrstaKorica = VrstaKoricaTextBox.Text;
                              }
                              else
                              {
                                   var updatedEKnjiga = eKnjige.FirstOrDefault(ek => ek.Naziv == knjiga.Naziv);
                                   if (updatedEKnjiga != null)
                                   {
                                        updatedEKnjiga.Autor = AutorTextBox.Text;
                                        updatedEKnjiga.DatumIzdavanja = DatumIzdavanjaDatePicker.SelectedDate ?? DateTime.MinValue;
                                   }
                              }

                              MessageBox.Show("Knjiga je uspešno izmenjena!");
                         }
                         else
                         {
                              MessageBox.Show("Došlo je do greške prilikom izmene knjige.");
                         }
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show("Greška prilikom izmene knjige: " + ex.Message);
                    }
               }

               Close();
          }
     }
}
