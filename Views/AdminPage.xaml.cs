using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GantsPlace.Models;
using GantsPlace.Services;
using Microsoft.Win32;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GantsPlace.Views
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public string SalleNom { get; set; } = "";
        public string UserNom { get; set; } = "";
        public string DateAffichee { get; set; } = "";
        public string Creneau { get; set; } = "";
        public string Statut { get; set; } = "";
        public string Description { get; set; } = "";

        public bool EstAnnulable => Statut == "Confirmée";
        public bool EstConfirmee => Statut == "Confirmée";

        public Brush StatutBackground => Statut == "Confirmée"
            ? new SolidColorBrush(Color.FromRgb(13, 46, 26))
            : new SolidColorBrush(Color.FromRgb(61, 15, 15));

        public Brush StatutForeground => Statut == "Confirmée"
            ? new SolidColorBrush(Color.FromRgb(74, 222, 128))
            : new SolidColorBrush(Color.FromRgb(248, 113, 113));
    }

    public class DoublonViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
    }

    public partial class AdminPage : Page
    {
        private readonly MainWindow _main;
        private readonly ObservableCollection<string> _equipements = new();
        private List<ReservationViewModel> _toutesReservations = new();

        public AdminPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            ListEquipements.ItemsSource = _equipements;

            // if (CmbFiltreStatut != null)
                //     CmbFiltreStatut.SelectedIndex = 0;

            ChargerSalles();
            ChargerToutesReservations();
            DetecterDoublons();
        }

        // ───────────── NAVIGATION ─────────────
        private void AfficherOnglet(string onglet)
        {
            PanelAjouter.Visibility = Visibility.Collapsed;
            PanelBtnValider.Visibility = Visibility.Collapsed;
            PanelSupprimer.Visibility = Visibility.Collapsed;
            PanelPlanning.Visibility = Visibility.Collapsed;
            PanelReservations.Visibility = Visibility.Collapsed;

            var inactif = (Style)FindResource("TabBtnStyle");
            var actif = (Style)FindResource("TabBtnActiveStyle");

            BtnTabAjouter.Style = inactif;
            BtnTabSupprimer.Style = inactif;
            BtnTabPlanning.Style = inactif;
            BtnTabReservations.Style = inactif;

            switch (onglet)
            {
                case "Ajouter":
                    PanelAjouter.Visibility = Visibility.Visible;
                    PanelBtnValider.Visibility = Visibility.Visible;
                    BtnTabAjouter.Style = actif;
                    break;

                case "Supprimer":
                    PanelSupprimer.Visibility = Visibility.Visible;
                    BtnTabSupprimer.Style = actif;
                    ChargerSalles();
                    break;

                case "Planning":
                    PanelPlanning.Visibility = Visibility.Visible;
                    BtnTabPlanning.Style = actif;
                    ChargerPlanningReservations(null);
                    DetecterDoublons();
                    break;

                case "Reservations":
                    PanelReservations.Visibility = Visibility.Visible;
                    BtnTabReservations.Style = actif;
                    ChargerToutesReservations();
                    AppliquerFiltreReservations();
                    break;
            }
        }

        private void BtnTabAjouter_Click(object s, RoutedEventArgs e) => AfficherOnglet("Ajouter");
        private void BtnTabSupprimer_Click(object s, RoutedEventArgs e) => AfficherOnglet("Supprimer");
        private void BtnTabPlanning_Click(object s, RoutedEventArgs e) => AfficherOnglet("Planning");
        private void BtnTabReservations_Click(object s, RoutedEventArgs e) => AfficherOnglet("Reservations");

        // ───────────── AJOUT SALLE ─────────────
        private void BtnParcourirImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.jpeg" };
            if (dlg.ShowDialog() == true)
                TxtImagePath.Text = dlg.FileName;
        }

        private void BtnAjouterEquipement_Click(object sender, RoutedEventArgs e)
        {
            var nom = TxtNouvelEquipement?.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(nom)) return;

            if (!_equipements.Contains(nom))
                _equipements.Add(nom);

            TxtNouvelEquipement.Text = "";
        }

        private void BtnSupprimerEquipementListe_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string nom)
                _equipements.Remove(nom);
        }

        private void BtnValiderAjout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nom = TxtNomSalle.Text.Trim();
                string type = (CmbTypeSalle.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                if (int.TryParse(TxtCapacite.Text, out int capacite) && !string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(type))
                {
                    string description = TxtDescription.Text;
                    string imagePath = TxtImagePath.Text;

                    int newId = DataService.AjouterSalle(nom, type, capacite, description, imagePath);

                    // Ajouter équipements
                    foreach (string equip in ListEquipements.ItemsSource as List<string> ?? new List<string>())
                    {
                        DataService.AjouterEquipement(newId, equip);
                    }

                    MessageBox.Show("Salle ajoutée avec succès et visible dans Explorer !");
                    // Reset form
                    TxtNomSalle.Text = TxtCapacite.Text = TxtDescription.Text = TxtImagePath.Text = "";
                    (ListEquipements.ItemsSource as ObservableCollection<string>)?.Clear();
                    DataService.LoadSalles(); // Refresh global cache
                }
                else
                {
                    MessageBox.Show("Vérifiez les champs (nom, type, capacité valide).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ───────────── SUPPRESSION SALLE ─────────────
        private void ChargerSalles()
        {
            DataService.LoadSalles();
            ListSallesSupprimer.ItemsSource = DataService.Salles?.ToList();
        }

        private void BtnSupprimerSalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int idSalle)
            {
                var confirmation = MessageBox.Show("Supprimer définitivement cette salle (équipements + réservations impactées) ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirmation == MessageBoxResult.Yes)
                {
                    DataService.SupprimerSalle(idSalle);
                    DataService.LoadSalles(); // Refresh cache
                    MessageBox.Show("Salle supprimée de la BD et de l'app !");
                }
            }
        }

        // ───────────── PLANNING ─────────────
        private void ChargerPlanningReservations(DateTime? filtre)
        {
            var reservations = DataService.GetToutesReservations();
            if (reservations == null) return;

            ListPlanningReservations.ItemsSource = reservations
                .Where(r => filtre == null || r.Date.Date == filtre.Value.Date)
                .Select(ToViewModel)
                .ToList();
        }

        private void DpFiltreDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            ChargerPlanningReservations(DpFiltreDate?.SelectedDate);
        }

        private void BtnAfficherToutPlanning_Click(object sender, RoutedEventArgs e)
        {
            if (DpFiltreDate != null)
                DpFiltreDate.SelectedDate = null;

            ChargerPlanningReservations(null);
        }

        // ───────────── DOUBLONS ─────────────
        private void DetecterDoublons()
        {
            var reservations = DataService.GetToutesReservations();
            if (reservations == null) return;

            var doublons = reservations
                .GroupBy(r => new { r.SalleId, r.Date.Date, r.HeureDebut })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .Select(r => new DoublonViewModel
                {
                    Id = r.Id,
                    Description = $"{r.SalleNom} {r.Date}"
                }).ToList();

            PanelDoublons.Visibility = doublons.Any() ? Visibility.Visible : Visibility.Collapsed;
            ListDoublons.ItemsSource = doublons;
        }

        private void BtnAnnulerDoublon_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var confirmation = MessageBox.Show("Annuler ce doublon ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmation == MessageBoxResult.Yes)
                {
                    DataService.AnnulerReservation(new Reservation { Id = id });
                    ChargerPlanningReservations(DpFiltreDate?.SelectedDate);
                    DetecterDoublons();
                    MessageBox.Show("Doublon annulé.");
                }
            }
        }

        // ───────────── RESERVATIONS ─────────────
        private void ChargerToutesReservations()
        {
            var data = DataService.GetToutesReservations();
            _toutesReservations = data?.Select(ToViewModel).ToList() ?? new();
        }

        private void AppliquerFiltreReservations()
        {
            if (!IsLoaded || _toutesReservations == null) return;

            var recherche = TxtRechercheRes?.Text?.ToLower() ?? "";
            var liste = _toutesReservations
                .Where(r => r.SalleNom.ToLower().Contains(recherche) ||
                            r.UserNom.ToLower().Contains(recherche))
                .ToList();

            ListToutesReservations.ItemsSource = liste;
        }

        private void TxtRechercheRes_Changed(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            AppliquerFiltreReservations();
        }

        // private void CmbFiltreStatut_Changed(object sender, SelectionChangedEventArgs e)
        // {
        //     if (!IsLoaded) return;
        //     AppliquerFiltreReservations();
        // }

        private void BtnAnnulerReservation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var confirmation = MessageBox.Show("Annuler cette réservation ? L'utilisateur recevra une notification.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmation == MessageBoxResult.Yes)
                {
                    DataService.AnnulerReservation(new Reservation { Id = id });
                    ChargerToutesReservations();
                    AppliquerFiltreReservations();
                    MessageBox.Show("Réservation annulée et statut mis à jour.");
                }
            }
        }

        // ───────────── HELPERS ─────────────
        private static ReservationViewModel ToViewModel(Reservation r) => new()
        {
            Id = r.Id,
            SalleNom = r.SalleNom ?? "",
            UserNom = r.UserNom ?? "",
            DateAffichee = r.Date.ToString("dd/MM/yyyy"),
            Creneau = $"{r.HeureDebut:hh\\:mm} - {r.HeureFin:hh\\:mm}",
            Statut = r.Statut ?? ""
        };
    }
}