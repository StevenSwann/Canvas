using AuctionPOCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using PasswordSecurity;

namespace AuctionContext
{
    public class SqlServerUserRepository : IUserRepository
    {
        private Context context;

        public SqlServerUserRepository(Context _context)
        {
            this.context = _context;
        }

        public SqlServerUserRepository()
        {
            context = new Context();
        }

        public List<User> GetAllUsers()
        {
             return context.Users.ToList();
        }

        public User GetUserById(int id)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return new User();
            }
            return user;
        }

        public bool CheckUsername(string username)
        {
            var user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckPassword(User userToEdit)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == userToEdit.Id);
            
            if (PasswordStorage.VerifyPassword(userToEdit.Username, user.Password))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool CheckEmailAddress(string email)
        {
            var user = context.Users.FirstOrDefault(u => u.EmailAddress == email);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public User AddNewUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public User LoginUser(User userToLogin)
        
        {
            var user = context.Users.FirstOrDefault(u => u.EmailAddress == userToLogin.EmailAddress);
            
            if (user == null)
            {
                return new User();
            }
            else if (PasswordStorage.VerifyPassword(userToLogin.Password, user.Password))
            {
                return user;
            }
            else
            {
                return new User();
            }
        }


        public void EditUser(User userToEdit)
        {
            User user = context.Users.First(u => u.Id == userToEdit.Id);
            user.Address = userToEdit.Address;
            user.EmailAddress = userToEdit.EmailAddress;
            user.AvatarImage = userToEdit.AvatarImage;
            context.SaveChanges();
        }



        public void EditUserPassword(User userToEdit)
        {
            User user = context.Users.First(u => u.Id == userToEdit.Id);            
            user.Password = PasswordStorage.CreateHash(userToEdit.Password);            
            context.SaveChanges();
        }
    }
}
