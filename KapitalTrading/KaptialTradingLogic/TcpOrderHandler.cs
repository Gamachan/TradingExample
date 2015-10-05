using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KapitalTradingDomain;
using vAutomtion.Logger.LoggerManager;

namespace KaptialTradingLogic
{
    public class TcpOrderHandler : IDisposable
    {
        private TcpClient tcpClient;

        /// <summary>
        /// Submit new order.
        /// </summary>
        /// <param name="orderSingle">Object of new order single.</param>
        public void SubmitNewOrder(NewOrderSingle orderSingle)
        {
            LoggerManager.Log(Level.Debug, "Submit new order - " + orderSingle.UniqueOrderID);
            XmlSerializer serializer = new XmlSerializer(typeof(NewOrderSingle));
            SubmitOrder(serializer, orderSingle);

        }

        /// <summary>
        /// Submit order cancelation.
        /// </summary>
        /// <param name="orderCancel"></param>
        public void SubmitOrderCancel(OrderCancel orderCancel)
        {
            LoggerManager.Log(Level.Debug, "Submit  order cancelation - " + orderCancel.UniqueOrderID);
            XmlSerializer serializer = new XmlSerializer(typeof(OrderCancel));
            SubmitOrder(serializer, orderCancel);
        }

        /// <summary>
        /// Submit order with tcp client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer"></param>
        /// <param name="relevantObject"></param>
        internal void SubmitOrder<T>(XmlSerializer serializer, T relevantObject)
        {
            try
            {
                LoggerManager.Log(Level.Debug, "Initialize tcp client.");
                tcpClient = new TcpClient();

                LoggerManager.Log(Level.Debug, "Set memory stream");
                using (var memoryStream = new MemoryStream())
                {
                    serializer.Serialize(memoryStream, relevantObject);
                }

                LoggerManager.Log(Level.Debug, "Set network stream");
                NetworkStream networkStream = tcpClient.GetStream();
                if (networkStream.CanWrite)
                {
                    serializer.Serialize(networkStream, relevantObject);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Log(Level.Error, "Failed to send order with TCP clinet.Error: "+ex.Message);
            }
        }

        /// <summary>
        /// Dispose TCP connection.
        /// </summary>
        public void Dispose()
        {
            if (tcpClient != null)
            {
                tcpClient.Client.Dispose();
            }
        }
    }
}
