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
            List<BestSellingProducts> rankedProducts = new List<BestSellingProducts>();

            foreach (GroceryListItem item in _groceriesRepository.GetAll())
            {
                if (rankedProducts.Find(p => p.Id == item.ProductId) == null)
                {
                    Product product = _productRepository.GetAll().FirstOrDefault(p => p.Id == item.ProductId);
                    rankedProducts.Add(new BestSellingProducts(item.ProductId, product.Name, product.Stock, 0, 0));
                }
                rankedProducts.Find(p => p.Id == item.ProductId).NrOfSells += item.Amount;
            }

            List<int> NrsOfSells = new List<int>();
            foreach (BestSellingProducts product in rankedProducts)
            {
                NrsOfSells.Add(product.NrOfSells);
            }

            rankedProducts = RankProducts(rankedProducts, NrsOfSells, topX);
            rankedProducts = SortRankedProducts(rankedProducts);
            return rankedProducts;
        }

        public List<BestSellingProducts> SortRankedProducts(List<BestSellingProducts> rankedProducts)
        {
            List<BestSellingProducts> sortedProducts = new List<BestSellingProducts>();
            for (int i = 0; i < 5; i++)
            {
                sortedProducts.Add(rankedProducts.Find(p => p.Ranking == i));
            }
            return sortedProducts;
        }

        public List<BestSellingProducts> RankProducts(List<BestSellingProducts> rankedProducts, List<int> NrsOfSells, int topX)
        {
            NrsOfSells.Sort();
            NrsOfSells.Reverse();

            for (int i = 0; i < NrsOfSells.Count && i < topX; i++)
            {
                rankedProducts.FirstOrDefault(p => p.nrOfSells == NrsOfSells[i] && p.Ranking == 0).Ranking = i + 1;
            }

            for (int i = rankedProducts.Count; i > 0; i--)
            {
                if (rankedProducts[i - 1].Ranking == 0)
                {
                    rankedProducts.RemoveAt(i - 1);
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
