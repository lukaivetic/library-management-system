using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace LibrarySystem
{
     public partial class UplatiClanarinuProzor : Window
     {
          ObservableCollection<Osoba> clanovi = new ObservableCollection<Osoba>();

          public UplatiClanarinuProzor()
          {
               InitializeComponent();
               GetClanoviFromDatabase();
               ClanarineListView.ItemsSource = clanovi;
          }

          private void GetClanoviFromDatabase()
          {
               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "SELECT Ime, Prezime, DatumUplate, TipClanarine FROM Clanovi WHERE StatusClanarine = 'Uplacena'";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                         string tipClanarine = reader["TipClanarine"].ToString();
                         DateTime datumUplate = Convert.ToDateTime(reader["DatumUplate"]);
                         DateTime vaziDo = IzracunajDatumVazenja(datumUplate, tipClanarine);
                         clanovi.Add(new Osoba
                         {
                              Ime = reader["Ime"].ToString(),
                              Prezime = reader["Prezime"].ToString(),
                              DatumUplate = datumUplate,
                              VaziDo = vaziDo,
                              StatusClanarine = tipClanarine
                         });
                    }
               }
          }

          private DateTime IzracunajDatumVazenja(DateTime datumUplate, string tipClanarine)
          {
               switch (tipClanarine)
               {
                    case "Nedeljna":
                         return datumUplate.AddDays(7);
                    case "Dve nedelje":
                         return datumUplate.AddDays(14);
                    case "Mesečna":
                         return datumUplate.AddMonths(1);
                    case "Dva meseca":
                         return datumUplate.AddMonths(2);
                    case "Pola godine":
                         return datumUplate.AddMonths(6);
                    case "Godišnja":
                         return datumUplate.AddYears(1);
                    default:
                         return datumUplate;
               }
          }

          private void PretraziButton_Click(object sender, RoutedEventArgs e)
          {
               string ime = ImeTextBox.Text.ToLower();
               string prezime = PrezimeTextBox.Text.ToLower();

               ObservableCollection<Osoba> filteredClanovi = new ObservableCollection<Osoba>(
                   clanovi.Where(clan =>
                       clan.Ime.ToLower().Contains(ime) &&
                       clan.Prezime.ToLower().Contains(prezime)
                   ));

               ClanarineListView.ItemsSource = filteredClanovi;
          }

          private void IzadjiButton_Click(object sender, RoutedEventArgs e)
          {
               Close();
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {
               UplatiClanarinuProzor2 uplatiClanarinuProzor2 = new UplatiClanarinuProzor2(this);
               uplatiClanarinuProzor2.ClanarinaUplacena += OsveziListuClanova;
               uplatiClanarinuProzor2.Show();
          }

          private void OsveziListuClanova(object sender, EventArgs e)
          {
               clanovi.Clear();
               GetClanoviFromDatabase();
               ClanarineListView.ItemsSource = clanovi;
          }

          private void OtkaziClanarinuButton_Click(object sender, RoutedEventArgs e)
          {
               if (ClanarineListView.SelectedItem == null)
               {
                    MessageBox.Show("Molimo selektujte člana čiju članarinu želite da otkažete.");
                    return;
               }

               Osoba selectedOsoba = (Osoba)ClanarineListView.SelectedItem;
               string ime = selectedOsoba.Ime;
               string prezime = selectedOsoba.Prezime;

               string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";
               string query = "DELETE FROM Clanovi WHERE Ime = @Ime AND Prezime = @Prezime";

               try
               {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                         SqlCommand command = new SqlCommand(query, connection);
                         command.Parameters.AddWithValue("@Ime", ime);
                         command.Parameters.AddWithValue("@Prezime", prezime);
                         connection.Open();
                         int rowsAffected = command.ExecuteNonQuery();

                         if (rowsAffected > 0)
                         {
                              MessageBox.Show("Članarina je uspešno otkazana i član je uklonjen iz baze.");
                              OsveziListuClanova(null, null);
                         }
                         else
                         {
                              MessageBox.Show("Greška prilikom otkazivanja članarine.");
                         }
                    }
               }
               catch (Exception ex)
               {
                    MessageBox.Show("Greška prilikom otkazivanja članarine: " + ex.Message);
               }
          }

          private void IzmeniButton_Click(object sender, RoutedEventArgs e)
          {
               if (ClanarineListView.SelectedItem == null)
               {
                    MessageBox.Show("Molimo izaberite člana čiju članarinu želite da izmenite.");
                    return;
               }

               Osoba selectedOsoba = (Osoba)ClanarineListView.SelectedItem;
               IzmeniClanarinuProzor izmeniProzor = new IzmeniClanarinuProzor(selectedOsoba);
               izmeniProzor.ClanarinaIzmenjena += OsveziListuClanova; 
               izmeniProzor.Show();
          }
     }

     public class Osoba
     {
          public string Ime { get; set; }
          public string Prezime { get; set; }
          public DateTime DatumUplate { get; set; }
          public DateTime VaziDo { get; set; }
          public string StatusClanarine { get; set; }
     }
}
