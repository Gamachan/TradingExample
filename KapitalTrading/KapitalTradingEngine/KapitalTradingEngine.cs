using KapitalTradingDomain;
using KaptialTradingLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Sockets;
using System.Xml.Serialization;
using vAutomtion.Logger.LoggerManager;

namespace KapitalTradingEngine
{

    public class KapitalTradingEngine
    {
        public bool IsRunning { get; private set; }

        private string dataFolderPath;

        private KapitalTradingParser<OrderBookObject> OrderBookParser;

        private KapitalTradingParser<OrderBook> OrderMessageParser;

        private List<OrderBook> orderBook;

        public KapitalTradingEngine()
        {
            OrderBookParser = new KapitalTradingParser<OrderBookObject>();
            OrderMessageParser = new KapitalTradingParser<OrderBook>();
            orderBook = new List<OrderBook>();
        }

        /// <summary>
        /// Initialize run.
        /// </summary>
        internal void Run()
        {
            LoggerManager.Log(Level.Debug, "Get all subdirectories deom the main path - " + dataFolderPath);
            string[] subDirecrtories = Directory.GetDirectories(dataFolderPath);
            if (!subDirecrtories.Any())
            {
                LoggerManager.Log(Level.Warning, "No subdirectories exist within folder " + dataFolderPath);
                return;
            }

            foreach (string subDire in subDirecrtories)
            {
                LoggerManager.Log(Level.Info, "Start handling files within directory  " + subDire);
                HandleDirectoryData(subDire);
            }
        }

        /// <summary>
        /// Handle all order books one by one.
        /// </summary>
        /// <param name="subDir">Path to subdirectory with relevant files.</param>
        private void HandleDirectoryData(string subDir)
        {
            try
            {
                LoggerManager.Log(Level.Debug, "Get order book file path and message file path for the subdirectory - " + subDir);
                string orderBookCsvFilePath, orderMessageListCsvFilePath;

                GetFilesPath(subDir, out orderMessageListCsvFilePath, out orderBookCsvFilePath);

                LoggerManager.Log(Level.Debug, "Get order data from the file - " + orderMessageListCsvFilePath);
                List<OrderBook> orderBookList = OrderMessageParser.GetMessageBookListFromFile(orderMessageListCsvFilePath);

                LoggerManager.Log(Level.Debug, "Get order book from for the file - " + orderMessageListCsvFilePath);
                OrderBookParser.GetOrderBookListFromFile(orderBookList, orderBookCsvFilePath);

                LoggerManager.Log(Level.Debug, "Get order book date and time from the  - " + orderBookCsvFilePath);
                DateTime orderTime = GetOrderDateFromFileName(Path.GetFileName(orderBookCsvFilePath));


                LoggerManager.Log(Level.Debug, "Start handling order books.");
                foreach (OrderBook orderBook in orderBookList)
                {
                    LoggerManager.Log(Level.Debug, "Start handling order books - " + orderBook.OrderID);
                    HandleSingleOrderBook(orderBook, orderTime);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Log(Level.Error, "Failed to handle directory " + subDir + ". Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Get order date from file name.
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        private DateTime GetOrderDateFromFileName(string pathName)
        {

            LoggerManager.Log(Level.Debug, "Get data and time from the file name - " + pathName);
            string[] fileNameParts = pathName.Split('_');

            if (fileNameParts.Any())
            {
                string dateAsStr = fileNameParts[1];

                LoggerManager.Log(Level.Debug, "Parse  - " + dateAsStr+" in date and time.");
                return DateTime.ParseExact(dateAsStr, "yyyy-MM-dd", null);

            }
            throw new Exception("Failed to extract date time from file.");

        }

        /// <summary>
        /// Handle single order book.
        /// </summary>
        /// <param name="orderBook">Order book object.</param>
        /// <param name="orderDate">Order date and time.</param>
        private void HandleSingleOrderBook(OrderBook orderBook, DateTime orderDate)
        {
            NewOrderSingle order = null;

            orderDate = orderDate.AddMilliseconds((double)orderBook.Time);

            for (int level = 0; level < orderBook.OrderBookList.Count; level++)
            {
                if (order == null)
                {
                    if (orderBook.OrderBookList[level].AskSize > 500 || orderBook.OrderBookList[level].BidSize > 500)
                    {
                        LoggerManager.Log(Level.Debug, "At least one Order book size is higher than 500. Ask size -   - " + orderBook.OrderBookList[level].AskSize
                            + ". Bid size - " + orderBook.OrderBookList[level].BidSize);

                        LoggerManager.Log(Level.Debug, "Create new order single object for the order  - " + orderBook.OrderID);
                        order = new NewOrderSingle
                        {
                            UniqueOrderID = orderBook.OrderID,
                            Price = orderBook.Price,
                            Quantity = 100,
                            SendingTimestampUTC = orderDate.ToFileTimeUtc()
                        };

                        order.Side = (orderBook.OrderBookList[level].AskSize > orderBook.OrderBookList[level].BidSize) ? SideEnum.Sell : SideEnum.Buy;

                        LoggerManager.Log(Level.Debug, "Submit new order   - " + orderBook.OrderID + " with tcp client.");
                        using (TcpOrderHandler handler = new TcpOrderHandler())
                        {
                            handler.SubmitNewOrder(order);
                        }
                    }
                }
                else
                {
                    if (orderBook.OrderBookList[level].AskSize < 200 || orderBook.OrderBookList[level].BidSize < 200)
                    {
                        LoggerManager.Log(Level.Debug, "Create order cancel object for the order  - " + orderBook.OrderID);
                        OrderCancel orderCancel = new OrderCancel
                        {
                            OrderIDofOrderToCancel = orderBook.OrderID
                        };

                        LoggerManager.Log(Level.Debug, "Submit order cancelation  - " + orderBook.OrderID + " with tcp client.");
                        using (TcpOrderHandler handler = new TcpOrderHandler())
                        {
                            handler.SubmitOrderCancel(orderCancel);
                            order = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get path for message file and order book files.
        /// </summary>
        /// <param name="directoryPath">Path to input directory.</param>
        /// <param name="orderMessageListCsvFilePath"></param>
        /// <param name="orderBookCsvFilePath"></param>
        internal void GetFilesPath(string directoryPath, out string orderMessageListCsvFilePath, out string orderBookCsvFilePath)
        {
            LoggerManager.Log(Level.Debug, "Get full file path for files within directory   - " + directoryPath);
            orderBookCsvFilePath = string.Empty;
            orderMessageListCsvFilePath = string.Empty;

            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                if (fileInfo.Name.Contains("_message_"))
                {
                    LoggerManager.Log(Level.Debug, "File name for message data - " + fileInfo.FullName);
                    orderMessageListCsvFilePath = fileInfo.FullName;
                }

                if (fileInfo.Name.Contains("_orderbook_"))
                {
                    LoggerManager.Log(Level.Debug, "File name for order book data - " + fileInfo.FullName);
                    orderBookCsvFilePath = fileInfo.FullName;
                }
            }

            if (string.IsNullOrEmpty(orderBookCsvFilePath) || string.IsNullOrEmpty(orderMessageListCsvFilePath))
            {
                throw new ArgumentNullException("Order and Message book file name cannot be null or empty.");
            }
        }

        /// <summary>
        /// Get main variables from configuration file.
        /// </summary>
        internal void Initialize()
        {
            LoggerManager.Log(Level.Info, "KapitalTradingEngine initialize main variables");
            if (ConfigurationManager.AppSettings["DataFolderPath"] == null)
            {
                throw new ArgumentNullException("DataFolderPath", "Missing DataFolderPath in confguration file.");
            }

            dataFolderPath = ConfigurationManager.AppSettings["DataFolderPath"].ToString();

            if (!Directory.Exists(dataFolderPath))
            {
                throw new ArgumentNullException("DataFolderPath", "DataFolderPath folder does not exists.");
            }

            LoggerManager.Log(Level.Debug, "Main folder path was retried from configuration file. Path - " + dataFolderPath);
            IsRunning = true;
        }

    }
}
