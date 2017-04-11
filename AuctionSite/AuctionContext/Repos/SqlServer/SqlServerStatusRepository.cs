using AuctionPOCOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuctionContext
{
    public class SqlServerStatusRepository : IStatusRepository
    {
        Context context;

        public SqlServerStatusRepository(Context _context)
        {
            this.context = _context;
        }

        public List<Status> GetAllStatuses()
        {
            return context.Statuses.ToList();
        }

        public Status GetStatusById(int id)
        {
            var status = context.Statuses.FirstOrDefault(s => s.Id == id);
            if (status == null)
            {
                return new Status();
            }
            return status;         
        }

        public Status AddStatus(Status status)
        {
            context.Statuses.Add(status);
            context.SaveChanges();
            return status;
        }

        public bool CheckStatus(string status)
        {
            var statusObj = context.Statuses.FirstOrDefault(s => s.StatusName == status);
            if (statusObj == null)
            {
                return false;
            }
            return true;
        }
    }
}
