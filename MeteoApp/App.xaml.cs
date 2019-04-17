using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MeteoApp
{
    public partial class App : Application
    {
        public static DataBase database;
        
        public static DataBase Database
        {
            get
            {
                if (database == null)
                    database = new DataBase();

                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            
            var nav = new NavigationPage(new MeteoListPage())
            {
                BarBackgroundColor = Color.FromRgba(0, 0, 0, 0.85),
                BarTextColor = Color.White
            };

            MainPage = nav;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}