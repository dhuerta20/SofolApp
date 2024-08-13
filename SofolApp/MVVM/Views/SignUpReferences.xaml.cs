using SofolApp.MVVM.ViewModels;

namespace SofolApp.MVVM.Views
{
    public partial class SignUpReferences : ContentPage
    {
        private readonly SignUpReferencesVM _viewModel;

        public SignUpReferences(SignUpReferencesVM viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            AddInitialReferences();
        }

        private void AddInitialReferences()
        {
            for (int i = 0; i < Math.Min(3, _viewModel.MaxReferences); i++)
            {
                AddReferenceEntry();
            }
        }

        private void AddReferenceEntry()
        {
            if (_viewModel.ReferenceEmails.Count >= _viewModel.MaxReferences)
            {
                return; // No agregar más si ya se alcanzó el límite
            }

            int referenceNumber = ReferencesContainer.Children.Count + 1;
            string placeholder = $"Referencia {referenceNumber} (Email)";
            var entry = new Entry
            {
                Placeholder = placeholder,
                TextColor = Colors.Black,
                BackgroundColor = Colors.White,
                Keyboard = Keyboard.Email
            };

            int index = ReferencesContainer.Children.Count;
            entry.SetBinding(Entry.TextProperty, new Binding($"ReferenceEmails[{index}]"));

            entry.Completed += async (sender, e) =>
            {
                await _viewModel.ValidateReferenceCommand.ExecuteAsync(index);
            };

            ReferencesContainer.Children.Add(entry);
            _viewModel.ReferenceEmails.Add(""); // Agregar un string vacío a la colección
        }

        protected override bool OnBackButtonPressed()
        {
            _viewModel.ReturnToRegisterCommand.ExecuteAsync(null);
            return true;
        }
    }
}