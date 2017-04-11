using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AuctionDTOs;

namespace CanvasMVC.Controllers
{
    public class HomeController : Controller
    {
        string apiUrl = "http://localhost:58999/API/Listing";
        HttpClient client;

        public HomeController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ActionResult> Index()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetAllListings");

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                List<ListingDTO> listingList = JsonConvert.DeserializeObject<List<ListingDTO>>(responseData);

                return View(listingList);
            }

            return View("Error");
        }

        public async Task<EmptyResult> CategoriesInitilise()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetAllProductCategories");
            string responseData = responseMessage.Content.ReadAsStringAsync().Result;
            List<ProductCategoryDTO> categoryList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseData);
            ViewData["Categories"] = categoryList;

            return new EmptyResult();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}