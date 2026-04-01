using System.Windows;
using System.Windows.Controls;
using GantsPlace.Services;

namespace GantsPlace.Views
{
    public partial class LoginPage : Page
    {
        private readonly MainWindow _main;
        public LoginPage(MainWindow main) { InitializeComponent(); _main = main; }

        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            ErrBorder.Visibility = Visibility.Collapsed;
            var email = TxtEmail.Text.Trim();
            var mdp   = TxtMdp.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(mdp))
            { ShowError("Veuillez remplir tous les champs."); return; }

            if (DataService.Authentifier(email, mdp))
            {
                _main.UpdateAuthUI();
                _main.ShowMainApp();
            }
            else ShowError("Email ou mot de passe incorrect.");
        }

        private void BtnVersInscription_Click(object sender, RoutedEventArgs e)
            => _main.NavigateTo("Inscription");

        private void ShowError(string msg)
        {
            TxtErreur.Text = msg;
            ErrBorder.Visibility = Visibility.Visible;
        }
    }
}
