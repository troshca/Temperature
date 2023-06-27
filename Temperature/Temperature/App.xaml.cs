using Acr.UserDialogs;
using Temperature.Services.BluetoothService;
using Temperature.ViewModels;
using Temperature.Views;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using System;

namespace Temperature
{
    public partial class App
    {
        public App()
        {
        }

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var result =  await NavigationService.NavigateAsync("NavigationPage/MainPage");

            if (!result.Success)
            {
                SetMainPageFromException(result.Exception);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterInstance(typeof(IUserDialogs), UserDialogs.Instance);
            containerRegistry.Register<IBluetoothService, BluetoothService>();

            containerRegistry.RegisterForNavigation<ServiceListPage, ServiceListPageViewModel>();
            containerRegistry.RegisterForNavigation<TemperatureSensorPage, TemperatureSensorPageViewModel>();

        }

        private void SetMainPageFromException(Exception ex)
        {
            var layout = new StackLayout
            {
                Padding = new Thickness(40)
            };
            layout.Children.Add(new Label
            {
                Text = ex?.GetType()?.Name ?? "Unknown Error encountered",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Children.Add(new ScrollView
            {
                Content = new Label
                {
                    Text = $"{ex}",
                    LineBreakMode = LineBreakMode.WordWrap
                }
            });

            MainPage = new ContentPage
            {
                Content = layout
            };
        }
    }
}
