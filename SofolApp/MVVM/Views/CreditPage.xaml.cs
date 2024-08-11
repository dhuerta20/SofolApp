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
                Console.WriteLine($"Error en la inicializaci�n de CreditPage: {ex}");
                SentrySdk.CaptureException(ex);
                // Aqu� puedes agregar m�s logging si es necesario
            }
        }
    }
}