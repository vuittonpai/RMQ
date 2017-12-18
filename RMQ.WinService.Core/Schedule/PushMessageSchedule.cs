
using RMQ.WinService.Core.Schedule.Base;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RMQ.WinService.Core.Schedule
{
    internal class PushMessageSchedule : ScheduleBase
    {
        public PushMessageSchedule(int id) : base( id)
        {
                
        }


        /// <summary>
        /// 實作執行 商業邏輯
        /// </summary>
        /// <returns></returns>
        protected override bool Work()
        {
            
            Console.WriteLine("商業邏輯: " + Task.ScheduleData);
            SendMe(Task.ScheduleData);
            return (!string.IsNullOrEmpty(Task.ScheduleData));

        }


        private static void SendMe(string messages)
        {
            try
            {
                var mailMsg = new MailMessage();
                mailMsg.BodyEncoding = Encoding.UTF8;
                mailMsg.SubjectEncoding = Encoding.UTF8;
                mailMsg.IsBodyHtml = true;

                MailAddress from = new MailAddress("Patrick.kuo@ehsn.com.tw", "寄信人", Encoding.GetEncoding("utf-8"));
                MailAddress to = new MailAddress("Patrick.kuo@ehsn.com.tw", "收信人", Encoding.GetEncoding("utf-8"));
                MailMessage mail = new MailMessage(from, to);
                mail.Subject = "這是郵件標題";
                mail.SubjectEncoding = Encoding.GetEncoding("utf-8");
                mail.Body = messages;
                mail.BodyEncoding = Encoding.GetEncoding("utf-8");
                mail.IsBodyHtml = true;    //是否使用 html 語法
                mail.CC.Add(new MailAddress(""));
                mail.CC.Add(new MailAddress(""));
                mail.CC.Add(new MailAddress(""));
                SmtpClient client = new SmtpClient("mail01.etzone.net");//公司的email Server
                //client.Credentials = new NetworkCredential("Patrick.kuo@ehsn.com.tw", "");
                client.UseDefaultCredentials = true;
                client.Send(mail);    

            }
            catch (ArgumentNullException ex)
            {
                
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
