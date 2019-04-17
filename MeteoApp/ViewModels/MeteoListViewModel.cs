using System.Collections.ObjectModel;

namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        static ObservableCollection<Weather> _weathers;

        public ObservableCollection<Weather> Weathers
        {
            get { return _weathers; }
            set
            {
                _weathers = value;
                OnPropertyChanged();
            }
        }

        public MeteoListViewModel()
        {
            Weathers = new ObservableCollection<Weather>();
        }

        public void UpdateWeather(Weather weather) 
        {
            for (int i = 0; i < _weathers.Count; i++) 
            {
                if (_weathers[i].Location.ID == weather.Location.ID) 
                {
                    _weathers[i]= weather;
                    return;
                }
            }
        }
    }
}