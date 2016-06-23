using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailLib;

namespace SendMailTestAPP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                //MailHelper mailHelper = new MailHelper("monowar.mbstu@gmail.com", "smtp.gmail.com", "587", "monowar.mbstu@gmail.com", "");

                MailHelper mailHelper = new MailHelper("m.monowarul@hawarit.com", "mail.hawarit.com", "587", "m.monowarul@hawarit.com", "");

                List<string> recipientEmailList = new List<string>();
                recipientEmailList.Add("m.monowarul@hawarit.com");
                recipientEmailList.Add("monowar.mbstu@gmail.com");

                List<string> ccEmailList = new List<string>();
                ccEmailList.Add("j.islam@hawarit.com");
                ccEmailList.Add("r.rahman@hawarit.com");

                mailHelper.SendCompleted += SendCompletedCallBack;

                //mailHelper.Attachment = @"C:\Users\HDSL115\Desktop\hello.txt";

                bool success = mailHelper.SendMail("Test mail", "This is the mail body", recipientEmailList, senderName: "Monowar", recipientName: "abc");
                
                if (success)
                {
                    MessageBox.Show("Send");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendCompletedCallBack()
        {
            MessageBox.Show("Send Completed");
        }
    }
}
