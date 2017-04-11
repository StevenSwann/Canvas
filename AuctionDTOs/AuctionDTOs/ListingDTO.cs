using AuctionDTOs.Custom_Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDTOs
{
    public class ListingDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A title is required for the listing.")]        
        [Display(Name = "Listing Title")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters.")]
        public string ItemName { get; set; }
        [Required(ErrorMessage = "A quantity is required.")]
        [Range(1,99, ErrorMessage = "Please enter a value between 1 and 99.")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "A price is required.")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage="Invalid price.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "An image is required.")]
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "A description abstract is required.")]
        public string DescriptionAbstract { get; set; }
        [Required(ErrorMessage = "A product category is required.")]
        public ProductCategoryDTO ProductCategory { get; set; }
        public DateTime AuctionStartTime { get; set; }
        [Required(ErrorMessage = "An end time is required.")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Auction end time must be at least 1 day in the future.")]
        public DateTime AuctionEndTime { get; set; }
        public UserDTO User { get; set; }
        public List<BidDTO> Bids { get; set; }
    }

    public class ListingDetailDTO : ListingDTO
    {
        [Required(ErrorMessage = "A description is required.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public StatusDTO Status { get; set; }
        public ShippingDTO Shipping { get; set; }        
        public List<CommentDTO> Comments { get; set; }
    }
}
