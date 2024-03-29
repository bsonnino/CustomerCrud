using System.Windows.Input;

using CustomerCrud.Models;
using CustomerCrud.Services;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Ioc;

namespace CustomerCrud.ViewModels
{
    public class CustomerDetailViewModel : ViewModelBase
    {
        public NavigationServiceEx NavigationService => SimpleIoc.Default.GetInstance<NavigationServiceEx>();
        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        public ICommand StateChangedCommand { get; private set; }

        private Customer _item;
        public Customer Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        public CustomerDetailViewModel()
        {
            StateChangedCommand = new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }
        
        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            if (args.OldState.Name == NarrowStateName && args.NewState.Name == WideStateName)
            {
                NavigationService.GoBack();
            }
        }
    }
}
