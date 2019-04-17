namespace MeteoApp
{
    public class MeteoItemViewModel : BaseViewModel
    {
        Weather _weather;

        public Weather Weather
        {
            get { return _weather; }
            set
            {
                _weather = value;
                OnPropertyChanged();
            }
        }

        public MeteoItemViewModel(Weather weather)
        {
            Weather = weather;
        }
    }
}