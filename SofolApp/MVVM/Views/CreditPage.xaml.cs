using Microsoft.Maui.Controls;
using SofolApp.MVVM.ViewModels;
using System;
using Sentry;

namespace SofolApp.MVVM.Views
{
    public partial class CreditPage : ContentPage
    {
        public CreditPage(CreditPageVM viewModel)
        {
            try
            {
                InitializeComponent();
                BindingContext = viewModel;
                SentrySdk.AddBreadcrumb("CreditPage initialized", "info");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la inicialización de CreditPage: {ex}");
                SentrySdk.CaptureException(ex);
                // Aquí puedes agregar más logging si es necesario
            }
        }
    }
}