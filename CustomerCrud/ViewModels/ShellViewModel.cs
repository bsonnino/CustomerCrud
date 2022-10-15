using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using CustomerCrud.Helpers;
using CustomerCrud.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;

namespace CustomerCrud.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private const string PanoramicStateName = "PanoramicState";
        private const string WideStateName = "WideState";
        private const string NarrowStateName = "NarrowState";

        public NavigationServiceEx NavigationService => SimpleIoc.Default.GetInstance<NavigationServiceEx>();

        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => Set(ref _isPaneOpen, value);
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.CompactInline;
        public SplitViewDisplayMode DisplayMode
        {
            get => _displayMode;
            set => Set(ref _displayMode, value);
        }

        private object _lastSelectedItem;

        private ObservableCollection<ShellNavigationItem> _primaryItems = new ObservableCollection<ShellNavigationItem>();
        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get => _primaryItems;
            set => Set(ref _primaryItems, value);
        }

        private ObservableCollection<ShellNavigationItem> _secondaryItems = new ObservableCollection<ShellNavigationItem>();
        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get => _secondaryItems;
            set => Set(ref _secondaryItems, value);
        }

        private ICommand _openPaneCommand;
        public ICommand OpenPaneCommand => _openPaneCommand ?? (_openPaneCommand = new RelayCommand(() => IsPaneOpen = !_isPaneOpen));

        private ICommand _itemSelected;
        public ICommand ItemSelectedCommand => _itemSelected ?? (_itemSelected = new RelayCommand<ShellNavigationItem>(ItemSelected));

        private ICommand _stateChangedCommand;
        public ICommand StateChangedCommand =>
            _stateChangedCommand ?? (_stateChangedCommand =
                new RelayCommand<VisualStateChangedEventArgs>(OnStateChanged));

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            switch (args.NewState.Name)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    IsPaneOpen = false;
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    IsPaneOpen = false;
                    break;
            }
        }

        public void Initialize(Frame frame)
        {
            NavigationService.Frame = frame;
            NavigationService.Frame.Navigated += NavigationService_Navigated;
            PopulateNavItems();
        }

        private void PopulateNavItems()
        {
            _primaryItems.Clear();
            _secondaryItems.Clear();

            // More on Segoe UI Symbol icons: https://docs.microsoft.com/windows/uwp/style/segoe-ui-symbol-font
            // Edit String/en-US/Resources.resw: Add a menu item title for each page
            _primaryItems.Add(new ShellNavigationItem("Shell_Customer".GetLocalized(), Symbol.People, typeof(CustomerViewModel).FullName));
            _secondaryItems.Add(new ShellNavigationItem("Shell_Settings".GetLocalized(), Symbol.Setting, typeof(SettingsViewModel).FullName));
        }

        private void ItemSelected(ShellNavigationItem e)
        {
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
            {
                IsPaneOpen = false;
            }
            Navigate(e);
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            if (e != null)
            {
                var vm = NavigationService.GetNameOfRegisteredPage(e.SourcePageType);
                var item = PrimaryItems?.FirstOrDefault(i => i.ViewModelName == vm) ?? SecondaryItems?.FirstOrDefault(i => i.ViewModelName == vm);

                if (item == null) return;
                ChangeSelected(_lastSelectedItem, item);
                _lastSelectedItem = item;
            }
        }

        private void ChangeSelected(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                ((ShellNavigationItem) oldValue).IsSelected = false;
            }
            if (newValue != null)
            {
                ((ShellNavigationItem) newValue).IsSelected = true;
            }
        }

        private void Navigate(object item)
        {
            if (item is ShellNavigationItem navigationItem)
            {
                NavigationService.Navigate(navigationItem.ViewModelName);
            }
        }
    }
}
