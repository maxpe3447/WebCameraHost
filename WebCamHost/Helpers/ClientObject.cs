using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WebCamHost.Services.ClientsEndPoints;
using WebCamHost.Services.TCPServer;

namespace WebCamHost.Helpers
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        TCPServer server; // объект сервера

        public ClientObject(TcpClient tcpClient, TCPServer serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            //serverObject.AddConnection(this);
        }

        public void Process(/*ref IClientsEndPoints _clientsEndPoints*/)
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                //string message = GetMessage();
                //userName = message;

                //message = userName + " вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                //server.BroadcastMessage(message, this.Id);
                //Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                //BitmapImage message;
                //while (true)
              //{
                    try
                    {
                    ClientEndPointList.Ports.Add(GetPort());
                    ClientEndPointList.Hosts.Add(GetIP());

                    MessageBox.Show($"Port: {ClientEndPointList.Ports.First()}");
                    MessageBox.Show($"IP: {ClientEndPointList.Hosts.First()}");
                    //_clientsEndPoints.Ports.Add(GetPort());
                    //_clientsEndPoints.Hosts.Add(GetIP());
                    //MessageBox.Show( $"Port: {_clientsEndPoints.Ports.First()}");
                    //MessageBox.Show( $"IP: {_clientsEndPoints.Hosts.First()}");

                    //message = server.GetImage();//GetMessage();
                    //message = String.Format("{0}: {1}", userName, message);
                    //Console.WriteLine(message);
                    //server.BroadcastMessage(/*message,*/ this.Id);
                }
                    catch
                    {
                        //message = String.Format("{0}: покинул чат", userName);
                        MessageBox.Show("Client disconected");
                        //Console.WriteLine(message);
                        //server.BroadcastMessage(/*message,*/ this.Id);
                        //break;
                    }
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                //server.RemoveConnection(this.Id);
                Close();
            }
        }
        // чтение входящего сообщения и преобразование в строку
        private string GetIP()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString().Remove(builder.Length);
        }
        private int GetPort()
        {
            byte[] reciveLen = new byte[4]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int lenght = 0;
            int bytes = 0;

                bytes = Stream.Read(reciveLen, 0, 4);

                lenght = BitConverter.ToInt32(reciveLen, 0);

            return lenght;
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
