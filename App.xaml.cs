using System.Windows;
using GantsPlace.Services;

namespace GantsPlace
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Initialise la BDD (création + données si première fois) puis charge les salles
            DataService.Init();
        }
    }
}
