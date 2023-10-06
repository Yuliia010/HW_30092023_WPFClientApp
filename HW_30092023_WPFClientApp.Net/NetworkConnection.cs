using Microsoft.VisualBasic;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HW_30092023_WPFClientApp.Net
{
    public class NetworkConnection
    {
        private TcpClient tcpClient;
        private IPEndPoint endPoint;
        private NetworkStream stream;
        private string answer;

        public async Task SendLog(string login)
        {
            string loginMessage = login;
            byte[] loginData = Encoding.UTF8.GetBytes(loginMessage);
            await tcpClient.GetStream().WriteAsync(loginData);

        }

        public async Task SendPass(string pass)
        {
            string passMessage = pass;
            byte[] passData = Encoding.UTF8.GetBytes(passMessage);
            await tcpClient.GetStream().WriteAsync(passData);
        }

        public async Task Connect(string host, int port, string login, string password)
        {

            try
            {
                endPoint = new IPEndPoint(IPAddress.Parse(host), port);
                tcpClient = new TcpClient();

                await tcpClient.ConnectAsync(endPoint);

                if (!tcpClient.Connected)
                {
                    throw new Exception("Not connected to the server!");
                }

                await SendLog(login);
                await GetAnswer();

                if (answer == "Reject")
                {
                    await Disconnect();
                    throw new Exception("Invalid login!");
                }

                await SendPass(password);
                await GetAnswer();

                if (answer == "Reject")
                {
                   await Disconnect();
                    throw new Exception("Invalid password!");
                }
            }
            catch (SocketException ex)
            {
                throw ex;
            }
        }

        public async Task Disconnect()
        {
            if (tcpClient != null)
            {
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                }
            }
        }

        public bool IsConnected()
        {
            if(tcpClient != null)
            {
               
                return tcpClient.Connected;
            }
           return false;
        }

        public async Task SendARequest()
        {
            stream = tcpClient.GetStream();

            var requestMessage = $"GET\r\n";
            var requestData = Encoding.UTF8.GetBytes(requestMessage);
            await stream.WriteAsync(requestData);


        }

        public string ReturnAnswer()
        {
            return answer;
        }

        public async Task GetAnswer()
        {
            var responseData = new byte[8192];
            stream = tcpClient.GetStream();
            var response = new StringBuilder();
            int bytes;
            do
            {
                bytes = await stream.ReadAsync(responseData);
                response.Append(Encoding.UTF8.GetString(responseData, 0, bytes));
            }
            while (stream.DataAvailable);

            answer = response.ToString();
        }
    }
}