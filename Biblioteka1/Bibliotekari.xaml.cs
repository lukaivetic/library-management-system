using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.SqlClient;

namespace LibrarySystem
{
     public partial class Bibliotekari : Window
     {
          private string connectionString = "Server=LukaIvetic;Database=Biblioteka;Integrated Security=True;";

          public Bibliotekari()
          {
               InitializeComponent();
               PrikaziBibliotekare();
          }

          public void PrikaziBibliotekare()
          {
               string query = "SELECT Ime, Prezime, Email, Telefon, Adresa FROM Bibliotekari";
               List<Bibliotekar> bibliotekari = new List<Bibliotekar>();

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                         Bibliotekar bibliotekar = new Bibliotekar();
                         bibliotekar.Ime = reader["Ime"].ToString();
                         bibliotekar.Prezime = reader["Prezime"].ToString();
                         bibliotekar.Email = reader["Email"].ToString();
                         bibliotekar.Telefon = reader["Telefon"].ToString();
                         bibliotekar.Adresa = reader["Adresa"].ToString();
                         bibliotekari.Add(bibliotekar);
                    }
               }

               BibliotekariListView.ItemsSource = bibliotekari;
          }

          private void PretraziButton_Click(object sender, RoutedEventArgs e)
          {
               string ime = ImeTextBox.Text;
               string prezime = PrezimeTextBox.Text;
               string query = $"SELECT Ime, Prezime, Email, Telefon, Adresa FROM Bibliotekari WHERE Ime LIKE '%{ime}%' AND Prezime LIKE '%{prezime}%'";
               List<Bibliotekar> bibliotekari = new List<Bibliotekar>();

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                         Bibliotekar bibliotekar = new Bibliotekar();
                         bibliotekar.Ime = reader["Ime"].ToString();
                         bibliotekar.Prezime = reader["Prezime"].ToString();
                         bibliotekar.Email = reader["Email"].ToString();
                         bibliotekar.Telefon = reader["Telefon"].ToString();
                         bibliotekar.Adresa = reader["Adresa"].ToString();
                         bibliotekari.Add(bibliotekar);
                    }
               }

               BibliotekariListView.ItemsSource = bibliotekari;
          }

          private void Izlaz_Click(object sender, RoutedEventArgs e)
          {
               MainWindow mainWindow = new MainWindow();
               mainWindow.Show();
               this.Close();
          }

          private void DodajButton_Click(object sender, RoutedEventArgs e)
          {
               DodajBibliotekaraProzor dodajBibliotekaraProzor = new DodajBibliotekaraProzor(this);
               dodajBibliotekaraProzor.ShowDialog();
          }

          private void ObrisiButton_Click(object sender, RoutedEventArgs e)
          {
               if (BibliotekariListView.SelectedItem == null)
               {
                    MessageBox.Show("Molimo selektujte bibliotekara koga želite da obrišete.");
                    return;
               }

               try
               {

                    Bibliotekar selectedBibliotekar = (Bibliotekar)BibliotekariListView.SelectedItem;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                         connection.Open();
                         string query = $"DELETE FROM Bibliotekari WHERE Ime = '{selectedBibliotekar.Ime}' AND Prezime = '{selectedBibliotekar.Prezime}'";
                         SqlCommand command = new SqlCommand(query, connection);
                         command.ExecuteNonQuery();
                    }


                    ((List<Bibliotekar>)BibliotekariListView.ItemsSource).Remove(selectedBibliotekar);
                    BibliotekariListView.Items.Refresh();

                    MessageBox.Show("Bibliotekar je uspešno obrisan.");
               }
               catch (Exception ex)
               {
                    MessageBox.Show("Greška prilikom brisanja bibliotekara: " + ex.Message);
               }
          }

          private void IzmeniButton_Click(object sender, RoutedEventArgs e)
          {
               if (BibliotekariListView.SelectedItem == null)
               {
                    MessageBox.Show("Molimo selektujte bibliotekara koga želite da izmenite.");
                    return;
               }

               Bibliotekar selectedBibliotekar = (Bibliotekar)BibliotekariListView.SelectedItem;
               IzmeniBibliotekaraProzor izmeniBibliotekaraProzor = new IzmeniBibliotekaraProzor(selectedBibliotekar, this);
               izmeniBibliotekaraProzor.ShowDialog();
          }
     }

     public class Bibliotekar
     {
          public string Ime { get; set; }
          public string Prezime { get; set; }
          public string Email { get; set; }
          public string Telefon { get; set; }
          public string Adresa { get; set; }
     }
}
