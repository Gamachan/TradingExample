using CsvHelper;
using KapitalTradingDomain;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vAutomtion.Logger.LoggerManager;

namespace KaptialTradingLogic
{

    public class KapitalTradingParser<T>
    {
        /// <summary>
        /// Get order book list data from the file.
        /// </summary>
        /// <param name="orderBookList"></param>
        /// <param name="orderBookCsvFilePath"></param>
        public void GetOrderBookListFromFile(List<OrderBook> orderBookList, string orderBookCsvFilePath)
        {
            int level = GetLevelNumber(orderBookCsvFilePath);

            LoggerManager.Log(Level.Debug, "Order book level is   - " + level);
            DataTable dt = ReadCsvToDataTable(orderBookCsvFilePath, level);

            LoggerManager.Log(Level.Debug, "Convert da table to order book  for the file - " + orderBookCsvFilePath);
            ConvertDataTableToOrderBooks(dt, level, orderBookList);
        }

        /// <summary>
        /// Conver data table to order book object.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="level"></param>
        /// <param name="orderBooks"></param>
        private void ConvertDataTableToOrderBooks(DataTable dt, int level, List<OrderBook> orderBooks)
        {
            LoggerManager.Log(Level.Debug, "Start converting data table to order book.");
            for (int i = 0; i < orderBooks.Count(); i++)
            {
                List<OrderBookObject> orderBookList = new List<OrderBookObject>();
                int offset = 0;
                for (int j = 0; j < level; j++)
                {
                    offset = j * 4;
                    OrderBookObject orderBook = new OrderBookObject();
                    orderBook.AskPrice = decimal.Parse(dt.Rows[i][offset].ToString());
                    orderBook.AskSize = int.Parse(dt.Rows[i][offset + 1].ToString());
                    orderBook.BidPrice = decimal.Parse(dt.Rows[i][offset + 2].ToString());
                    orderBook.BidSize = int.Parse(dt.Rows[i][offset + 3].ToString());
                    orderBookList.Add(orderBook);
                }
                orderBooks[i].OrderBookList = orderBookList;
                orderBooks[i].OrderID = Guid.NewGuid().ToString();
            }

            LoggerManager.Log(Level.Debug, "Finished converting data table to order book.");
        }

        /// <summary>
        /// Get level number from file name.
        /// </summary>
        /// <param name="csvFilePath"></param>
        /// <returns></returns>
        private int GetLevelNumber(string csvFilePath)
        {
            return int.Parse(csvFilePath.Substring(csvFilePath.Length - 5, 1));
        }

        /// <summary>
        /// Get data from orderbook file.
        /// </summary>
        /// <param name="csvPath"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private DataTable ReadCsvToDataTable(string csvPath, int level)
        {
            LoggerManager.Log(Level.Debug, "Start reading csv file to data table. Csv file path - "+csvPath);
            DataTable dt = new DataTable();

            for (int i = 0; i < level; i++)
            {
                dt.Columns.Add("AskPrice_" + (i + 1).ToString());
                dt.Columns.Add("AskSize_" + (i + 1).ToString());
                dt.Columns.Add("BidPrice_" + (i + 1).ToString());
                dt.Columns.Add("BidSize_" + (i + 1).ToString());
            }

            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader))
            {
                while (csv.Read())
                {
                    var row = dt.NewRow();

                    if (dt.Rows.Count < 1)
                    {
                        string[] header = csv.FieldHeaders;
                        for (int i = 0; i < header.Length; i++)
                        {
                            row[i] = header[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            row[i] = csv.GetField(i);
                        }
                    }

                    dt.Rows.Add(row);
                }
            }

            LoggerManager.Log(Level.Debug, "Finished reading csv file to data table. Csv file path - " + csvPath);
            return dt;
        }

        /// <summary>
        /// Get all relevant data from message file.
        /// </summary>
        /// <param name="orderMessageListCsvFilePath"></param>
        /// <returns></returns>
        public List<OrderBook> GetMessageBookListFromFile(string orderMessageListCsvFilePath)
        {
            LoggerManager.Log(Level.Debug, "Start reading csv file to order book object. Csv file path - " + orderMessageListCsvFilePath);
            List<OrderBook> orderBookList = new List<OrderBook>();

            using (var reader = new StreamReader(orderMessageListCsvFilePath))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.RegisterClassMap<OrderBookMap>();
                while (csv.Read())
                {
                    OrderBook record = csv.GetRecord<OrderBook>();
                    orderBookList.Add(record);
                }
            }

            LoggerManager.Log(Level.Debug, "Finished reading csv file to order book object. Csv file path - " + orderMessageListCsvFilePath);
            return orderBookList;
        }
    }
}
