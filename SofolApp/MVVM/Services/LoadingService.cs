using SofolApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.Services
{
    public class LoadingService : ILoadingService
    {
        private ContentPage _loadingPage;
        private bool _isShowing;

        public LoadingService()
        {
            _loadingPage = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    Color = Colors.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
        }

        public async Task ShowLoadingAsync()
        {
            if (!_isShowing)
            {
                _isShowing = true;
                await Application.Current.MainPage.Navigation.PushModalAsync(_loadingPage, false);
            }
        }

        public async Task HideLoadingAsync()
        {
            if (_isShowing)
            {
                _isShowing = false;
                await Application.Current.MainPage.Navigation.PopModalAsync(false);
            }
        }
    }
}