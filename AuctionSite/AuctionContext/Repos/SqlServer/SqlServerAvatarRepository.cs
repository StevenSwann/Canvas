using AuctionPOCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionContext
{
    public class SqlServerAvatarRepository : IAvatarRepository
    {
        Context context;

        public SqlServerAvatarRepository()
        {
            context = new Context();
        }

        public SqlServerAvatarRepository(Context _context)
        {
            this.context = _context;
        }

        public List<AvatarImage> GetAvatarsForUser(int id)
        {
            return context.Avatars.Where(b => b.UserId == id).OrderByDescending(c => c.Id).ToList();
        }

        public AvatarImage GetAvatar(int id)
        {
            return context.Avatars.Where(b => b.Id == id).First();
        }

        public AvatarImage AddAvatar(AvatarImage avatar)
        {
            context.Avatars.Add(avatar);
            context.SaveChanges();
            return avatar;
        }
    }
}
