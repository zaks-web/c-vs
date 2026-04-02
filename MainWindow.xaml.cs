using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GantsPlace.Models;
using GantsPlace.Services;
using GantsPlace.Views;

namespace GantsPlace
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavLinks.Visibility = Visibility.Collapsed;
            ShowDemarrage();
        }

        // ── Navigation ──────────────────────────────────────────
        public void ShowDemarrage()
        {
            NavLinks.Visibility = Visibility.Collapsed;
            UpdateAuthUI();
            NavigateToPage(new DemarragePage(this));
        }

        public void ShowMainApp()
        {
            NavLinks.Visibility = Visibility.Visible;
            NavigateTo("Accueil");
            UpdateNavButtons("Accueil");
        }

        public void NavigateTo(string pageName, object? param = null)
        {
            Page? page = pageName switch
            {
                "Accueil"     => new AccueilPage(this),
                "Explorer"    => new ExplorerPage(this),
                "Historique"  => new HistoriquePage(this),
                "Contact"     => new ContactPage(this),
                "Login"       => new LoginPage(this),
                "Inscription" => new InscriptionPage(this),
                "Admin"       => new AdminPage(this),
                "DetailSalle" => param is Salle s ? new DetailSallePage(this, s) : null,
                _ => null
            };
            if (page != null)
                NavigateToPage(page);
        }

        private void NavigateToPage(Page page)
        {
            // Animation fade
            MainFrame.Opacity = 0;
            MainFrame.Navigate(page);

            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220));
            MainFrame.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        public void UpdateNavButtons(string activePage)
        {
            BtnAccueil.Style    = activePage == "Accueil"    ? (Style)FindResource("NavButtonActiveStyle") : (Style)FindResource("NavButtonStyle");
            BtnExplorer.Style   = activePage == "Explorer"   ? (Style)FindResource("NavButtonActiveStyle") : (Style)FindResource("NavButtonStyle");
            BtnHistorique.Style = activePage == "Historique" ? (Style)FindResource("NavButtonActiveStyle") : (Style)FindResource("NavButtonStyle");
            BtnContact.Style    = activePage == "Contact"    ? (Style)FindResource("NavButtonActiveStyle") : (Style)FindResource("NavButtonStyle");
            BtnAdmin.Style      = activePage == "Admin"      ? (Style)FindResource("NavButtonActiveStyle") : (Style)FindResource("NavButtonStyle");
        }

        public void UpdateAuthUI()
        {
            if (Session.EstConnecte)
            {
                TxtUserName.Text        = Session.UtilisateurConnecte?.NomComplet ?? "";
                PanelAuth.Visibility    = Visibility.Collapsed;
                PanelUser.Visibility    = Visibility.Visible;
                
                // Admin button only for gestionnaire
                if (Session.UtilisateurConnecte.Email == "gestion@gmail.com")
                    BtnAdmin.Visibility = Visibility.Visible;
                else
                    BtnAdmin.Visibility = Visibility.Collapsed;
            }
            else
            {
                PanelAuth.Visibility = Visibility.Visible;
                PanelUser.Visibility = Visibility.Collapsed;
                BtnAdmin.Visibility = Visibility.Collapsed;
            }
        }

        // Compatibilité ancienne API
        public void RefreshAuthButtons() => UpdateAuthUI();

        // ── Handlers ────────────────────────────────────────────
        private void BtnAccueil_Click(object sender, RoutedEventArgs e)    { NavigateTo("Accueil");    UpdateNavButtons("Accueil");    }
        private void BtnExplorer_Click(object sender, RoutedEventArgs e)   { NavigateTo("Explorer");   UpdateNavButtons("Explorer");   }
        private void BtnHistorique_Click(object sender, RoutedEventArgs e) { NavigateTo("Historique"); UpdateNavButtons("Historique"); }
        private void BtnAdmin_Click(object sender, RoutedEventArgs e)     { NavigateTo("Admin");      UpdateNavButtons("Admin");      }
        private void BtnContact_Click(object sender, RoutedEventArgs e)    { NavigateTo("Contact");    UpdateNavButtons("Contact");    }
        private void BtnConnexion_Click(object sender, RoutedEventArgs e)   { NavigateTo("Login");      UpdateNavButtons("");           }
        private void BtnInscription_Click(object sender, RoutedEventArgs e) { NavigateTo("Inscription");UpdateNavButtons("");           }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            Session.UtilisateurConnecte = null;
            ShowDemarrage();
        }
    }
}
