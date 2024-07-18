using Sofol_Utils.Utils;
using SofolApp.MVVM.Views.DetailRegister;

namespace SofolApp.MVVM.Views;

public partial class RegisterPage2 : ContentPage
{
    private bool _isEntryFocused;

    #region Methods

    #region Constructor

    public RegisterPage2()
    {
        InitializeComponent();
    }



    #endregion

    #region Validate Fields

    private bool _validatePlaceHolderColor()
    {
        bool IsValid = false;

        if (this.entName.TextColor == Colors.Red)
        {
            IsValid = true;
        }

        if (this.entMiddleName.TextColor  == Colors.Red)
        {
            IsValid = true;
        }


        if (this.entLastName.TextColor  == Colors.Red)
        {
            IsValid = true;
        }

        if (this.entRFC.TextColor == Colors.Red)
        {
            IsValid = true;
        }

        if (this.entCURP.TextColor == Colors.Red)
        {
            IsValid = true;
        }

        if (this.entEdad.TextColor == Colors.Red)
        {
            IsValid = true;
        }

        return IsValid;

    }



    #endregion

    #endregion

    private void btnNext_Clicked(object sender, EventArgs e)
    {
        //if (_validatePlaceHolderColor())
        //{
        //    DisplayAlert("Error", "Por favor, llena todos los campos", "Ok");
        //}

        Navigation.PushAsync(new ScanIDPage());
    }
}
