using AuctionPOCOs;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        User GetUserById(int id);
        bool CheckUsername(string username);
        bool CheckEmailAddress(string email);
        bool CheckPassword(User userToEdit);
        User AddNewUser(User user);
        User LoginUser(User userToLogin);
        void EditUser(User userToEdit);
        void EditUserPassword(User userToEdit);
    }
}
