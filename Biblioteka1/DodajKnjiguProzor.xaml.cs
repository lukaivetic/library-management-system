using System;
using System.Windows;

namespace Biblioteka1
{

     public class Knjiga
     {
          public string Naziv { get; set; }
          public string Autor { get; set; }
          public DateTime DatumIzdavanja { get; set; }
          public string StatusDostupnosti { get; set; }
          public string VrstaKorica { get; set; }
     }

     public partial class DodajKnjiguProzor : Window
     {
          public Knjiga NovaKnjiga { get; private set; }

          public DodajKnjiguProzor()
          {
               InitializeComponent();
          }

          private void PotvrdiButton_Click(object sender, RoutedEventArgs e)
          {

               if (string.IsNullOrWhiteSpace(NazivTextBox.Text) || string.IsNullOrWhiteSpace(AutorTextBox.Text) ||
                   string.IsNullOrWhiteSpace(DatumIzdavanjaTextBox.Text) || string.IsNullOrWhiteSpace(StatusDostupnostiTextBox.Text) ||
                   string.IsNullOrWhiteSpace(VrstaKoricaTextBox.Text))
               {
                    MessageBox.Show("Svi podaci moraju biti uneti.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
               }


               if (!DateTime.TryParse(DatumIzdavanjaTextBox.Text, out DateTime datumIzdavanja))
               {
                    MessageBox.Show("Neispravan format datuma.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
               }


               NovaKnjiga = new Knjiga
               {
                    Naziv = NazivTextBox.Text,
                    Autor = AutorTextBox.Text,
                    DatumIzdavanja = datumIzdavanja,
                    StatusDostupnosti = StatusDostupnostiTextBox.Text,
                    VrstaKorica = VrstaKoricaTextBox.Text
               };


               DialogResult = true;
          }
     }
}
