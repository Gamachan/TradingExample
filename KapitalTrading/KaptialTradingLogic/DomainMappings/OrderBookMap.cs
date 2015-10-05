using CsvHelper.Configuration;
using KapitalTradingDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaptialTradingLogic
{
    /// <summary>
    /// Data mapping for data Order book class.
    /// </summary>
    public sealed class OrderBookMap : CsvClassMap<OrderBook>
    {
        public OrderBookMap()
        {
            Map(m => m.Time).Index(0);
            Map(m => m.Size).Index(3);
            Map(m => m.Price).Index(4);
        }
    }
}


