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
            for (int i = 0; i < 3; i++)
            {
                AddReferenceEntry();
            }
        }

        private void AddReferenceEntry()
        {
            int referenceNumber = ReferencesContainer.Children.Count + 1;
            string placeholder = $"Referencia {referenceNumber} (Email)";

            var entry = new Entry
            {
                Placeholder = placeholder,
                TextColor = Colors.Black,
                BackgroundColor = Colors.White,
                Keyboard = Keyboard.Email
            };

            entry.SetBinding(Entry.TextProperty, new Binding($"ReferenceEmails[{ReferencesContainer.Children.Count}]"));

            entry.Completed += async (sender, e) =>
            {
                int index = ReferencesContainer.Children.IndexOf(entry);
                await _viewModel.ValidateReferenceCommand.ExecuteAsync(index);
            };

            ReferencesContainer.Children.Add(entry);
        }

        protected override bool OnBackButtonPressed()
        {
            _viewModel.ReturnToRegisterCommand.ExecuteAsync(null);
            return true;
        }
    }
}