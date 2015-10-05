using System.Collections.Generic;
namespace KapitalTradingDomain
{
    public class OrderBook
    {
        public decimal Time { get; set; }
        public string OrderID { get; set; }
        public long Size { get; set; }
        public long Price { get; set; }
        public List<OrderBookObject> OrderBookList { get; set; }
    }
}
