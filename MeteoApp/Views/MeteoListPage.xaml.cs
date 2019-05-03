using System;
using System.Collections;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace MeteoApp
{
    public partial class MeteoListPage : ContentPage
    {
        // Locations array
        public static ArrayList weathers = null;

        // API KEY
        private const string APIKey = "815504bb440299e3ebbb76868cbc7c47";

        // Weather URL
        private const string WeatherURL = "https://api.openweathermap.org/data/2.5/weather";

        // Icon URL
        private const string APIIcon = "https://openweathermap.org/img/w/";

        public MeteoListPage()
        {
            InitializeComponent();

            //Binding between this class and MeteoListViewModel
            BindingContext = new MeteoListViewModel();
        }

        // Start when view is loaded
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Debug delle risorse
            var assembly = typeof(MeteoListPage).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                Console.WriteLine("RESOURCES: " + res);

            // If DB is not initialized
            if (weathers == null)
            {
                // Init the DB and load it.
                var locations = App.Database.GetItemsAsync().Result.ToArray();

                // Init array list of weathers
                weathers = new ArrayList();

                // Get current location
                GetLocation();

                //Update vie
                LoadView(locations);

                // Start thread to update the temperature
                ThreadPool.QueueUserWorkItem(update => CheckTemperature());
            }
        }

        // Get the location from GPS
        private async void GetLocation()
        {

            var locator = CrossGeolocator.Current; // singleton

            // If geo location is not enable
            if (!locator.IsGeolocationEnabled)
                return;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(30));

            // Get the response from API
            string response = await GetWeatherCoordinateAsync(position.Longitude, position.Latitude);

            Location currentLocation = new Location
            {
                Name = JSonParserName(response),
                Lat = position.Latitude,
                Long = position.Longitude
            };

            Weather currentWeather = new Weather
            {
                Location = currentLocation,
                Temp = JSonParserTemp(response),
                IconURL = APIIcon + JSonParserIcon(response) + ".png"
            };

            // Load icon from API
            currentWeather.LoadIcon();

            // Add weather to array list
            weathers.Add(currentWeather);

            // Add the location in view
            ((MeteoListViewModel)BindingContext).Weathers.Insert(0, currentWeather);

            Console.WriteLine("Position Status: {0}", position.Timestamp);
            Console.WriteLine("Position Latitude: {0}", position.Latitude);
            Console.WriteLine("Position Longitude: {0}", position.Longitude);
        }

        // Update view with DB content
        private async void LoadView(Array locations)
        {
            // Load on view the location
            foreach (Location location in locations)
            {
                Weather weather = new Weather();

                // Get the respose from API
                string response = await GetWeatherCityNameAsync(location);

                // Update Temperature
                weather.Temp = JSonParserTemp(response);

                // Add location
                weather.Location = location;

                // Add icon url
                weather.IconURL = APIIcon + JSonParserIcon(response) + ".png";

                // Load icon from API
                weather.LoadIcon();

                // Add weather to array list
                weathers.Add(weather);

                // Add the location in view
                ((MeteoListViewModel)BindingContext).Weathers.Add(weather);
            }
        }

        // Update the temperature
        private async void CheckTemperature()
        {
            while (true)
            {
                // Update it every 5 minutes
                Thread.Sleep(300000);

                foreach (Weather weather in weathers)
                {
                    // Get the respose from API
                    string respose = await GetWeatherCityNameAsync(weather.Location);

                    // Update Temperature
                    weather.Temp = JSonParserTemp(respose);

                    ((MeteoListViewModel)BindingContext).UpdateWeather(weather);

                    Console.WriteLine("Temp changed: " + weather.Temp);
                }
            }
        }

        // Add button was pressed
        async void OnItemAdded(object sender, EventArgs e)
        {
            // Create and start dialog
            var result = await UserDialogs.Instance.PromptAsync(new PromptConfig
            {
                InputType = InputType.Name,
                OkText = "Add",
                CancelText = "Cancel",
                Title = "Location",
            });

            // If the result is Ok and is not emptly
            if (result.Ok && !string.IsNullOrWhiteSpace(result.Text))
            {
                // Generate a new location
                Random random = new Random();
                Location location = new Location
                {
                    ID = random.Next(),
                    Name = result.Value
                };
                // URL REQUEST
                String response = await GetWeatherCityNameAsync(location);

                // If the city exists
                if (response != null)
                {
                    Weather weather = new Weather
                    {

                        // add temperature
                        Temp = JSonParserTemp(response),

                        // Add icon url
                        IconURL = APIIcon + JSonParserIcon(response) + ".png"
                    };

                    // Load icon from API
                    weather.LoadIcon();

                    // Add location
                    weather.Location = location;

                    // Add the location in view
                    ((MeteoListViewModel)BindingContext).Weathers.Add(weather);

                    // Save in DB
                    _ = App.database.InsertItemAsync(location);
                }
            }
        }

        // return the temperature
        double JSonParserTemp(String content)
        {
            // See responses https://api.openweathermap.org/data/2.5/weather?q=Sementina&appid=815504bb440299e3ebbb76868cbc7c47
            JObject weatherObj = JObject.Parse(content);

            // get temp
            return ((Double)weatherObj["main"]["temp"]) - 273.15;
        }

        // return the icon URL
        string JSonParserIcon(String content)
        {
            // See responses https://api.openweathermap.org/data/2.5/weather?q=Sementina&appid=815504bb440299e3ebbb76868cbc7c47
            JObject weatherObj = JObject.Parse(content);

            // Get the icon of the weather
            return (string)(weatherObj["weather"][0]["icon"]);
        }

        //Return the name of city by the coordinate
        string JSonParserName(string content)
        {
            // See responses https://api.openweathermap.org/data/2.5/weather?q=Sementina&appid=815504bb440299e3ebbb76868cbc7c47
            JObject weatherObj = JObject.Parse(content);

            // Get the name of location
            return (string)(weatherObj["name"]);
        }

        // Show weather view
        void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Navigation.PushAsync(new MeteoItemPage()
                {
                    // MVVM
                    BindingContext = new MeteoItemViewModel(e.SelectedItem as Weather)
                });
            };
        }

        // Get weather from city name
        private Task<string> GetWeatherCityNameAsync(Location location)
        {
            HttpClient httpClient = new HttpClient();

            string requestUrl = WeatherURL
                               + "?q=" + location.Name
                               + "&appid=" + APIKey;
            // Get response
            return httpClient.GetStringAsync(requestUrl);
        }

        // Get weather from coordiante
        private async Task<string> GetWeatherCoordinateAsync(double lon, double lat)
        {
            HttpClient httpClient = new HttpClient();

            string requestUrl = "https://api.openweathermap.org/data/2.5/weather"
                               + "?lon=" + lon
                               + "&lat=" + lat
                               + "&appid=" + APIKey;
            // Get response
            return await httpClient.GetStringAsync(requestUrl);
        }
    }
}