using AuctionContext;
using AuctionDTOs;
using AuctionPOCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace AuctionWebApi.Controllers
{
    //[RoutePrefix("api/listing")]
    public class ListingController : ApiController
    {
        Context dbcontext;
        Mapper mapper;
        IListingRepository listingRepo;
        ICategoryRepository productCategoryRepo;
        IBidRepository bidRepo;
        ICommentRepository commentRepo;

        public ListingController()
        {
            dbcontext = new Context();
            mapper = new Mapper(dbcontext);
            listingRepo = new SqlServerListingRepository(dbcontext);
            productCategoryRepo = new SqlServerCategoryRepository(dbcontext);
            bidRepo = new SqlServerBidRepository(dbcontext);
            commentRepo = new SqlServerCommentRepository(dbcontext);
        }

        //GET api/listing/getallproductcategories
        [HttpGet]
        public List<ProductCategoryDTO> GetAllProductCategories()
        {
            return productCategoryRepo.GetAllCategories().Select(c => mapper.CreateProductCategoryDTO(c)).ToList();
        }

        //GET api/listing/getalllistings
        [HttpGet]
        public List<ListingDTO> GetAllListings()
        {
            return listingRepo.GetAllListings().Select(l => mapper.CreateListingDTO(l)).ToList();
        }

        //GET api/listing/getlisting/1
        [ResponseType(typeof(ListingDetailDTO)), HttpGet]
        public IHttpActionResult GetListing(int id)
        {
            var listing = listingRepo.GetListingById(id);
            if (listing.Id == 0)
            {
                return NotFound();
            }
            return Ok(mapper.CreateListingDetailDTO(listing));
        }

        //POST api/listing/postlisting
        [HttpPost]
        public HttpResponseMessage PostListing(ListingDetailDTO listingDTO)
        {          
            Listing listToAdd = mapper.CreateListingEntity(listingDTO);
            listToAdd = listingRepo.AddListing(listToAdd);
            var response = Request.CreateResponse<ListingDetailDTO>(HttpStatusCode.Created, mapper.CreateListingDetailDTO(listToAdd));
            string uri = Url.Link("DefaultApi", new { id = listToAdd.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        //POST api/listing/bid
        [HttpPost]
        public HttpResponseMessage Bid(BidDTO bidDTO)
        {
            if (bidDTO.Listing.Bids.Count == 0)
            {

            }
            else
            {
                if (bidDTO.BidAmount <= bidDTO.Listing.Bids.Max(b => b.BidAmount))
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }
            
            Bid bidToAdd = mapper.CreateBidEntity(bidDTO);
            if (bidDTO.Listing.AuctionEndTime > System.DateTime.Now)
            {
                bidToAdd = bidRepo.BidOnListing(bidToAdd);
            }
            var response = Request.CreateResponse<ListingDetailDTO>(HttpStatusCode.Created, mapper.CreateListingDetailDTO(bidToAdd.Listing));
            string uri = Url.Link("DefaultApi", new { id = bidToAdd.ListingId });
            response.Headers.Location = new Uri(uri);
            return response;         
   
        }
        
        //POST api/listing/addcomment
        [HttpPost]
        public HttpResponseMessage AddComment(CommentDTO commentDTO)
        {
            Comment commentToAdd = mapper.CreateCommentEntity(commentDTO);
            commentToAdd = commentRepo.AddComment(commentToAdd);
            var response = Request.CreateResponse<ListingDetailDTO>(HttpStatusCode.Created, mapper.CreateListingDetailDTO(commentToAdd.Listing));
            string uri = Url.Link("DefaultApi", new { id = commentToAdd.ListingId });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        //POST api/listing/deletecomment
        [HttpPost]
        public HttpResponseMessage DeleteComment(CommentDTO commentDTO)
        {
            Comment commentToDelete = mapper.DeleteCommentEntity(commentDTO);
            commentRepo.DeleteComment(commentToDelete);
            var response = Request.CreateResponse<ListingDetailDTO>(HttpStatusCode.Created, mapper.CreateListingDetailDTO(commentToDelete.Listing));
            string uri = Url.Link("DefaultApi", new { id = commentToDelete.ListingId });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        //POST api/listing/editcomment
        [HttpPost]
        public HttpResponseMessage EditComment(CommentDTO commentDTO)
        {
            Comment commentToEdit = mapper.DeleteCommentEntity(commentDTO);
            commentRepo.EditComment(commentToEdit);
            var response = Request.CreateResponse<ListingDetailDTO>(HttpStatusCode.Created, mapper.CreateListingDetailDTO(commentToEdit.Listing));
            string uri = Url.Link("DefaultApi", new { id = commentToEdit.ListingId });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        //POST api/listing/removelisting
        [HttpPost]
        public HttpResponseMessage RemoveListing(ListingDTO listingDTO)
        {
            Listing listingToRemove = mapper.RemoveListingEntity(listingDTO);
            var response = Request.CreateResponse<ListingDetailDTO>(HttpStatusCode.Created, mapper.CreateListingDetailDTO(listingToRemove));
            string uri = Url.Link("DefaultApi", new { id = listingToRemove.Id });
            response.Headers.Location = new Uri(uri);
            listingRepo.RemoveListing(listingToRemove);
            return response;
        }

        //POST api/listing/editlisting
        [HttpPost]
        public HttpResponseMessage EditListing(ListingDetailDTO listingDTO)
        {
            Listing listingToEdit = mapper.EditListingEntity(listingDTO);
            listingRepo.EditListing(listingToEdit);
            var response = Request.CreateResponse<ListingDetailDTO>(HttpStatusCode.Created, mapper.CreateListingDetailDTO(listingToEdit));
            string uri = Url.Link("DefaultApi", new { id = listingToEdit.Id });
            response.Headers.Location = new Uri(uri);
            
            return response;
        }

        //PUT api/listing/getlistingbystring
        [HttpPut]
        public List<ListingDTO> GetListingsByString(SearchDTO searchDTO)
        {
            return listingRepo.GetListingsByString(searchDTO.SearchBox, searchDTO.Category).Select(l => mapper.CreateListingDTO(l)).ToList();
        }

        //PUT api/listing/getlistingbystringadvanced
        [HttpPut]
        public List<ListingDTO> GetListingsByStringAdvanced(AdvancedSearchDTO searchDTO)
        {
            return listingRepo.GetListingsByStringAdvanced(searchDTO.SearchBox, searchDTO.Category, searchDTO.MaximumPrice, searchDTO.MinimumPrice, searchDTO.AuctionEndTime).Select(l => mapper.CreateListingDTO(l)).ToList();
        }
    }
}
