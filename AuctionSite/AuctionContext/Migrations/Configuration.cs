namespace AuctionContext.Migrations
{
    using AuctionPOCOs;
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<AuctionContext.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(AuctionContext.Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            context.Statuses.AddOrUpdate(
                p => p.Id,
                new Status { Id = 1, StatusName = "Current" }
                );
            context.SaveChanges();

            context.Shippings.AddOrUpdate(
                p => p.Id,
                new Shipping { Id = 1, ShipMode = "Regular Mail" }
                );
            context.SaveChanges();

            context.ProductCategories.AddOrUpdate(
                p => p.Id,
                new ProductCategory { Id = 1, ProductCategoryName = "Decorative" }
                );
            context.SaveChanges();

            context.Users.AddOrUpdate(
                p => p.Id,
                new User { Id = 1, Username = "Blue Peter", EmailAddress = "bp@domain.com", Password = "admin", Address = "100 Bond Street" },
                new User { Id = 2, Username = "User 2", EmailAddress = "email@address.com", Password = "password", Address = "123 Fake Street" }
                );
            context.SaveChanges();

            context.Listings.AddOrUpdate(
                p => p.Id,
                new Listing { Id = 1, ItemName = "Stuff", Description = "Stuff I'm Selling", AuctionStartTime = DateTime.Now.Date, AuctionEndTime = DateTime.Now.Date, Price = 0, ProductCategoryId = 1, ShippingId = 1, UserId = 1, StatusId = 1, ImageUrl = "/notfound" }
                );
            context.SaveChanges();

            context.Bids.AddOrUpdate(
                p => p.Id,
                new Bid { Id = 1, ListingId = 1, UserId = 1, BidAmount = 0.0m }
                );
            context.SaveChanges();

            context.Comments.AddOrUpdate(
                p => p.Id,
                new Comment { Id = 1, Content = "You Suck McBain", UserId = 1, ListingId = 1 },
                new Comment { Id = 2, Content = "Why you do dis", UserId = 2, ListingId = 1 }
                );
            context.SaveChanges();
        }
    }
}