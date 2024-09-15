using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
/*
 Лекция 9 тема. Семинар 4
Отправка сообщений нескольким клиентам
 */
namespace Sem2Task2
{
    internal class Message
    {
        public string FromName { get; set; }
        public string ToName { get; set; } 
        public string Text { get; set; }
        public DateTime Stime { get; set; }

        public Message(string fromName, string toName, string text, DateTime stime) //   /*прототип
        {
            FromName = fromName;
            ToName = toName;
            Text = text;
            Stime = stime;
        }
        public Message Clone()      //    прототип */
        {
            var p = new Message(FromName, ToName, Text, Stime);
            return p;
        }                                 

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Message? FromJson(string someMessage)
        {
            return JsonSerializer.Deserialize<Message>(someMessage);
        }
        public Message(string nikname, string text)
        {
            this.FromName = nikname;
            this.ToName = nikname;
            this.Text = text;
            this.Stime = DateTime.Now;
        }



        public Message()
        {

        }
        public override string ToString()
        {
            return $"Получено сообщение от {FromName} ({Stime.ToShortTimeString()}):{Text} ";
        }
    }
}
