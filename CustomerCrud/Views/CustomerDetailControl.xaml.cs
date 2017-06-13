using CustomerCrud.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CustomerCrud.Views
{
    public sealed partial class CustomerDetailControl : UserControl
    {
        public Customer MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as Customer; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static DependencyProperty MasterMenuItemProperty = DependencyProperty.Register(
            "MasterMenuItem",typeof(Customer),typeof(CustomerDetailControl),new PropertyMetadata(null));

        public CustomerDetailControl()
        {
            InitializeComponent();
        }
    }
}
