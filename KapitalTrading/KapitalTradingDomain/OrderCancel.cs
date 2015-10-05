using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapitalTradingDomain
{
    [Serializable]
    public class OrderCancel : OrderMessage
    {
        public String OrderIDofOrderToCancel { get; set; }

    }
}
