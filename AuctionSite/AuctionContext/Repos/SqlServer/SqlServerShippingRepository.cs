using AuctionPOCOs;
using System.Collections.Generic;
using System.Linq;

namespace AuctionContext
{
    public class SqlServerShippingRepository : IShippingRepository
    {
        Context context;

        public SqlServerShippingRepository(Context _context)
        {
            this.context = _context;
        }

        public List<Shipping> GetAllShippingStatuses()
        {
            return context.Shippings.ToList();
        }

        public Shipping GetShippingStatusById(int id)
        {
            var shipping = context.Shippings.FirstOrDefault(s => s.Id == id);
            if (shipping == null)
            {
                return new Shipping();
            }
            return shipping;
        }

        public Shipping AddShipping(Shipping shipping)
        {
            context.Shippings.Add(shipping);
            context.SaveChanges();
            return shipping;
        }

        public bool CheckShippingMode(string shipMode)
        {
            var mode = context.Shippings.FirstOrDefault(s => s.ShipMode == shipMode);
            if (mode == null)
            {
                return false;
            }
            return true;
        }
    }
}
