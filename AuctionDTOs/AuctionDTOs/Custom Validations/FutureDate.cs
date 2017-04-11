using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDTOs.Custom_Validations
{
    public sealed class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value != null && ((DateTime)value).Subtract(TimeSpan.FromDays(1)) > DateTime.Now;
        }
    }
}
