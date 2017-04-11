using AuctionContext;
using AuctionDTOs;
using AuctionPOCOs;
using System.Linq;
using PasswordSecurity;
using System.Collections.Generic;

namespace AuctionWebApi
{
    public class Mapper
    {
        Context dbcontext;
        IListingRepository listingRepo;
        IBidRepository bidRepo;
        ICommentRepository commentRepo;
        IUserRepository userRepo;
        IShippingRepository shippingRepo;
        IStatusRepository statusRepo;
        ICategoryRepository categoryRepo;
        IAvatarRepository avatarRepo;

        public Mapper(Context _dbcontext)
        {
            this.dbcontext = _dbcontext;
            listingRepo = new SqlServerListingRepository(dbcontext);
            bidRepo = new SqlServerBidRepository(dbcontext);
            commentRepo = new SqlServerCommentRepository(dbcontext);
            userRepo = new SqlServerUserRepository(dbcontext);
            shippingRepo = new SqlServerShippingRepository(dbcontext);
            statusRepo = new SqlServerStatusRepository(dbcontext);
            categoryRepo = new SqlServerCategoryRepository(dbcontext);
            avatarRepo = new SqlServerAvatarRepository(dbcontext);
        }

        //LISTING MAPS

        public ProductCategoryDTO CreateProductCategoryDTO(ProductCategory productCategory)
        {
            return new ProductCategoryDTO()
            {
                Id = productCategory.Id,
                ProductCategoryName = productCategory.ProductCategoryName
            };
        }

        public ListingDTO CreateListingDTO(Listing listing)
        {                        
            return new ListingDTO()
            {
                Id = listing.Id,
                ItemName = listing.ItemName,
                Quantity = listing.Quantity,
                Price = listing.Price,
                AuctionStartTime = listing.AuctionStartTime,
                AuctionEndTime = listing.AuctionEndTime,
                ImageUrl = listing.ImageUrl,
                DescriptionAbstract = listing.Description, // TODO: Abstract is not Full Description, see end of this class
                ProductCategory = new ProductCategoryDTO()
                {
                    Id = listing.ProductCategory.Id,
                    ProductCategoryName = listing.ProductCategory.ProductCategoryName
                },
                Bids = bidRepo.GetAllBidsForListing(listing.Id).Select(b => new BidDTO()
                {
                    Id = b.Id,
                    BidAmount = b.BidAmount,
                    User = CreateUserDTO(b.User)
                }).ToList(),
                User = new UserDTO()
                {
                    Id = listing.User.Id,
                    Username = listing.User.Username
                }
            };
        }

        public ListingDetailDTO CreateListingDetailDTO(Listing listing)
        {
            return new ListingDetailDTO()
            {
                Id = listing.Id,
                ItemName = listing.ItemName,
                Quantity = listing.Quantity,
                Price = listing.Price,
                ImageUrl = listing.ImageUrl,
                Description = listing.Description,
                ProductCategory = new ProductCategoryDTO()
                {
                    Id = listing.ProductCategory.Id,
                    ProductCategoryName = listing.ProductCategory.ProductCategoryName
                },
                Status = new StatusDTO()
                {
                    Id = listing.Status.Id,
                    StatusName = listing.Status.StatusName,
                },
                Shipping = new ShippingDTO()
                {
                    Id = listing.Shipping.Id,
                    ShipMode = listing.Shipping.ShipMode
                },
                AuctionStartTime = listing.AuctionStartTime,
                AuctionEndTime = listing.AuctionEndTime,
                User = CreateUserDTO(listing.User),
                Bids = bidRepo.GetAllBidsForListing(listing.Id).Select(b => new BidDTO()
                {
                    Id = b.Id,
                    User = CreateUserDTO(b.User),
                    Listing = CreateListingDTO(listingRepo.GetListingById(b.ListingId)),
                    BidAmount = b.BidAmount
                }).ToList(),
                Comments = commentRepo.GetAllCommentsForListing(listing.Id).Select(c => new CommentDTO()
                {
                    Id = c.Id,
                    Content = c.Content,
                    Listing = CreateListingDTO(c.Listing),
                    User = CreateUserDTO(c.User)
                }).ToList(),
            };
        }

        public Listing RemoveListingEntity(ListingDTO listingDTO)
        {
            Listing listing = new Listing();
            listing = listingRepo.GetListingById(listingDTO.Id);
            return listing;
        }

        public Listing EditListingEntity(ListingDetailDTO listingDTO)
        {
            Listing listing = new Listing();
            listing = listingRepo.GetListingById(listingDTO.Id);
            listing.Price = listingDTO.Price;
            listing.ItemName = listingDTO.ItemName;
            listing.ImageUrl = listingDTO.ImageUrl;
            listing.ProductCategory = categoryRepo.GetCategoryById(listingDTO.ProductCategory.Id);
            listing.Description = listingDTO.Description;
            listing.AuctionEndTime = listingDTO.AuctionEndTime;
            listing.Quantity = listingDTO.Quantity;

            return listing;
        }

        public Listing CreateListingEntity(ListingDetailDTO listingDTO)
        {
            return new Listing()
            {
                Description = listingDTO.Description,
                ItemName = listingDTO.ItemName,
                Quantity = listingDTO.Quantity,
                ProductCategory = categoryRepo.GetCategoryById(listingDTO.ProductCategory.Id),
                Price = listingDTO.Price,
                ImageUrl = listingDTO.ImageUrl,
                Status = statusRepo.GetStatusById(listingDTO.Status.Id),
                Shipping = shippingRepo.GetShippingStatusById(listingDTO.Shipping.Id),
                AuctionStartTime = listingDTO.AuctionStartTime,
                AuctionEndTime = listingDTO.AuctionEndTime,
                User = userRepo.GetUserById(listingDTO.User.Id)
            };
        }

        //USER MAPS

        public AvatarImageDTO CreateAvatarDTO(AvatarImage avatar)
        {
            return new AvatarImageDTO()
            {
                Id = avatar.Id,
                AvatarImageBytes = avatar.AvatarImageBytes
            };
        }

        public AvatarImageIdDTO CreateAvatarIdDTO(AvatarImage avatar)
        {
            return new AvatarImageIdDTO()
            {
                Id = avatar.Id
            };
        }

        public UserDTO CreateUserDTO(User user)
        {
            UserDTO userDTO = new UserDTO()
            {
                Id = user.Id,
                Username = user.Username,
                AvatarImage = avatarRepo.GetAvatarsForUser(user.Id).Select(a => CreateAvatarIdDTO(a)).FirstOrDefault()
            };

            if (userDTO.AvatarImage == null)
            {
                userDTO.AvatarImage = new AvatarImageIdDTO() { Id = 0 };
            }

            return userDTO;
        }

        public UserDetailDTO CreateUserDetailDTO(User user)
        {
            return new UserDetailDTO()
            {
                Id = user.Id,
                Username = user.Username,
                EmailAddress = user.EmailAddress,
                Address = user.Address,
                AvatarImage = avatarRepo.GetAvatarsForUser(user.Id).Select(a => CreateAvatarIdDTO(a)).FirstOrDefault(),
                Listings = listingRepo.GetAllListingsForUser(user.Id).Select(l => CreateListingDTO(l)).ToList(),
                Bids = bidRepo.GetAllBidsForUser(user.Id).Select(b => new BidDTO
                {
                    Id = b.Id,
                    Dismissed = b.Dismissed,
                    BidAmount = b.BidAmount,
                    User = CreateUserDTO(b.User),
                    Listing = CreateListingDTO(listingRepo.GetListingById(b.ListingId))
                }).ToList(),
            };
        }

        public AuthenticatedUserDTO CreateAuthenticatedUserDTO(User user)
        {
            return new AuthenticatedUserDTO()
            {
                Id = user.Id,
                Username = user.Username,
                EmailAddress = user.EmailAddress,                
                Bids = bidRepo.GetAllBidsForUser(user.Id).Select(b => new BidDTO
                {
                    Id = b.Id,
                    Dismissed = b.Dismissed,
                    BidAmount = b.BidAmount,
                    User = CreateUserDTO(b.User),
                    Listing = CreateListingDTO(listingRepo.GetListingById(b.ListingId))
                }).ToList(),
                Listings = listingRepo.GetAllListingsForUser(user.Id).Where(l => l.User.Id == user.Id).Select(l => CreateListingDTO(l)).ToList(),
            };
        }

        public User CreateUserRegisterEntity(UserRegisterDTO userDTO)
        {
            return new User()
            {
                Username = userDTO.Username,
                Address = userDTO.Address,
                EmailAddress = userDTO.EmailAddress,
                Password = PasswordStorage.CreateHash(userDTO.Password)
            };
        }

        public User CreateUserLoginEntity(UserLoginDTO userDTO)
        {
            return new User()
            {
                EmailAddress = userDTO.EmailAddress,
                Password = userDTO.Password
            };
        }

        public User CreateUserEditEntity(UserEditDTO userDTO)
        {
            List<AvatarImage> avatars = new List<AvatarImage>();

            if (userDTO.AvatarImage != null)
            {
                avatars.Add(CreateAvatarEntity(userDTO.AvatarImage));
                return new User()
                {
                    Id = userDTO.Id,
                    EmailAddress = userDTO.EmailAddress,
                    Address = userDTO.Address,
                    AvatarImage = avatars
                };
            }
            else
            {                
                return new User()
                {
                    Id = userDTO.Id,
                    EmailAddress = userDTO.EmailAddress,
                    Address = userDTO.Address
                };
            }


        }

        public User CreateUserEditPasswordEntity(UserEditPasswordDTO userDTO) 
        {
            return new User()
            {
                Id = userDTO.Id,                
                Password = userDTO.NewPassword,
                Username = userDTO.OldPassword //Username holds old password.
            };
        }

        public User CreateUserAuthenticateEntity(AuthenticatedUserDTO userDTO)
        {
            return new User()
            {
                Id = userDTO.Id,
                //AuthToken = user.AuthToken
            };
        }

        public Bid CreateBidEntity(BidDTO bidDTO)
        {
            return new Bid()
            {
                Id = bidDTO.Id,
                Dismissed = bidDTO.Dismissed,
                BidAmount = bidDTO.BidAmount,
                Listing = listingRepo.GetListingById(bidDTO.Listing.Id),
                User = userRepo.GetUserById(bidDTO.User.Id)                
            };
        }

        public List<Bid> CreateBidEditEntities(BidDTO bidDTO)
        {
            List<Bid> bids = bidRepo.GetAllBidsForListing(bidDTO.Listing.Id).Where(b => b.UserId == bidDTO.User.Id).ToList();
            return bids;
        }

        public Comment CreateCommentEntity(CommentDTO commentDTO)
        {
            return new Comment()
            {
                Content = commentDTO.Content,
                Listing = listingRepo.GetListingById(commentDTO.Listing.Id),
                User = userRepo.GetUserById(commentDTO.User.Id)
            };
        }

        public Comment DeleteCommentEntity(CommentDTO commentDTO)
        {
            return new Comment()
            {
                Id = commentDTO.Id,
                Content = commentDTO.Content,
                Listing = listingRepo.GetListingById(commentDTO.Listing.Id),
                User = userRepo.GetUserById(commentDTO.User.Id)
            };
        }

        public AvatarImage CreateAvatarEntity(AvatarImageDTO avatarDTO)
        {
            return new AvatarImage()
            {                
                AvatarImageBytes = avatarDTO.AvatarImageBytes,
                User = userRepo.GetUserById(avatarDTO.User.Id)                
            };
        }
    }
}