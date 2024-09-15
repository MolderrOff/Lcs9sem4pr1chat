using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Task2
{
    internal class Client
    {
        public static async Task SendMsg(string name)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5050);
            UdpClient udpClient = new UdpClient();




            while (true)
            {
                Console.WriteLine("Введите имя получателя");
                
                string toName = Console.ReadLine();
                if (String.IsNullOrEmpty(toName))
                {
                    Console.WriteLine($"Вы не ввели имя пользователя");
                    continue;
                }
                Console.WriteLine("Введите сообщение или <Exit> для выхода из Client.cs (внутри while (true):");

                string text = Console.ReadLine();

                Message newMessage = new Message(name, text);
                Message msg4 = new Message();
                msg4.ToName = toName;
                msg4.FromName = name;
                

                string doubleString = text;
                newMessage.Text = doubleString;

                msg4.Text = doubleString;
                msg4.Stime = DateTime.Now;

                Message original = new Message(name, toName, doubleString, DateTime.Now); //прототип
                Message clone = original.Clone(); //прототип

                Message msg = new Message(toName, doubleString);

                //string responseMsgJs = msg4.ToJson(); //было до шаблона Прототип
                string responseMsgJs = clone.ToJson();
                byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJs);
                await udpClient.SendAsync(responseData, responseData.Length, ep); 

                byte[] answerData = udpClient.Receive(ref ep);

                string answerMsgJs = Encoding.UTF8.GetString(answerData);
                Message? answerMsg = Message.FromJson(answerMsgJs);
                Console.WriteLine(answerMsg.ToString());



            }

        }



    }
}
