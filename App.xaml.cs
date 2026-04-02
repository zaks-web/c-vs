using System;
using System.Windows;
using GantsPlace.Services;

namespace GantsPlace
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                MessageBox.Show($"Erreur fatale :\n{args.ExceptionObject}",
                                "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Erreur :\n{args.Exception.Message}\n\n{args.Exception.StackTrace}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            DataService.LoadSalles();
        }
    }
}