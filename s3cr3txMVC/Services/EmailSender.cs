using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Data;

namespace s3cr3txMVC.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendKey, subject, message, email);
        }
        public Task Execute(string apiKey, string subject, string message, string email)
        {
            string UserState = "EmailMessage TO: " + email + " WITH SUBJECT: " + subject + " AND BODY: " + message + " FROM: " + Options.SendUser;
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Port = 587;
                string result = GetResult(@"w6PFveKAlMK2XMOcwrc1QMKlYMK2RQnCu8ONwrNZasW+w4wlIBVhN+KAmsO2wqbDi8KsdRNzw7cYwrRWwqXDvSjDk0lqV8KtNTXCqjTCncKyOsOD4oCcQTHDhwXigKDCp8OkLEk=", @"JBgEw6R2BFDDtBfigqwhUEF9TeKAkyjigJzCo8KdV8O14oCYw7PDl8Ouw75z4oSiX3bCtlzDnHk74oSiwrfFvWfDqcOzwrPDmsKyw5HDicWTdcOxwrHCtsWSShbDnQYyw4BM4oC6w7RLw4c=", @"e8999b8c-117f-43a3-a805-b286ffa65c6f", apiKey, false, false);
                smtpClient.Credentials = new NetworkCredential(Options.SendUser, result);
                smtpClient.EnableSsl = true;
                return smtpClient.SendMailAsync(Options.SendUser, email, subject, message);
            }
            catch(Exception ex)
            {
                LogMessage(ex.GetBaseException().ToString() + Environment.NewLine + UserState, @"s3cr3tx.Services.EmailSender.Execute()");
                return null;
            }

        }
        //static bool mailSent = false;
        //private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        //{
        //    // Get the unique identifier for this asynchronous operation.
        //    String token = (string)e.UserState;

        //    if (e.Cancelled)
        //    {
        //        LogMessage("Send canceled.", token);
        //    }
        //    if (e.Error != null)
        //    {
        //        LogMessage(e.Error.ToString(), token );
        //    }
        //    else
        //    {
        //        Console.WriteLine("Message sent.",token);
        //    }
        //    mailSent = true;
        //}
        private static void LogMessage(string message, string source)
        {
            SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=s3cr3tx;Integrated Security=SSPI");
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"dbo.insertLog";
            SqlParameter p1 = new SqlParameter(@"Source", source);
            SqlParameter p2 = new SqlParameter(@"Message", message);
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            con.Open();
            command.Connection = con;
            int intResult = command.ExecuteNonQuery();
        }
        private static string GetResult(string strAuth, string strToken, string strEmail, string strInput, bool blnEoD, bool blnDefault)
        {
            try
            {
                string strResult = @"";
                string strConnection = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=s3cr3tx";
                SqlConnection sql = new SqlConnection(strConnection);
                SqlCommand command = new SqlCommand();
                command.CommandText = @"dbo.EorD";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter p1 = new SqlParameter(@"Auth", strAuth);
                SqlParameter p2 = new SqlParameter(@"APIToken", strToken);
                SqlParameter p3 = new SqlParameter(@"email", strEmail);
                SqlParameter p4 = new SqlParameter(@"input", strInput);
                SqlParameter p5 = new SqlParameter(@"EorD", blnEoD);
                SqlParameter p6 = new SqlParameter(@"blnDefault", blnDefault);
                SqlParameter p7 = new SqlParameter(@"strOutput", System.Data.SqlDbType.NVarChar, -1);
                p7.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(p1);
                command.Parameters.Add(p2);
                command.Parameters.Add(p3);
                command.Parameters.Add(p4);
                command.Parameters.Add(p5);
                command.Parameters.Add(p6);
                command.Parameters.Add(p7);
                using (sql)
                {
                    sql.Open();
                    command.Connection = sql;
                    int result = command.ExecuteNonQuery();
                }
                strResult = (string)p7.Value;
                return strResult;
            }
            catch(Exception ex)
            {
                LogMessage(ex.GetBaseException().ToString(), @"s3cr3tx.Services.EmailSender.GetResult()");
                return @"";
            }
        }
    }
}
