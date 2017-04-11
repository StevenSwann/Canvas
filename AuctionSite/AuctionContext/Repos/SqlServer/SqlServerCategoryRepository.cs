using AuctionPOCOs;
using System.Collections.Generic;
using System.Linq;

namespace AuctionContext
{
    public class SqlServerCategoryRepository : ICategoryRepository
    {
        Context context;

        public SqlServerCategoryRepository(Context _context)
        {
            this.context = _context;
        }

        public List<ProductCategory> GetAllCategories()
        {
            return context.ProductCategories.ToList();
        }

        public ProductCategory GetCategoryById(int id)
        {
            var category = context.ProductCategories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return new ProductCategory();
            }
            return category;
        }

        public ProductCategory AddNewCategory(ProductCategory category)
        {
            context.ProductCategories.Add(category);
            context.SaveChanges();
            return category;
        }

        public bool CheckCategoryName(string name)
        {
            var category = context.ProductCategories.FirstOrDefault(c => c.ProductCategoryName == name);
            if (category == null)
            {
                return false;
            }
            return true;
        }
    }
}
