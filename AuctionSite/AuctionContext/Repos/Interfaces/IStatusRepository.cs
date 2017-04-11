using AuctionPOCOs;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface IStatusRepository
    {
        List<Status> GetAllStatuses();
        Status GetStatusById(int id);
        Status AddStatus(Status status);
        bool CheckStatus(string status);
    }
}
