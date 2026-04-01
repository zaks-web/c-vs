using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GantsPlace.Services;

namespace GantsPlace.Views
{
    public partial class ContactPage : Page
    {
        private readonly MainWindow _main;
        private readonly string[] _membres = { "Ziad S.", "Mariam D.", "Honoré N.", "Brightson G.", "Emmanuel A." };
        private readonly string[] _couleurs = { "#4F8EF7", "#2ECC71", "#F39C12", "#E74C3C", "#9B59B6" };

        public ContactPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            Loaded += (_, _) => BuildTeam();
        }

        private void BuildTeam()
        {
            foreach (var m in _membres)
            {
                var idx = System.Array.IndexOf(_membres, m);
                var couleur = _couleurs[idx % _couleurs.Length];

                var container = new StackPanel { Margin = new Thickness(0, 0, 10, 0), Width = 52 };

                // Avatar circulaire
                var grid = new System.Windows.Controls.Grid { Width = 44, Height = 44, HorizontalAlignment = HorizontalAlignment.Center };

                var imgBrush = ImageHelper.LoadImageBrush($"membre{idx + 1}.jpg")
                            ?? ImageHelper.LoadImageBrush($"membre{idx + 1}.png");

                var ellipse = new System.Windows.Shapes.Ellipse
                {
                    Width = 44, Height = 44,
                    Fill = imgBrush != null
                        ? (Brush)imgBrush
                        : new SolidColorBrush((Color)ColorConverter.ConvertFromString(couleur))
                };
                grid.Children.Add(ellipse);

                if (imgBrush == null)
                {
                    var init = new TextBlock
                    {
                        Text = m.Length > 0 ? m[0].ToString() : "?",
                        FontSize = 14, FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    grid.Children.Add(init);
                }

                container.Children.Add(grid);
                container.Children.Add(new TextBlock
                {
                    Text = m.Split(' ')[0], FontSize = 10,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
                });
                TeamPanel.Children.Add(container);
            }
        }

        private void BtnEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNom.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                string.IsNullOrWhiteSpace(TxtMessage.Text))
            {
                ShowMsg("⚠️", "Veuillez remplir tous les champs obligatoires.", false);
                return;
            }

            if (!TxtEmail.Text.Contains("@"))
            {
                ShowMsg("⚠️", "L'adresse email semble invalide.", false);
                return;
            }

            var sujet = (CmbSujet.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            ShowMsg("✅", $"Votre message ({sujet}) a bien été envoyé ! Nous vous répondrons à {TxtEmail.Text} sous 24h.", true);

            TxtNom.Text = "";
            TxtEmail.Text = "";
            TxtMessage.Text = "";
        }

        private void ShowMsg(string icon, string text, bool success)
        {
            MsgIcon.Text = icon;
            TxtMsg.Text = text;

            if (success)
            {
                MsgBorder.Background   = new SolidColorBrush(Color.FromRgb(0x0D, 0x2A, 0x1A));
                MsgBorder.BorderBrush  = (Brush)Application.Current.FindResource("SuccessBrush");
                TxtMsg.Foreground      = (Brush)Application.Current.FindResource("SuccessBrush");
            }
            else
            {
                MsgBorder.Background   = new SolidColorBrush(Color.FromRgb(0x2A, 0x1A, 0x0D));
                MsgBorder.BorderBrush  = (Brush)Application.Current.FindResource("WarningBrush");
                TxtMsg.Foreground      = (Brush)Application.Current.FindResource("WarningBrush");
            }

            MsgBorder.BorderThickness  = new Thickness(1);
            MsgBorder.Visibility       = Visibility.Visible;
        }
    }
}
