using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;


namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty]
        Product selectedProduct;
        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; } = [];
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<string> ProductNames { get; set; }

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
            ProductNames = new ObservableCollection<string>();
            foreach (Product p in Products)
            {
                ProductNames.Add(p.Name);
            }
        }

        partial void OnSelectedProductChanged(Product? oldValue, Product newValue)
        {
            BoughtProductsList.Clear();
            foreach (BoughtProducts p in _boughtProductsService.Get(newValue.Id).ToObservableCollection())
            {
                BoughtProductsList.Add(p);
            }
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
