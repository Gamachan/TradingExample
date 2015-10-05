using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapitalTradingDomain
{
    public class OrderMessage
    {
        public long SendingTimestampUTC { get; set; }
        public String UniqueOrderID { get; set; }
    }
}
