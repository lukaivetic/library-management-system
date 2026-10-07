using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace LibrarySystem
{
     public partial class MainWindow : Window
     {
          public ObservableCollection<Knjiga> Knjige { get; set; }

          public MainWindow()
          {
               InitializeComponent();
               Knjige = new ObservableCollection<Knjiga>();
               
          }

          private void PronadjiKnjigu_Click(object sender, RoutedEventArgs e)
          {
               FindBookWindow findBookWindow = new FindBookWindow();
               findBookWindow.Show();
               this.Close();
          }

          private void PozajmiKnjigu_Click(object sender, RoutedEventArgs e)
          {
               PozajmiKnjiguProzor pozajmiKnjiguProzor = new PozajmiKnjiguProzor(Knjige);
               pozajmiKnjiguProzor.Show();
               this.Close();
          }

          private void UplatiClanarinu_Click(object sender, RoutedEventArgs e)
          {
               var uplatiClanarinuProzor = new UplatiClanarinuProzor();
               uplatiClanarinuProzor.Closed += (s, args) => this.Show();
               uplatiClanarinuProzor.Show();
               this.Hide();
          }

          private void Zaposleni_Click(object sender, RoutedEventArgs e)
          {
               Bibliotekari bibliotekariProzor = new Bibliotekari();
               bibliotekariProzor.Show();
               this.Close();
          }

          private void EKnjige_Click(object sender, RoutedEventArgs e)
          {
               EKnigeProzor eKnigeProzor = new EKnigeProzor();
               eKnigeProzor.Show();
               this.Close();
          }
     }
}
