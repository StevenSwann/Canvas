using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AuctionDTOs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Text;
using System.Security.Claims;
using System.Threading;
using PagedList;

namespace CanvasMVC.Controllers
{
    public class ListingController : Controller
    {
        string apiUrl = "http://localhost:58999/API/Listing";
        decimal bidIncrement = 0.50M;
        HttpClient client;

        public ListingController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        
        }        

        public async Task<ActionResult> Index(SearchDTO searchDTO, int? page)
        {
            if (searchDTO.SearchBox == "" || searchDTO.SearchBox == null)
            {
                HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetAllListings");

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    List<ListingDTO> listingList = JsonConvert.DeserializeObject<List<ListingDTO>>(responseData);

                    int pageSize = 5;
                    int pageNumber = (page ?? 1);

                    return View(listingList.OrderBy(l => l.AuctionEndTime).ToPagedList(pageNumber, pageSize));
                }

                return View("Error");
            }
            else
            {
                TempData["SearchString"] = searchDTO.SearchBox;                

                string content = JsonConvert.SerializeObject(searchDTO);
                Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                ByteArrayContent byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage responseMessage = await client.PutAsync(apiUrl + "/GetListingsByString", byteContent);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    List<ListingDTO> listingList = JsonConvert.DeserializeObject<List<ListingDTO>>(responseData);

                    int pageSize = 5;
                    int pageNumber = (page ?? 1);

                    return View(listingList.OrderByDescending(l => l.AuctionEndTime).ToPagedList(pageNumber, pageSize));
                }

                return View("Error");
            }            
        }

        public async Task<ActionResult> AdvancedSearch(AdvancedSearchDTO searchDTO, int? page)
        {
            ViewBag.SearchBox = searchDTO.SearchBox;
            ViewBag.Category = searchDTO.Category;
            ViewBag.MinimumPrice = searchDTO.MinimumPrice;
            ViewBag.MaximumPrice = searchDTO.MaximumPrice;
            ViewBag.AuctionEndTime = searchDTO.AuctionEndTime;

            string content = JsonConvert.SerializeObject(searchDTO);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage responseMessage = await client.PutAsync(apiUrl + "/GetListingsByStringAdvanced", byteContent);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                List<ListingDTO> listingList = JsonConvert.DeserializeObject<List<ListingDTO>>(responseData);

                int pageSize = 5;
                int pageNumber = (page ?? 1);

                return View(listingList.OrderByDescending(l => l.AuctionEndTime).ToPagedList(pageNumber, pageSize));
            }

            return View("Error");
        }

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + id);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

                if (listing.Bids.Count > 0)
                {
                    ViewBag.BidAmount = (listing.Bids.Max(bid => bid.BidAmount)) + bidIncrement;
                }
                else
                {
                    ViewBag.BidAmount = (listing.Price) + bidIncrement;
                }

                ViewBag.Listing = listing;

                return View(listing);

            }
            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Bid(BidDTO bidDTO)
        {          
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + bidDTO.Listing.Id);
            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
            ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

            if (listing.Bids.Count > 0)
            {
                if (bidDTO.BidAmount < (listing.Bids.Max(b => b.BidAmount) + 0.50M))
                {
                    TempData["BidError"] = "true";
                    return RedirectToAction("Detail", "Listing", new { id = bidDTO.Listing.Id });
                }
            }
            else
            {
                if (bidDTO.BidAmount < (listing.Price + 0.50M))
                {
                    TempData["BidError"] = "true";
                    return RedirectToAction("Detail", "Listing", new { id = bidDTO.Listing.Id });
                }
            }

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            responseMessage = await client.GetAsync("http://localhost:58999/API/user/GetUser/" + int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
            responseData = responseMessage.Content.ReadAsStringAsync().Result;
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(responseData);

            BidDTO newBid = new BidDTO();
            newBid.BidAmount = bidDTO.BidAmount;           
            newBid.User = user;
            newBid.Listing = listing;

            string content = JsonConvert.SerializeObject(newBid);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            responseMessage = await client.PostAsync(apiUrl + "/bid", byteContent);

            if (responseMessage.StatusCode == HttpStatusCode.Created)
            {
                return RedirectToAction("Detail", "Listing", new { id = bidDTO.Listing.Id });
            }

            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddComment(int listingId, string comment)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + listingId);
            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
            ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            responseMessage = await client.GetAsync("http://localhost:58999/API/user/GetUser/" + int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
            responseData = responseMessage.Content.ReadAsStringAsync().Result;
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(responseData);

            CommentDTO newComment = new CommentDTO();

            newComment.Content = comment;
            newComment.Listing = listing;
            newComment.User = user;

            string content = JsonConvert.SerializeObject(newComment);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await client.PostAsync(apiUrl + "/AddComment", byteContent);

            return RedirectToAction("Detail", "Listing", new { id = listingId });
        }

        [ChildActionOnly]
        public PartialViewResult _Bid(BidDTO bidDTO)
        {
            return PartialView("_Bid", bidDTO);
        }

        [ChildActionOnly]
        public PartialViewResult _AdvancedSearch()
        {
            HttpResponseMessage responseMessage = client.GetAsync(apiUrl + "/GetAllProductCategories").Result;

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);

                ViewData["Categories"] = categoryList;

                return PartialView("_AdvancedSearch", categoryList);
            }

            return PartialView("Error");
        }

        [ChildActionOnly]
        public PartialViewResult _SearchBar()
        {
            HttpResponseMessage responseMessage = client.GetAsync(apiUrl + "/GetAllProductCategories").Result;

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);

                ViewData["Categories"] = categoryList;

                return PartialView("_SearchBar", categoryList);
            }

            return PartialView("Error");
        }

        [Authorize]
        public async Task<ActionResult> NewListing(int? listingId)
        {
            if (listingId > 0)
            {
                HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + listingId);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

                    if (User.Identity.Name != listing.User.Username)
                    {
                        return View("Unauthorised");
                    }

                    if (listing.Bids.Count > 0)
                    {
                        ViewBag.BidAmount = (listing.Bids.Max(bid => bid.BidAmount)) + bidIncrement;
                    }
                    else
                    {
                        ViewBag.BidAmount = (listing.Price) + bidIncrement;
                    }

                    responseMessage = await client.GetAsync(apiUrl + "/GetAllProductCategories");
                    responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);
                    ViewData["Categories"] = categoryList;

                    return View(listing);
                }
            }
            else
            {
                HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetAllProductCategories");

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);

                    ViewData["Categories"] = categoryList;

                    return View();
                }
            }
            
            return View("Error");                    
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> DeleteComment(int commentId, int listingId)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + listingId);
            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
            ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            responseMessage = await client.GetAsync("http://localhost:58999/API/user/GetUser/" + int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
            responseData = responseMessage.Content.ReadAsStringAsync().Result;
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(responseData);

            CommentDTO commentToDelete = new CommentDTO();

            commentToDelete.Id = commentId;
            commentToDelete.Content = "";
            commentToDelete.Listing = listing;
            commentToDelete.User = user;

            string content = JsonConvert.SerializeObject(commentToDelete);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await client.PostAsync(apiUrl + "/DeleteComment", byteContent);

            return RedirectToAction("Detail", "Listing", new { id = listingId });                    
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditComment(int commentId, int listingId, string comment)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + listingId);
            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
            ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            responseMessage = await client.GetAsync("http://localhost:58999/API/user/GetUser/" + int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
            responseData = responseMessage.Content.ReadAsStringAsync().Result;
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(responseData);

            CommentDTO commentToEdit = new CommentDTO();

            commentToEdit.Id = commentId;
            commentToEdit.Content = comment;
            commentToEdit.Listing = listing;
            commentToEdit.User = user;

            string content = JsonConvert.SerializeObject(commentToEdit);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await client.PostAsync(apiUrl + "/EditComment", byteContent);

            return RedirectToAction("Detail", "Listing", new { id = listingId });

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> NewListing(ListingDetailDTO newListing)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetAllProductCategories");

                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                        List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);

                        ViewData["Categories"] = categoryList;

                        return View(newListing);
                    }

                    return View("Error"); 
                }
                else
                {
                    ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
                    HttpResponseMessage responseMessage = await client.GetAsync("http://localhost:58999/API/user/GetUser/" + int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    UserDTO user = JsonConvert.DeserializeObject<UserDTO>(responseData);

                    newListing.User = user;
                    newListing.AuctionStartTime = System.DateTime.Now;
                    newListing.Status = new StatusDTO() { Id = 1 };
                    newListing.Shipping = new ShippingDTO() { Id = 1 };

                    responseMessage = await client.GetAsync(apiUrl + "/GetAllProductCategories");
                    responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);

                    newListing.ProductCategory = categoryList.Find(c => c.ProductCategoryName == newListing.ProductCategory.ProductCategoryName);

                    responseMessage = await client.PostAsync(apiUrl + "/PostListing", new StringContent(
                            new JavaScriptSerializer().Serialize(newListing), Encoding.UTF8, "application/json"));

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View("Error");
                    }

                }
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public async Task<ActionResult> RemoveListing(int listingId)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + listingId);
            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
            ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            responseMessage = await client.GetAsync("http://localhost:58999/API/user/GetUser/" + int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
            responseData = responseMessage.Content.ReadAsStringAsync().Result;
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(responseData);

            ListingDTO listingToDelete = new ListingDTO();
            listingToDelete = listing;
            listingToDelete.Id = listingId;
            listingToDelete.User = user;

            string content = JsonConvert.SerializeObject(listingToDelete);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await client.PostAsync(apiUrl + "/RemoveListing", byteContent);

            ClaimsPrincipal prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string userId = prinicpal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            return RedirectToAction("Detail", "User", new { id = userId });
        }

        [Authorize]
        public async Task<ActionResult> EditListing(int listingId)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + listingId);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

                if (User.Identity.Name != listing.User.Username)
                {
                    return View("Unauthorised");
                }

                if (listing.Bids.Count > 0)
                {
                    ViewBag.BidAmount = (listing.Bids.Max(bid => bid.BidAmount)) + bidIncrement;
                }
                else
                {
                    ViewBag.BidAmount = (listing.Price) + bidIncrement;
                }

                responseMessage = await client.GetAsync(apiUrl + "/GetAllProductCategories");
                responseData = responseMessage.Content.ReadAsStringAsync().Result;
                List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);
                ViewData["Categories"] = categoryList;

                return View(listing);
            }
            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditListing(ListingDetailDTO listingDetailDto, int listingId)
        {            
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetListing/" + listingId);
            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
            ListingDetailDTO listing = JsonConvert.DeserializeObject<ListingDetailDTO>(responseData);

            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            responseMessage = await client.GetAsync("http://localhost:58999/API/user/GetUser/" + int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
            responseData = responseMessage.Content.ReadAsStringAsync().Result;
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(responseData);

            if (user.Id != listing.User.Id)
            {
                return View("Unauthorised");
            }

            ListingDetailDTO listingToEdit = new ListingDetailDTO();
            listingToEdit = listing;
            listingToEdit.Price = listingDetailDto.Price;
            listingToEdit.ItemName = listingDetailDto.ItemName;
            listingToEdit.ImageUrl = listingDetailDto.ImageUrl;
            listingToEdit.Description = listingDetailDto.Description;
            listingToEdit.AuctionEndTime = listingDetailDto.AuctionEndTime;
            listingToEdit.Quantity = listingDetailDto.Quantity;

            responseMessage = await client.GetAsync(apiUrl + "/GetAllProductCategories");
            responseData = responseMessage.Content.ReadAsStringAsync().Result;
            List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);

            listingToEdit.ProductCategory = categoryList.Find(c => c.ProductCategoryName == listingDetailDto.ProductCategory.ProductCategoryName);
            listingToEdit.User = user;

            string content = JsonConvert.SerializeObject(listingToEdit);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await client.PostAsync(apiUrl + "/EditListing", byteContent);

            ClaimsPrincipal prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string userId = prinicpal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            return RedirectToAction("Detail", "User", new { id = userId });
        }

        public ActionResult Slider()
        {
            return View();
        }
    }
}