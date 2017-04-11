using AuctionPOCOs;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface ICategoryRepository
    {
        List<ProductCategory> GetAllCategories();
        ProductCategory GetCategoryById(int id);
        ProductCategory AddNewCategory(ProductCategory category);
        bool CheckCategoryName(string name);
    }
}
