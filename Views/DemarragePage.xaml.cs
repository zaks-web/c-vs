using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GantsPlace.Services;

namespace GantsPlace.Views
{
    public partial class DemarragePage : Page
    {
        private readonly MainWindow _main;
        private readonly string[] _membres = { "Ziad SANHONGOU", "Mariam DIALLO", "Honoré N'TSAKPE", "Brightson GNASSOUNOU-AKPA", "Emmanuel ADANDE" };
        private readonly string[] _couleurs = { "#4F8EF7", "#2ECC71", "#F39C12", "#E74C3C", "#9B59B6" };

        public DemarragePage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            BuildMembres();
        }

        private void BuildMembres()
        {
            for (int i = 0; i < _membres.Length; i++)
            {
                var nom = _membres[i];
                var couleur = _couleurs[i];
                var initials = GetInitials(nom);

                var card = new StackPanel { Width = 120, Margin = new Thickness(10, 0, 10, 0), HorizontalAlignment = HorizontalAlignment.Center };

                // Cercle avatar
                var grid = new Grid { Width = 88, Height = 88, HorizontalAlignment = HorizontalAlignment.Center };

                // Essayer de charger la photo
                var photoIndex = i + 1;
                var imgBrush = ImageHelper.LoadImageBrush($"membre{photoIndex}.jpg");
                if (imgBrush == null) imgBrush = ImageHelper.LoadImageBrush($"membre{photoIndex}.png");

                var ellipseBg = new Ellipse
                {
                    Width = 88, Height = 88,
                    Fill = imgBrush != null ? (Brush)imgBrush :
                           new SolidColorBrush((Color)ColorConverter.ConvertFromString(couleur)),
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = (Color)ColorConverter.ConvertFromString(couleur),
                        ShadowDepth = 0, BlurRadius = 16, Opacity = 0.4
                    }
                };
                grid.Children.Add(ellipseBg);

                // Initiales si pas de photo
                if (imgBrush == null)
                {
                    var txt = new TextBlock
                    {
                        Text = initials, FontSize = 22, FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    grid.Children.Add(txt);
                }

                card.Children.Add(grid);

                // Nom
                var nameTb = new TextBlock
                {
                    Text = GetShortName(nom), FontSize = 11, TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center, Foreground = new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)),
                    Margin = new Thickness(0, 10, 0, 0),
                    FontFamily = new FontFamily("Segoe UI")
                };
                card.Children.Add(nameTb);

                MembresPanel.Children.Add(card);
            }
        }

        private static string GetInitials(string name)
        {
            var parts = name.Split(' ');
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[1][0]}";
            return name.Length > 0 ? name[0].ToString() : "?";
        }

        private static string GetShortName(string name)
        {
            var parts = name.Split(' ');
            return parts.Length >= 2 ? $"{parts[0]}\n{parts[1]}" : name;
        }

        private void BtnEntrer_Click(object sender, RoutedEventArgs e)
        {
            _main.NavLinks.Visibility = Visibility.Visible;
            _main.NavigateTo("Accueil");
            _main.UpdateNavButtons("Accueil");
        }
    }
}
