using AuctionContext;
using AuctionDTOs;
using AuctionPOCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace AuctionWebApi.Controllers
{
    public class UserController : ApiController
    {
        Context dbcontext;
        Mapper mapper;
        IUserRepository userRepo;
        IAvatarRepository avatarRepo;
        IBidRepository bidRepo;

        public UserController()
        {
            dbcontext = new Context();
            mapper = new Mapper(dbcontext);
            userRepo = new SqlServerUserRepository(dbcontext);
            avatarRepo = new SqlServerAvatarRepository(dbcontext);
            bidRepo = new SqlServerBidRepository(dbcontext);
        }

        //GET /api/user/getallusers
        [HttpGet]
        public List<UserDTO> GetAllUsers()
        {
            return userRepo.GetAllUsers().Select(a => mapper.CreateUserDTO(a)).ToList();
        }

        //GET /api/user/getuser/1
        [HttpGet]
        [ResponseType(typeof(UserDetailDTO))]
        public IHttpActionResult GetUser(int id)
        {
            var user = userRepo.GetUserById(id);
            if (user.Id == 0)
            {
                return NotFound();
            }
            return Ok(mapper.CreateUserDetailDTO(user));
        }

        [HttpGet]
        [ResponseType(typeof(AvatarImageDTO))]
        public IHttpActionResult GetUserAvatar(int id)
        {
            var avatar = avatarRepo.GetAvatar(id);
            if (avatar.Id == 0 || avatar == null)
            {
                return NotFound();
            }
            return Ok(mapper.CreateAvatarDTO(avatar));
        }

        //POST /api/user/register
        [HttpPost]
        public HttpResponseMessage RegisterUser(UserRegisterDTO userDTO)
        {
            User userToRegister = mapper.CreateUserRegisterEntity(userDTO);
            bool duplicateUsername = userRepo.CheckUsername(userToRegister.Username);
            bool duplicateEmailAddress = userRepo.CheckEmailAddress(userToRegister.EmailAddress);
            if (duplicateUsername == false && duplicateEmailAddress == false)
            {
                userToRegister = userRepo.AddNewUser(userToRegister);
                if (userToRegister.Id == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Created);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }

        //POST /api/user/login
        [HttpPost]
        public HttpResponseMessage LoginUser(UserLoginDTO userDTO)
        {
            User userToLogin = mapper.CreateUserLoginEntity(userDTO);
            userToLogin = userRepo.LoginUser(userToLogin);
            if (userToLogin.Id == 0)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
            else
            {
                var response = Request.CreateResponse<AuthenticatedUserDTO>(HttpStatusCode.OK,
                                mapper.CreateAuthenticatedUserDTO(userToLogin));
                string uri = Url.Link("DefaultApi", new { id = userToLogin.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }            
        }

        //POST /api/user/edituser
        [HttpPost]
        public HttpResponseMessage EditUser(UserEditDTO userDTO)
        {
            User userToEdit = mapper.CreateUserEditEntity(userDTO);
            bool duplicateEmailAddress = userRepo.CheckEmailAddress(userToEdit.EmailAddress);
            if (duplicateEmailAddress == false)
            {                
                if (userToEdit.Id == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                else
                {
                    userRepo.EditUser(userToEdit);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            else if (userRepo.GetUserById(userDTO.Id).EmailAddress == userToEdit.EmailAddress)
            {
                userRepo.EditUser(userToEdit);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }

        //POST api/user/dismissbid
        [HttpPost]
        public IHttpActionResult DismissBid(BidDTO bidDTO)
        {
            List<Bid> bidsToDismiss = mapper.CreateBidEditEntities(bidDTO);
            bidRepo.DismissBid(bidsToDismiss);            
            return Ok();
        }

        //POST /api/user/edituserpassword
        [HttpPost]
        public HttpResponseMessage EditUserPassword(UserEditPasswordDTO userDTO)
        {
            User userToEdit = mapper.CreateUserEditPasswordEntity(userDTO);
            bool passwordCheck = userRepo.CheckPassword(userToEdit);
            if (passwordCheck == true)
            {
                if (userToEdit.Id == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                else
                {
                    userRepo.EditUserPassword(userToEdit);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
    }
}
