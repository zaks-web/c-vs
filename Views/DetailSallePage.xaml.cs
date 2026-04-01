using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GantsPlace.Models;
using GantsPlace.Services;

namespace GantsPlace.Views
{
    public partial class DetailSallePage : Page
    {
        private readonly MainWindow _main;
        private readonly Salle _salle;

        public DetailSallePage(MainWindow main, Salle salle)
        {
            InitializeComponent();
            _main = main; _salle = salle;
            Loaded += (_, _) => Init();
        }

        private void Init()
        {
            // Image
            var brush = _salle.ImagePath != null ? ImageHelper.LoadImageBrush(_salle.ImagePath) : null;
            if (brush != null) ImgBorder.Background = brush;
            else ImgIcon.Text = _salle.TypeSalle == "Amphithéâtre" ? "🏛️" : _salle.TypeSalle == "Salle de cours" ? "📚" : "🤝";

            // Infos
            TxtNom.Text = _salle.Nom;
            TxtType.Text = _salle.TypeSalle;
            TxtCapacite.Text = $"Capacité : {_salle.Capacite} personnes maximum";
            TxtDescription.Text = _salle.Description;

            TypeBadge.Background = new SolidColorBrush(_salle.TypeSalle == "Amphithéâtre"
                ? Color.FromRgb(0x2D, 0x6C, 0xE0)
                : _salle.TypeSalle == "Salle de cours"
                    ? Color.FromRgb(0x1A, 0xAD, 0x5A)
                    : Color.FromRgb(0x7B, 0x40, 0xC8));

            // Équipements
            var colors = new[] { "#142045", "#1E1030", "#0D2A1A", "#2A1A0D" };
            var fgColors = new[] { "#4F8EF7", "#9B59B6", "#2ECC71", "#F39C12" };
            for (int i = 0; i < _salle.Equipements.Count; i++)
            {
                var badge = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[i % colors.Length])),
                    CornerRadius = new CornerRadius(8), Padding = new Thickness(12, 6, 12, 6),
                    Margin = new Thickness(0, 0, 8, 8)
                };
                badge.Child = new TextBlock
                {
                    Text = _salle.Equipements[i], FontFamily = new FontFamily("Segoe UI"), FontSize = 12,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fgColors[i % fgColors.Length]))
                };
                EquipPanel.Children.Add(badge);
            }

            // Auth
            if (Session.EstConnecte)
            {
                PanelConnecte.Visibility = Visibility.Visible;
                PanelNonConnecte.Visibility = Visibility.Collapsed;
                DpDate.SelectedDate = DateTime.Today.AddDays(1);
                DpDate.DisplayDateStart = DateTime.Today;
                for (int h = 7; h <= 21; h++) { CmbDebut.Items.Add($"{h:00}:00"); CmbFin.Items.Add($"{h+1:00}:00"); }
                CmbDebut.SelectedIndex = 0; CmbFin.SelectedIndex = 0;
            }
            else { PanelConnecte.Visibility = Visibility.Collapsed; PanelNonConnecte.Visibility = Visibility.Visible; }
        }

        private void BtnReserver_Click(object sender, RoutedEventArgs e)
        {
            if (DpDate.SelectedDate == null) { ShowMsg("Veuillez choisir une date.", false); return; }
            var debut = CmbDebut.SelectedItem?.ToString() ?? "";
            var fin   = CmbFin.SelectedItem?.ToString() ?? "";
            if (string.Compare(debut, fin) >= 0) { ShowMsg("L'heure de fin doit être après l'heure de début.", false); return; }

            try
            {
                int creneauId = DataService.GetOrCreateCreneau(debut + ":00", fin + ":00");
                DataService.AjouterReservation(_salle.Id, creneauId, DpDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                ShowMsg($"✓  Réservation confirmée pour le {DpDate.SelectedDate.Value:dd/MM/yyyy} de {debut} à {fin}.", true);
            }
            catch (Exception ex) { ShowMsg($"Erreur : {ex.Message}", false); }
        }

        private void BtnSeConnecter_Click(object sender, RoutedEventArgs e) => _main.NavigateTo("Login");

        private void ShowMsg(string msg, bool ok)
        {
            TxtMsg.Text = msg;
            MsgBorder.Background = ok ? new SolidColorBrush(Color.FromRgb(0x0D, 0x2A, 0x1A)) : new SolidColorBrush(Color.FromRgb(0x2A, 0x0D, 0x0D));
            MsgBorder.BorderBrush = ok ? (Brush)Application.Current.FindResource("SuccessBrush") : (Brush)Application.Current.FindResource("DangerBrush");
            MsgBorder.BorderThickness = new Thickness(1);
            TxtMsg.Foreground = ok ? (Brush)Application.Current.FindResource("SuccessBrush") : (Brush)Application.Current.FindResource("DangerBrush");
            MsgBorder.Visibility = Visibility.Visible;
        }
    }
}
