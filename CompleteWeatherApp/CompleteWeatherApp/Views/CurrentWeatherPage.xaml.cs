
using CompleteWeatherApp.Helpers;
using CompleteWeatherApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompleteWeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        public CurrentWeatherPage()
        {
            InitializeComponent();

            GetCoordinatesAsync();
        }

        private string Location { get; set; } = "Cheltenham, United Kingdom";
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private async void GetCoordinatesAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    Location = await GetCityAsync(location);

                    GetWeatherInfoAsync();
                    GetForecastAsync();
                    GetBackgroundAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        private async Task<string> GetCityAsync(Location location)
        {
            var places = await Geocoding.GetPlacemarksAsync(location);
            var currentPlace = places?.First();

            if (currentPlace != null)
            {
                return $"{currentPlace.Locality}, {currentPlace.CountryName}";
            }

            return null;
        }

        private async void GetBackgroundAsync()
        {
            var url = "https://api.pexels.com/v1/search?query={Location}&per_page=15&page=1";

            var result = await ApiCaller.GetAsync(url, "563492ad6f917000010000011612d7af1e8a49b2be3ca0918be12dfc");

            if (result.Successful)
            {
                var bgInfo = JsonConvert.DeserializeObject<BackgroundInfo>(result.Response);

                if (bgInfo != null && bgInfo.photos.Length > 0)
                {
                    bgImg.Source = ImageSource.FromUri(new Uri(bgInfo.photos[new Random().Next(0, bgInfo.photos.Length - 1)].src.medium));
                }
            }
        }

        private async void GetWeatherInfoAsync()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?lat={Latitude}&lon={Longitude}&appid=2e75fcb3797a6df9ab16cccad6294431&units=metric";

            var result = await ApiCaller.GetAsync(url);

            if (result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);

                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0");
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                    windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                    // weather.dt is an int of seconds - the code below converts to a real date/time that we can use.
                    var dt = DateTime.Now;
                    dateTxt.Text = dt.ToString("dddd, MMM dd").ToUpper();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found", "OK");
            }
        }

        private async void GetForecastAsync()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?lat={Latitude}&lon={Longitude}&appid=2e75fcb3797a6df9ab16cccad6294431&units=metric";

            var result = await ApiCaller.GetAsync(url);

            if (result.Successful)
            {
                try
                {
                    var forecastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    var allList = new List<List>();

                    foreach (var list in forecastInfo.list)
                    {
                        //var date = DateTime.ParseExact(list.dt_txt, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                        {
                            allList.Add(list);
                        }
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }
    }
}
