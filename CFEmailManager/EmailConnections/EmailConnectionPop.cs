using EAGetMail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CFEmailManager.EmailConnections
{
    /// <summary>
    /// Email downloader via POP3
    /// </summary>
    internal class EmailConnectionPop : EmailConnectionBase, IEmailConnection
    {
        public void Download(string server, string username, string password, string downloadFolder,
                            bool downloadAttachments, IEmailRepository emailRepository,
                             CancellationToken cancellationToken,
                            Action<string> folderStartAction = null, Action<string> folderEndAction = null)
        {
            Directory.CreateDirectory(downloadFolder);

            // Hotmail/MSN POP3 server is "pop3.live.com"
            MailServer oServer = new MailServer(server,
                        username,
                        password,
                        ServerProtocol.Pop3);

            // Enable SSL connection.
            oServer.SSLConnection = true;

            // Set 995 SSL port
            oServer.Port = 995;

            MailClient oClient = new MailClient("TryIt");
            oClient.Connect(oServer);

            MailInfo[] infos = oClient.GetMailInfos();
            Console.WriteLine("Total {0} email(s)\r\n", infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                MailInfo info = infos[i];
                Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                    info.Index, info.Size, info.UIDL);

                // download email from  Hotmail/MSN server
                Mail oMail = oClient.GetMail(info);

                Console.WriteLine("From: {0}", oMail.From.ToString());
                Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                // Generate an unqiue email file name based on date time.
                //string fileName = _generateFileName(i + 1);
                //string fullPath = string.Format("{0}\\{1}", localInbox, fileName);

                string filename = string.Format("{0}.xml", Guid.NewGuid().ToString());
                string emailPath = Path.Combine(downloadFolder, filename);
                oMail.SaveAs(emailPath, true);

                // Save email to local disk
                //oMail.SaveAs(fullPath, true);                

                // Mark email as deleted from POP3 server.
                //oClient.Delete(info);
            }

            // Quit and expunge emails marked as deleted from POP3 server.
            oClient.Quit();

        }
    }
}
