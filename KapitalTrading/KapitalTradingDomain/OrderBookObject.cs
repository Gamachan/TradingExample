namespace KapitalTradingDomain
{
    public class OrderBookObject
    {
        public decimal AskPrice { get; set; }

        public decimal BidPrice { get; set; }

        public long BidSize { get; set; }

        public long AskSize { get; set; }
    }
}
