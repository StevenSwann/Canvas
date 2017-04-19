using AuctionDTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MoreLinq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using PagedList;

namespace CanvasMVC.Controllers
{
    public class UserController : Controller
    {
        string apiUrl = "http://localhost:58999/API/User";
        HttpClient client;

        public UserController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ActionResult> GetImg(int? id)
        {
                HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetUserAvatar/" + id);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    AvatarImageDTO avatar = JsonConvert.DeserializeObject<AvatarImageDTO>(responseData);

                    FileContentResult file = new FileContentResult(avatar.AvatarImageBytes, "image/JPEG");
                    return file;
                }
                return new EmptyResult();                    
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> DismissBid(int Id, int ListingId, int UserId)
        {
            BidDTO bid = new BidDTO()
            {
                Id = Id, 
                Listing = new ListingDTO() { Id = ListingId, AuctionEndTime = System.DateTime.UtcNow },
                User = new UserDTO() { Id = UserId }             
            };

            string content = JsonConvert.SerializeObject(bid);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage result = await client.PostAsync(apiUrl + "/DismissBid", byteContent);

            return new EmptyResult();
        }

        public async Task<ActionResult> Detail(int? id)
        {       
              
            if (id == null || id == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl + "/GetUser/" + id);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string responseData = responseMessage.Content.ReadAsStringAsync().Result;

                UserDetailDTO userDetail = JsonConvert.DeserializeObject<UserDetailDTO>(responseData);
                             

                return View(userDetail);
            }
            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditUser(UserEditDTO model, HttpPostedFileBase picture)
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Detail", "User", new { id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value) });
            }            

            UserEditDTO userToEdit = new UserEditDTO();

            userToEdit.Address = model.Address;
            userToEdit.EmailAddress = model.EmailAddress;        
            userToEdit.Id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (picture != null)
            {
                if (picture.ContentType == "image/jpeg" || picture.ContentType == "image/jpg" || picture.ContentType == "image/png" || picture.ContentType == "image/gif")
                {
                    byte[] image = new byte[picture.ContentLength];
                    picture.InputStream.Read(image, 0, image.Length);

                    userToEdit.AvatarImage = new AvatarImageDTO()
                    {
                        AvatarImageBytes = image,
                        User = new UserDTO() { Id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value) }
                    };
                }                
            }
           
            string content = JsonConvert.SerializeObject(userToEdit);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage result = await client.PostAsync(apiUrl + "/EditUser", byteContent);

            if (result.StatusCode == HttpStatusCode.Conflict)
            {
                TempData["EmailError"] = "true";
                return RedirectToAction("Detail", "User", new { id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value) });                
            }
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("Detail", "User", new { id = userToEdit.Id });             
            }
            else
            {
                return View("Error");
            }
            
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditUserPassword(UserEditPasswordDTO model)
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Detail", "User", new { id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value) });
            }   

            UserEditPasswordDTO userToEdit = new UserEditPasswordDTO();

            userToEdit.OldPassword = model.OldPassword;
            userToEdit.NewPassword = model.NewPassword;
            userToEdit.ConfirmNewPassword = model.ConfirmNewPassword;
            userToEdit.Id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);


            string content = JsonConvert.SerializeObject(userToEdit);
            Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage result = await client.PostAsync(apiUrl + "/EditUserPassword", byteContent);

            if (result.StatusCode == HttpStatusCode.Conflict)
            {
                return View("Error");
            }
            if (result.StatusCode == HttpStatusCode.OK)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                TempData["PasswordChanged"] = "true";
                return RedirectToAction("Login", "Account");
            }
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return View("Unauthorised");
            }
            else
            {
                return View("Error");
            }

        }

    }
}