using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            //List<Product> products = _productRepository.GetAll();
            List<BestSellingProducts> rankedProducts = new List<BestSellingProducts>();
            //List<GroceryListItem> groceryItems = _groceriesRepository.GetAll().Where(p => p.ProductId == product.Id).ToList();

            foreach (GroceryListItem item in _groceriesRepository.GetAll())
            {
                if (rankedProducts.Find(p => p.Id == item.ProductId) == null)
                {
                    Product product = _productRepository.GetAll().FirstOrDefault(p => p.Id == item.ProductId);
                    rankedProducts.Add(new BestSellingProducts(
                        item.ProductId, product.Name, product.Stock, 0, 0));
                }
                rankedProducts.Find(p => p.Id == item.ProductId).NrOfSells += item.Amount;
            }
            List<int> NrsOfSells = new List<int>();
            foreach (BestSellingProducts product in rankedProducts)
            {
                NrsOfSells.Add(product.NrOfSells);
            }
            NrsOfSells.Sort();
            NrsOfSells.Reverse();

            for (int i = 0; i < NrsOfSells.Count && i < 5; i++)
            {
                rankedProducts.FirstOrDefault(p => p.nrOfSells == NrsOfSells[i]).Ranking = i +1;
            }

            for (int i = rankedProducts.Count; i > 0; i--)
            {
                if (rankedProducts[i -1].Ranking == 0) 
                { 
                    rankedProducts.RemoveAt(i -1); 
                }
            }
            return rankedProducts;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
