using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using GantsPlace.Models;
using GantsPlace.Services;

namespace GantsPlace.Views
{
    public partial class HistoriquePage : Page
    {
        private readonly MainWindow _main;
        public HistoriquePage(MainWindow main) { InitializeComponent(); _main = main; Loaded += (_, _) => Charger(); }

        private void Charger()
        {
            ContenuPanel.Children.Clear();

            if (!Session.EstConnecte)
            {
                ContenuPanel.Children.Add(CreateNotConnected());
                BadgeCount.Visibility = Visibility.Collapsed;
                return;
            }

            DataService.LoadUserReservations(Session.UtilisateurConnecte!);
            var reservations = Session.UtilisateurConnecte!.Reservations.ToList();
            TxtCount.Text = $"{reservations.Count} réservation{(reservations.Count > 1 ? "s" : "")}";

            if (reservations.Count == 0)
            {
                ContenuPanel.Children.Add(CreateEmpty());
                return;
            }

            foreach (var r in reservations)
                ContenuPanel.Children.Add(CreateCard(r));
        }

        private UIElement CreateCard(Reservation r)
        {
            var card = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x20, 0x30)),
                CornerRadius = new CornerRadius(14), BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x25, 0x2D, 0x45)),
                Margin = new Thickness(0, 0, 0, 14), Padding = new Thickness(24),
                Effect = new DropShadowEffect { ShadowDepth = 0, BlurRadius = 16, Opacity = 0.2, Color = Colors.Black }
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Infos gauche
            var left = new StackPanel();
            // Nom salle + type
            var topRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            topRow.Children.Add(new TextBlock { Text = r.SalleNom, FontFamily = new FontFamily("Segoe UI"), FontSize = 17, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(0xEE, 0xF0, 0xF8)), VerticalAlignment = VerticalAlignment.Center });
            var typeBadge = new Border { Background = new SolidColorBrush(Color.FromRgb(0x14, 0x20, 0x45)), CornerRadius = new CornerRadius(5), Padding = new Thickness(8, 3, 8, 3), Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            typeBadge.Child = new TextBlock { Text = r.TypeSalle, FontSize = 10, Foreground = new SolidColorBrush(Color.FromRgb(0x4F, 0x8E, 0xF7)), FontFamily = new FontFamily("Segoe UI") };
            topRow.Children.Add(typeBadge);
            left.Children.Add(topRow);

            // Date + heure
            var details = new StackPanel { Orientation = Orientation.Horizontal };
            var dateChip = CreateChip($"📅  {r.DateFormatee}", "#142045", "#4F8EF7");
            var heureChip = CreateChip($"🕐  {r.HeuresFormatees}", "#0D2A1A", "#2ECC71");
            details.Children.Add(dateChip); details.Children.Add(heureChip);
            left.Children.Add(details);

            Grid.SetColumn(left, 0);
            grid.Children.Add(left);

            // Badge statut + bouton annuler (droite)
            var right = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
            var badge = CreateStatutBadge(r.Statut);
            right.Children.Add(badge);

            if (r.Statut == "Confirmée")
            {
                var btnAnnuler = new Button
                {
                    Content = "Annuler", Margin = new Thickness(0, 8, 0, 0),
                    Style = (Style)Application.Current.FindResource("DangerButtonStyle"),
                    Tag = r
                };
 btnAnnuler.Click += (s, e) => {
                    if (MessageBox.Show($"Annuler la réservation de {r.SalleNom} ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    { DataService.AnnulerReservation(r); Charger(); }
                };
                right.Children.Add(btnAnnuler);
            }

            Grid.SetColumn(right, 1);
            grid.Children.Add(right);

            card.Child = grid;
            return card;
        }

        private static Border CreateChip(string text, string bg, string fg)
        {
            var b = new Border { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bg)), CornerRadius = new CornerRadius(6), Padding = new Thickness(10, 5, 10, 5), Margin = new Thickness(0, 0, 8, 0) };
            b.Child = new TextBlock { Text = text, FontSize = 12, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fg)), FontFamily = new FontFamily("Segoe UI") };
            return b;
        }

        private static UIElement CreateStatutBadge(string statut)
        {
            var (bg, fg, text) = statut switch
            {
                "Confirmée" => ("#0D2A1A", "#2ECC71", "✅  Confirmée"),
                "Annulee"   => ("#2A0D0D", "#E74C3C", "❌  Annulée"),
                _           => ("#2A200D", "#F39C12", "⏳  En attente")
            };
            var b = new Border { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bg)), CornerRadius = new CornerRadius(8), Padding = new Thickness(12, 6, 12, 6), BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fg)) };
            b.Child = new TextBlock { Text = text, FontSize = 12, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fg)), FontFamily = new FontFamily("Segoe UI") };
            return b;
        }

        private UIElement CreateNotConnected()
        {
            var p = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 80, 0, 0) };
            p.Children.Add(new TextBlock { Text = "🔒", FontSize = 52, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 16) });
            p.Children.Add(new TextBlock { Text = "Connectez-vous pour voir vos réservations", FontFamily = new FontFamily("Segoe UI"), FontSize = 16, Foreground = new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)), HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 20) });
            var btn = new Button { Content = "Se connecter", Style = (Style)Application.Current.FindResource("PrimaryButtonStyle"), HorizontalAlignment = HorizontalAlignment.Center };
            btn.Click += (s, e) => _main.NavigateTo("Login");
            p.Children.Add(btn);
            return p;
        }

        private static UIElement CreateEmpty()
        {
            var p = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 80, 0, 0) };
            p.Children.Add(new TextBlock { Text = "📋", FontSize = 52, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 16) });
            p.Children.Add(new TextBlock { Text = "Aucune réservation pour le moment", FontFamily = new FontFamily("Segoe UI"), FontSize = 16, Foreground = new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)), HorizontalAlignment = HorizontalAlignment.Center });
            return p;
        }
    }
}
