using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuctionPOCOs
{
    public class Listing : BaseEntity
    {
        [Required, MaxLength(200)]
        public string ItemName { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public bool Removed { get; set; }
        [Required]
        public DateTime AuctionStartTime { get; set; }
        [Required]
        public DateTime AuctionEndTime { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required, MaxLength(2000)]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public virtual Shipping Shipping { get; set; }
        public int ShippingId { get; set; }
        [Required]
        public virtual Status Status { get; set; }
        public int StatusId { get; set; }
        [Required]
        public virtual ProductCategory ProductCategory { get; set; }
        public int ProductCategoryId { get; set; }
        [Required]
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual List<Bid> Bids { get; set; }
        public virtual List<Comment> Comments { get; set; }
    }
}