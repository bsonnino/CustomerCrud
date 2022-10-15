using CommonServiceLocator;
using CustomerCrud.Services;
using CustomerCrud.Views;

using GalaSoft.MvvmLight.Ioc;

namespace CustomerCrud.ViewModels
{
    public class ViewModelLocator
    {
        readonly NavigationServiceEx _navigationService = new NavigationServiceEx();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => _navigationService);
            SimpleIoc.Default.Register<ShellViewModel>();
            Register<CustomerViewModel, CustomerPage>();
            Register<CustomerDetailViewModel, CustomerDetailPage>();
            Register<SettingsViewModel, SettingsPage>();
        }

        public SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public CustomerDetailViewModel CustomerDetailViewModel => ServiceLocator.Current.GetInstance<CustomerDetailViewModel>();

        public CustomerViewModel CustomerViewModel => ServiceLocator.Current.GetInstance<CustomerViewModel>();

        public ShellViewModel ShellViewModel => ServiceLocator.Current.GetInstance<ShellViewModel>();

        public void Register<VM, V>() where VM : class
        {
            SimpleIoc.Default.Register<VM>();
            _navigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
