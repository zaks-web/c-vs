using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GantsPlace.Services;

namespace GantsPlace.Views
{
    public partial class InscriptionPage : Page
    {
        private readonly MainWindow _main;
        public InscriptionPage(MainWindow main) { InitializeComponent(); _main = main; }

        private void BtnInscrire_Click(object sender, RoutedEventArgs e)
        {
            var nom   = TxtNom.Text.Trim();
            var email = TxtEmail.Text.Trim();
            var mdp   = TxtMdp.Password;
            var mdp2  = TxtMdp2.Password;

            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(mdp))
            { ShowMsg("Veuillez remplir tous les champs.", false); return; }
            if (mdp.Length < 6) { ShowMsg("Le mot de passe doit contenir au moins 6 caractères.", false); return; }
            if (mdp != mdp2) { ShowMsg("Les mots de passe ne correspondent pas.", false); return; }

            if (DataService.Inscrire(nom, email, mdp))
            { ShowMsg($"Bienvenue {nom} ! Votre compte a été créé. Vous pouvez vous connecter.", true); }
            else ShowMsg("Cet email est déjà utilisé.", false);
        }

        private void BtnVersLogin_Click(object sender, RoutedEventArgs e) => _main.NavigateTo("Login");

        private void ShowMsg(string msg, bool success)
        {
            TxtMsg.Text = msg;
            MsgBorder.Background = success ? new SolidColorBrush(Color.FromRgb(0x0D, 0x2A, 0x1A)) : new SolidColorBrush(Color.FromRgb(0x2A, 0x0D, 0x0D));
            MsgBorder.BorderBrush = success ? (System.Windows.Media.Brush)Application.Current.FindResource("SuccessBrush") : (System.Windows.Media.Brush)Application.Current.FindResource("DangerBrush");
            MsgBorder.BorderThickness = new Thickness(1);
            TxtMsg.Foreground = success ? (System.Windows.Media.Brush)Application.Current.FindResource("SuccessBrush") : (System.Windows.Media.Brush)Application.Current.FindResource("DangerBrush");
            MsgBorder.Visibility = Visibility.Visible;
        }
    }
}
