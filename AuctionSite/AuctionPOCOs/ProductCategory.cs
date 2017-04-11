using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionPOCOs
{
    public class ProductCategory : BaseEntity
    {
        [Required, MaxLength(50), Index(IsUnique = true)]
        public string ProductCategoryName { get; set; }
    }
}