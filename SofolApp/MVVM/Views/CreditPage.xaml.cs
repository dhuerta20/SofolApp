using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;
using System;
using Sentry;

namespace SofolApp.MVVM.Views
{
    public partial class CreditPage : ContentPage
    {
        private readonly CreditPageVM _viewModel;

        public CreditPage(CreditPageVM viewModel)
        {
            try
            {
                InitializeComponent();
                BindingContext = viewModel;
                _viewModel = viewModel;
                SentrySdk.AddBreadcrumb("CreditPage initialized", "info");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la inicialización de CreditPage: {ex}");
                SentrySdk.CaptureException(ex);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () => await _viewModel.LoadUserData());
        }
    }
}