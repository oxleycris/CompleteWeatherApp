﻿using CompleteWeatherApp.Views;
using Xamarin.Forms;

namespace CompleteWeatherApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new CurrentWeatherPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
