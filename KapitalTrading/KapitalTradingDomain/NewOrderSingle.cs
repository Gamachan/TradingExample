using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapitalTradingDomain
{
    [Serializable]
    public class NewOrderSingle : OrderMessage
    {
        public SideEnum Side { get; set; }
        public long Quantity { get; set; }
        public long Price { get; set; }
    }
}
