using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace IPSender
{
    public partial class FormMain : Form
    {
        string address = null;
        string newaddress = null;
        string from = null;
        string to = null;
        string pass = null;
        string smtp = null;
        int port = 0;
        int know = 0;
        int send = 0;

        public FormMain()
        {
            InitializeComponent();
            string file = pathAddSlash(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)) + "IPSender.txt";
            if (File.Exists(file))
            {
                List<string> cacheFile = new List<string>();
                try
                {
                    cacheFile.AddRange(File.ReadAllLines(file, new UTF8Encoding(false)));
                    from = cacheFile[0];
                    to = cacheFile[1];
                    pass = cacheFile[2];
                    smtp = cacheFile[3];
                    Int32.TryParse(cacheFile[4], out port);
                }
                catch
                {
                    MessageBox.Show(file + "Файл должен содержать 5 строк: от кого, к кому, пароль (от кого), smtp сервер, порт.");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            startCheck();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            startCheck();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Enabled = false;
            know++;
            label4.Text = know.ToString();
            newaddress = getIPAddress();
            if (newaddress != null)
            {
                compareAdress();
                return;
            }
            timer3.Enabled = true;
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            timer4.Enabled = false;
            send++;
            label5.Text = know.ToString();
            if (sendMail())
            {
                timer2.Enabled = true;
                return;
            }
            timer4.Enabled = true;
        }

        private void startCheck()
        {
            timer2.Enabled = false;
            newaddress = getIPAddress();
            know = 0;
            label4.Text = know.ToString();
            if (newaddress == null)
            {
                timer3.Enabled = true;
                return;
            }
            compareAdress();
        }

        private void compareAdress()
        {
            if (address != newaddress)
            {
                address = newaddress;
                label1.Text = address;
                send = 0;
                label5.Text = know.ToString();
                if (!sendMail())
                {
                    timer4.Enabled = true;
                    return;
                }
            }
            timer2.Enabled = true;
        }

        private bool sendMail()
        {
            try
            {
                MailMessage message = new MailMessage(from, to);
                message.Subject = "IP address";
                message.Body = address;
                SmtpClient client = new SmtpClient(smtp, port);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(from, pass);
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string getIPAddress()
        {
            try
            {
                String address = "";
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                WebResponse response = request.GetResponse();
                StreamReader stream = new StreamReader(response.GetResponseStream());
                address = stream.ReadToEnd();
                int first = address.IndexOf("Address: ") + 9;
                int last = address.LastIndexOf("</body>");
                return address.Substring(first, last - first);
            }
            catch
            {
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            notifyIcon1.Visible = true;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        public static string pathAddSlash(string path)
        {
            if (!path.EndsWith("/") && !path.EndsWith(@"\"))
            {
                if (path.Contains("/"))
                {
                    path += "/";
                }
                else if (path.Contains(@"\"))
                {
                    path += @"\";
                }
            }
            return path;
        }
    }
}
