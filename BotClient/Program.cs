using Common;
using System.Net;

namespace BotClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, Config.PORT);

            BotClient botClient = new BotClient(endPoint);
            botClient.Start();

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}