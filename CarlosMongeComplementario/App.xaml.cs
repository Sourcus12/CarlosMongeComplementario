using CarlosMongeComplementario.Services;
using Microsoft.Data.Sqlite;
using System.IO;
namespace CarlosMongeComplementario
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();

            MainPage = new NavigationPage(new Views.LOGIN());
        }

        protected override void OnStart()
        {
            // Inicializar la base de datos
            DatabaseService.InitializeDatabase();
        }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
   
}
