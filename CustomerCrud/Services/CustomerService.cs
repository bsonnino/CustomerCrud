using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using CustomerCrud.Models;
using CustomerCrud.Helpers;

namespace CustomerCrud.Services
{
    public class CustomerService
    {
        public async Task<IEnumerable<Customer>> GetDataAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var customerData = await localFolder.ReadAsync<List<Customer>>("Customers");
            if (customerData == null)
            {
                customerData = await LoadInitialCustomerDataAsync();
                await localFolder.SaveAsync("Customers", customerData);
            }
            return customerData;
        }

        private static async Task<List<Customer>> LoadInitialCustomerDataAsync()
        {
            StorageFile customerFile = await StorageFile.GetFileFromApplicationUriAsync(
                new Uri("ms-appx:///Customers.json"));
            var customerJson = await FileIO.ReadTextAsync(customerFile);
            return await Json.ToObjectAsync<List<Customer>>(customerJson);
        }

        public async Task SaveDataAsync(IEnumerable<Customer> customerData)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.SaveAsync("Customers", customerData);
        }
    }
}
