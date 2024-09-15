using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sem2Task2
{
    internal class Server
    {
        private static void Register()
        {

        }
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static CancellationToken ct = cts.Token;
        private static bool exitRequested = false;        
        public static async Task msgServ()
        {
            Task t = new Task(AcceptMsg, ct);
            t.Start();
            try
            {
                await t;
            }
            catch (OperationCanceledException e)
            {
                if (t.IsCanceled)
                {
                    Console.WriteLine("Задача завершена");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Задача не была завершена");
                    Console.ReadLine();
                };
            }
            t.Wait();
        }
        public static void AcceptMsg()
        {
            Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(5050);

            Console.WriteLine("Сервер ожидает сообщение. Нажмите <Esc> для выхода: ");
            string servMessage = String.Empty;
            //ConsoleKeyInfo info = Console.ReadKey(true);


            Task exitTask = Task.Run(() =>
            {
                Console.WriteLine("Жду сообщения для exitTask");
                Console.ReadKey();
                exitRequested = true;
                cts.Cancel();
            });


            while (!ct.IsCancellationRequested)//while (!exitRequested)
            {
                Console.WriteLine("возможно    здесь ставить exitRequested = true ");
                var data = udpClient.Receive(ref ep);

                string data1 = Encoding.UTF8.GetString(data);
                Task.Run(() =>
                {
                    Message? originalServerMes = Message.FromJson(data1);  //было Message? someMessage = Message.FromJson(data1);
                    Message someMessage = originalServerMes.Clone();      //после шаблона Прототип


                    Message responseMsg = new Message();
                    if (someMessage.ToName.Equals("Server"))
                    {
                        if (someMessage.Text.ToLower().Equals("register"))
                        {
                            if (clients.TryAdd(someMessage.FromName, ep))
                            {
                                responseMsg = new Message("Server", $"Клиент добавлен: {someMessage.FromName}");
                            }
                        }
                        else if (someMessage.Text.ToLower().Equals("delete"))
                        {
                            if (clients.TryGetValue(someMessage.FromName, out ep)) 
                            {
                                clients.Remove(someMessage.FromName);
                                responseMsg = new Message("Server", $"Клиент {someMessage.FromName} удалён");
                            }
                        }
                        else if (someMessage.Text.ToLower().Equals("list"))
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var client in clients)
                            {
                                sb.Append(client.Key + "\n");
                            }
                            responseMsg = new Message("Server", $"Список клиентов: \n {sb.ToString()}");

                        }


                    }
                    else if (clients.TryGetValue(someMessage.ToName, out IPEndPoint? value))
                    {
                        string js1 = someMessage.ToJson();
                        byte[] bytes1 = Encoding.UTF8.GetBytes(js1);
                        udpClient.Send(bytes1, bytes1.Length, value);
                        responseMsg = new Message("Server", $"Пользоватялю {someMessage.ToName} отправелено сообщение");
                    }
                    else if (someMessage.ToName.ToLower().Equals("all"))
                    {
                        foreach (var client in clients)
                        {
                            someMessage.ToName = client.Key;
                            //string js1 = someMessage.ToJson();
                            string js1 = someMessage.ToJson();



                            byte[] bytes1 = Encoding.UTF8.GetBytes(js1);
                            udpClient.Send(bytes1, bytes1.Length, client.Value);
                        }
                        responseMsg = new Message("Server", $"Сообщение отправлено всем клиентам");//
                    }

                    else
                    {
                        responseMsg = new Message("Server", $"Пользователя {someMessage.Text} не существует");
                    }

                    Console.WriteLine(someMessage.ToString());
                    //Message responseMsg = new Message("Server", "Message accept on serv!");
                    string responseMsgJs = responseMsg.ToJson();
                    byte[] responseDate = Encoding.UTF8.GetBytes(responseMsgJs);
                    udpClient.Send(responseDate, responseDate.Length, ep);
                });

            }


        }
    }
}
