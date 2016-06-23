using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;
using System.IO;

namespace MailLib
{
    /// <summary>
    /// The helper class for sending mail;
    /// </summary>
    public class MailHelper
    {
        string senderEmail = string.Empty;
        string mailServer = string.Empty;
        string mailPort = string.Empty;    
        string userName = string.Empty;
        string password = string.Empty;

       
        public delegate void _SendCompleted();

        /// <summary>
        /// Gets or Sets the SendCompleted callback function
        /// </summary>
        public _SendCompleted SendCompleted;

        /// <summary>
        /// Gets or Sets attachment file address
        /// </summary>
        public string Attachment
        {
            get;
            set;
        }


        MailMessage message = null;
        SmtpClient client = null;

        /// <summary>
        /// MailHelper Constructor
        /// </summary>
        /// <param name="_senderEmail">Sender email address</param>
        /// <param name="_mailServer">Address of the hosted mail server</param>
        /// <param name="_mailPort">port number of the hosted mail server</param>
        /// <param name="_userName">Sender user name</param>
        /// <param name="_password">Sender password</param>
        public MailHelper(string _senderEmail, string _mailServer, string _mailPort, string _userName, string _password)
        {
            senderEmail = _senderEmail;
            mailServer = _mailServer;
            mailPort = _mailPort;
            userName = _userName;
            password = _password;           
        }

        /// <summary>
        /// Send mail to the user
        /// </summary>
        /// <param name="subject">Subject of the mail</param>
        /// <param name="body">Body of the mail</param>
        /// <param name="recipientEmail">List of Recipient email address</param>
        /// <param name="ccEmail">(Optional)List of CC email address</param>
        /// <param name="bccEmail">(Optional)List of BCC email address</param>
        /// <param name="senderName">(Optional)Sender display name</param>
        /// <param name="recipientName">(Optional)Recipient display name</param>
        public bool SendMail(string subject, string body, List<string> recipientEmail, List<string> ccEmail = null, List<string> bccEmail = null, string senderName = null, string recipientName = null)
        {
            if (validate(subject, body))
            {
                try
                {
                    message = new MailMessage()
                        {
                            From = new MailAddress(senderEmail, senderName),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = false
                        };

                    //Check empty recipient email list
                    if (recipientEmail.Count == 0)
                    {
                        throw new Exception("Recipient list should not be empty.");
                    }

                    //Add recipient email address
                    foreach (string email in recipientEmail)
                    {
                        message.To.Add(new MailAddress(email, recipientName));
                    }

                    //Add CC email address
                    if (ccEmail != null)
                    {
                        foreach (string email in ccEmail)
                        {
                            message.CC.Add(email);                        
                        }
                    }

                    //Add bcc email address
                    if (bccEmail != null)
                    {
                        foreach (string email in bccEmail)
                        {
                            message.Bcc.Add(email);
                        }
                    }

                    //Include attachment
                    if (File.Exists(Attachment))
                    {                        
                        byte[] data = File.ReadAllBytes(Attachment);
                        message.Attachments.Add(new Attachment(new MemoryStream(data), Path.GetFileName(Attachment)));
                    }
                    else
                    {
                        if (Attachment != null)
                            throw new Exception("Attachment file does not exists");
                    }

                    //Define SMTP client
                    client = new SmtpClient(mailServer, int.Parse(mailPort));
                    NetworkCredential credential = new NetworkCredential(userName, password);
                    client.Credentials = credential;
                    //client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 20000;

                    //Handle the Callback
                    client.SendCompleted += SendCompletedCallback;

                    //Send message asynchronously
                    client.SendAsync(message, null);

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return true;
        }

        private bool validate(string sub, string body)
        {
            if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(body))
                throw new Exception("Subject or Body should not be empty.");

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(mailServer) || string.IsNullOrEmpty(mailPort) ||
                string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new Exception("MailHelper constructor argument value should not be empty.");

            return true;
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                throw new Exception("Mail sending cancel.");
            }
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
            else
            {
                try
                {
                    if (SendCompleted != null)
                        SendCompleted();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            message.Dispose();
            client.Dispose();
        }
    }
}
