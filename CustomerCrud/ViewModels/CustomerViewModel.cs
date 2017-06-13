using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using CustomerCrud.Models;
using CustomerCrud.Services;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CustomerCrud.Helpers;

namespace CustomerCrud.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        public NavigationServiceEx NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            }
        }

        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        private VisualState _currentState;

        private Customer _selected;
        

        public ICommand ItemClickCommand { get; private set; }
        public ICommand StateChangedCommand { get; private set; }
        public ICommand AddCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand SaveCustomersCommand { get; }

        public CustomerViewModel()
        {
            ItemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
            AddCustomerCommand = new RelayCommand(DoAddCustomer);
            DeleteCustomerCommand = new RelayCommand(DoDeleteCustomer);
            SaveCustomersCommand = new RelayCommand(DoSaveCustomers);
        }

        

        private void DoDeleteCustomer()
        {
            if (Selected != null)
                Customers.Remove(Selected);
            Selected = Customers.FirstOrDefault();
        }

        private void DoAddCustomer()
        {
            var customer = new Customer();
            Customers.Add(customer);
            Selected = customer;
        }

        public Customer Selected
        {
            get { return _selected; }
            set
            {
                Set(ref _selected, value);
                SaveSettings();
            }
        }
        private async void DoSaveCustomers()
        {
            var customerService = new CustomerService();
            await customerService.SaveDataAsync(Customers);
            Singleton<ToastNotificationsService>.Instance.ShowToastNotificationSample($"Saved {Customers.Count} customers");
            Singleton<LiveTileService>.Instance.SampleUpdate($"{Customers.Count} customers in the database");
        }

        public async Task LoadDataAsync(VisualState currentState)
        {
            _currentState = currentState;
            Customers.Clear();

            var service = new CustomerService();
            var data = await service.GetDataAsync();

            foreach (var item in data)
            {
                Customers.Add(item);
            }
            await LoadSettingsAsync();
            
            Singleton<ToastNotificationsService>.Instance.ShowToastNotificationSample($"Loaded {Customers.Count} customers");
            Singleton<LiveTileService>.Instance.SampleUpdate($"{Customers.Count} customers in the database");
        }

        private async void SaveSettings()
        {
            if (Selected != null)
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                var container =
                    localSettings.CreateContainer("CustSettings",
                        Windows.Storage.ApplicationDataCreateDisposition.Always);
                await container.SaveAsync("LastCust", Selected.Id);
            }
        }

        private async Task LoadSettingsAsync()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var container =
                localSettings.CreateContainer("CustSettings",
                    Windows.Storage.ApplicationDataCreateDisposition.Always);
            var lastCust = await container.ReadAsync<string>("LastCust");
            Selected = !string.IsNullOrEmpty(lastCust) ? 
                Customers.FirstOrDefault(c => c.Id == lastCust) : 
                Customers.FirstOrDefault();
        }

        public ObservableCollection<Customer> Customers { get; private set; } = new ObservableCollection<Customer>();

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            _currentState = args.NewState;
        }

        private void OnItemClick(ItemClickEventArgs args)
        {
            Customer item = args?.ClickedItem as Customer;
            if (item != null)
            {
                if (_currentState.Name == NarrowStateName)
                {
                    NavigationService.Navigate(typeof(CustomerDetailViewModel).FullName, item);
                }
                else
                {
                    Selected = item;
                }
            }
        }
    }
}
