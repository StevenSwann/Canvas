using AuctionPOCOs;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface IShippingRepository
    {
        List<Shipping> GetAllShippingStatuses();
        Shipping GetShippingStatusById(int id);
        Shipping AddShipping(Shipping shipping);
        bool CheckShippingMode(string shipMode);
    }
}
