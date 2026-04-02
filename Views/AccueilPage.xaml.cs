using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using GantsPlace.Models;
using GantsPlace.Services;
using System.Collections.Generic;

namespace GantsPlace.Views
{
    public partial class AccueilPage : Page
    {
        private readonly MainWindow _main;
        private readonly string[] _badgeColors = { "#4F8EF7", "#9B59B6", "#2ECC71", "#F39C12", "#E74C3C" };

        public AccueilPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            Loaded += (_, _) => AfficherSalles(GetFeaturedSalles());
        }

        private List<Salle> GetFeaturedSalles()
        {
            var amphi = GetRandomSalles(DataService.Salles.Where(s => s.TypeSalle == "Amphithéâtre"), 2);
            var cours = GetRandomSalles(DataService.Salles.Where(s => s.TypeSalle == "Salle de cours"), 4);
            var reunion = GetRandomSalles(DataService.Salles.Where(s => s.TypeSalle == "Salle de réunion"), 2);
            
            return amphi.Concat(cours).Concat(reunion).ToList();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == (string)tb.Tag)
            {
                tb.Text = "";
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0x1C, 0x20, 0x30));
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = (string)tb.Tag;
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
            }
        }

        private void BtnRechercher_Click(object sender, RoutedEventArgs e)
        {
            var resultats = DataService.Salles.AsEnumerable();

            if (int.TryParse(TxtPersonnes.Text, out int cap) && cap > 0)
                resultats = resultats.Where(s => s.Capacite >= cap);

            var type = (CmbType.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(type) && type != "Tous les types")
                resultats = resultats.Where(s => s.TypeSalle == type);

            AfficherSalles(resultats.ToList());
        }

        private void AfficherSalles(List<Salle> salles)
        {
            SallesPanel.Children.Clear();
            TxtResultats.Text = salles.Count == 0 ? "Aucun résultat" : $"{salles.Count} salle{(salles.Count > 1 ? "s" : "")}";

            foreach (var salle in salles)
            {
                var card = CreateSalleCard(salle);
                SallesPanel.Children.Add(card);
            }
        }

        private UIElement CreateSalleCard(Salle salle)
        {
            var outer = new Border
            {
                Width = 270, Margin = new Thickness(0, 0, 20, 20),
                Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x20, 0x30)),
                CornerRadius = new CornerRadius(14),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x25, 0x2D, 0x45)),
                Cursor = Cursors.Hand,
                Effect = new DropShadowEffect { ShadowDepth = 0, BlurRadius = 20, Opacity = 0.3, Color = Colors.Black }
            };

            var stack = new StackPanel();

            // Image
            var imgBorder = new Border { Height = 160, CornerRadius = new CornerRadius(14, 14, 0, 0), Background = new SolidColorBrush(Color.FromRgb(0x12, 0x18, 0x28)), ClipToBounds = true };
            var brush = salle.ImagePath != null ? ImageHelper.LoadImageBrush(salle.ImagePath) : null;
            if (brush != null) imgBorder.Background = brush;
            else
            {
                var placeholder = new Grid();
                var ell = new Ellipse { Width = 60, Height = 60, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                ell.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0x2D, 0x45));
                var icon = new TextBlock { Text = GetTypeIcon(salle.TypeSalle), FontSize = 28, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                placeholder.Children.Add(ell); placeholder.Children.Add(icon);
                imgBorder.Child = placeholder;
            }

            // Badge type sur l'image
            var imgGrid = new Grid();
            imgGrid.Children.Add(imgBorder);
            var badge = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GetTypeColor(salle.TypeSalle))),
                CornerRadius = new CornerRadius(6), Padding = new Thickness(8, 4, 8, 4),
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 10, 10)
            };
            badge.Child = new TextBlock { Text = salle.TypeSalle, FontSize = 10, FontWeight = FontWeights.SemiBold, Foreground = Brushes.White, FontFamily = new FontFamily("Segoe UI") };
            imgGrid.Children.Add(badge);
            stack.Children.Add(imgGrid);

            // Infos
            var info = new StackPanel { Margin = new Thickness(16, 14, 16, 16) };
            info.Children.Add(new TextBlock { Text = salle.Nom, FontSize = 15, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(0xEE, 0xF0, 0xF8)), FontFamily = new FontFamily("Segoe UI"), TextTrimming = TextTrimming.CharacterEllipsis });

            var capPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 6, 0, 0) };
            capPanel.Children.Add(new TextBlock { Text = "👥", FontSize = 12 });
            capPanel.Children.Add(new TextBlock { Text = $"  {salle.Capacite} personnes max", FontSize = 12, Foreground = new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)), FontFamily = new FontFamily("Segoe UI") });
            info.Children.Add(capPanel);

            // Équipements (badges)
            if (salle.Equipements.Count > 0)
            {
                var eqPanel = new WrapPanel { Margin = new Thickness(0, 10, 0, 0) };
                foreach (var eq in salle.Equipements.Take(3))
                {
                    var eb = new Border { Background = new SolidColorBrush(Color.FromRgb(0x14, 0x20, 0x45)), CornerRadius = new CornerRadius(5), Padding = new Thickness(7, 3, 7, 3), Margin = new Thickness(0, 0, 5, 5) };
                    eb.Child = new TextBlock { Text = eq, FontSize = 10, Foreground = new SolidColorBrush(Color.FromRgb(0x4F, 0x8E, 0xF7)), FontFamily = new FontFamily("Segoe UI") };
                    eqPanel.Children.Add(eb);
                }
                if (salle.Equipements.Count > 3)
                {
                    var plus = new Border { Background = new SolidColorBrush(Color.FromRgb(0x14, 0x20, 0x45)), CornerRadius = new CornerRadius(5), Padding = new Thickness(7, 3, 7, 3) };
                    plus.Child = new TextBlock { Text = $"+{salle.Equipements.Count - 3}", FontSize = 10, Foreground = new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)), FontFamily = new FontFamily("Segoe UI") };
                    eqPanel.Children.Add(plus);
                }
                info.Children.Add(eqPanel);
            }

            // Bouton voir
            var btn = new Button { Content = "Voir les détails →", Margin = new Thickness(0, 12, 0, 0), Style = (Style)Application.Current.FindResource("SecondaryButtonStyle"), HorizontalAlignment = HorizontalAlignment.Stretch };
            btn.Click += (s, e) => { _main.NavigateTo("DetailSalle", salle); _main.UpdateNavButtons(""); };
            info.Children.Add(btn);
            stack.Children.Add(info);
            outer.Child = stack;

            // Hover animation
            outer.MouseEnter += (s, e) => {
                var anim = new DoubleAnimation(1, 1.02, TimeSpan.FromMilliseconds(150));
                var st = new ScaleTransform(1, 1); outer.RenderTransform = st; outer.RenderTransformOrigin = new Point(0.5, 0.5);
                st.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
                st.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
                outer.BorderBrush = new SolidColorBrush(Color.FromRgb(0x4F, 0x8E, 0xF7));
            };
            outer.MouseLeave += (s, e) => {
                var anim = new DoubleAnimation(1.02, 1, TimeSpan.FromMilliseconds(150));
                if (outer.RenderTransform is ScaleTransform st) { st.BeginAnimation(ScaleTransform.ScaleXProperty, anim); st.BeginAnimation(ScaleTransform.ScaleYProperty, anim); }
                outer.BorderBrush = new SolidColorBrush(Color.FromRgb(0x25, 0x2D, 0x45));
            };

            return outer;
        }

        private static string GetTypeIcon(string type) => type switch
        {
            "Amphithéâtre" => "🏛️",
            "Salle de cours" => "📚",
            _ => "🤝"
        };

        private static string GetTypeColor(string type) => type switch
        {
            "Amphithéâtre" => "#2D6CE0",
            "Salle de cours" => "#1AAD5A",
            _ => "#7B40C8"
        };

        // Extension method for random shuffle
        private static List<Salle> GetRandomSalles(IEnumerable<Salle> all, int count)
        {
            var list = all.Take(count).ToList();
            var random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = random.Next(n--);
                var temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
            return list;
        }
    }
}
