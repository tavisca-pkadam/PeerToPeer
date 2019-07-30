using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Client
    {
        Socket _sender;
        IPAddress _ipAddress;
        IPEndPoint _localEndPoint;

        public Client()
        {
            _ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            _localEndPoint = new IPEndPoint(_ipAddress, 11111);
        }

        public Client(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out _ipAddress))
            {
                _ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            }
            _localEndPoint = new IPEndPoint(_ipAddress, 11111);
        }

        public void CreateSocket()
        {
            _sender = new Socket(_ipAddress.AddressFamily,
                     SocketType.Stream, ProtocolType.Tcp);
        }

        public void EstablishConnection()
        {
            _sender.Connect(_localEndPoint);
        }

        public void SendMessage(string message)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(message);
            int byteSent = _sender.Send(messageSent);
            Console.WriteLine($"Client -> {message}");

        }

        public void RecieveMessage()
        {
            byte[] messageReceived = new byte[1024];

            int byteRecv = _sender.Receive(messageReceived);
            string message = Encoding.ASCII.GetString(messageReceived,
                                             0, byteRecv);
            Console.WriteLine($"Server -> {message}");
        }

        public void TerminateConnection()
        {
            Console.ReadKey(true);
            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();
        }

        public void ExecuteClient()
        {

            try
            {
                CreateSocket();

                try
                {

                    EstablishConnection();

                    Console.WriteLine("Socket connected to -> {0} ",
                                  _sender.RemoteEndPoint.ToString());
                    SendMessage("Test Client");

                    string data = "";
                    var readMessageThread = new Thread(_ =>
                    {
                        while (true)
                        {
                            byte[] messageReceived = new byte[1024];

                            int byteRecv = _sender.Receive(messageReceived);
                            if (byteRecv > 1)
                            {
                                string message = Encoding.ASCII.GetString(messageReceived,
                                                            0, byteRecv);
                                data += $"Server -> {message}" + "\n";

                                Console.Clear();
                                Console.WriteLine("______________--- This Is Client --- __________________");
                                Console.WriteLine(data);
                                Console.WriteLine(">>>");
                            }
                        }
                    });
                    readMessageThread.Start();

                    // var writeMessage = new Thread(_ =>
                    //{
                    //    while(true)
                    //    {
                    //        string sendMessage = Console.ReadLine();
                    //        SendMessage(sendMessage);

                    //        if (sendMessage.Contains("EOF"))
                    //            break;
                    //    }
                    //});

                    while (true)
                    {
                        string sendMessage = Console.ReadLine();

                        byte[] messageSent = Encoding.ASCII.GetBytes(sendMessage);
                        int byteSent = _sender.Send(messageSent);
                        Console.WriteLine($"Client -> {sendMessage}");
                        data += $"Client -> {sendMessage}" + "\n";
                        if (sendMessage.Contains("EOF"))
                        {
                            break;
                        }

                        Console.Clear();
                        Console.WriteLine("______________--- This Is Client --- __________________");
                        Console.WriteLine(data);
                        Console.WriteLine(">>>");
                    }


                    TerminateConnection();

                }

                // Manage of Socket's Exceptions 
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
    }

}
