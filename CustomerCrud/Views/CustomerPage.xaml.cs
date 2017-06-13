using CustomerCrud.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CustomerCrud.Views
{
    public sealed partial class CustomerPage : Page
    {
        private CustomerViewModel ViewModel
        {
            get { return DataContext as CustomerViewModel; }
        }

        public CustomerPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.LoadDataAsync(WindowStates.CurrentState);
        }

    }
}
