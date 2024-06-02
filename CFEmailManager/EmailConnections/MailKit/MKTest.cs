//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using MailKit;
//using MailKit.Net.Imap;
//using MailKit.Search;
//using MailKit.Security;

//namespace CFEmailManager.EmailConnections.MailKit
//{
//    public class MKTest
//    {
//        public void Test()
//        {
//            using (var client = new ImapClient(new ProtocolLogger("imap.log")))
//            {
//                //client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
//                client.Connect("imap-mail.outlook.com", 993, SecureSocketOptions.SslOnConnect);

//                client.Authenticate("myemail@hotmail.co.uk", "");

//                client.Inbox.Open(FolderAccess.ReadOnly);

//                System.Diagnostics.Debug.WriteLine("Personal namespaces:");
//                foreach (var ns in client.PersonalNamespaces)
//                    System.Diagnostics.Debug.WriteLine($"* \"{ns.Path}\" \"{ns.DirectorySeparator}\"");

//                System.Diagnostics.Debug.WriteLine("Shared namespaces:");
//                foreach (var ns in client.SharedNamespaces)
//                    System.Diagnostics.Debug.WriteLine($"* \"{ns.Path}\" \"{ns.DirectorySeparator}\"");
//                System.Diagnostics.Debug.WriteLine("Other namespaces:");
//                foreach (var ns in client.OtherNamespaces)
//                    System.Diagnostics.Debug.WriteLine($"* \"{ns.Path}\" \"{ns.DirectorySeparator}\"");

//                // get the folder that represents the first personal namespace
//                var rootFolder = client.GetFolder(client.PersonalNamespaces[0]);

//                // list the folders under the first personal namespace
//                var folders = rootFolder.GetSubfolders();
//                foreach (var folder in folders)
//                {
//                    for (int messageIndex = 0; messageIndex < folder.Count; messageIndex++)
//                    {
//                        var message = folder.GetMessage(messageIndex);
//                        int xxx = 1000;
//                    }
//                }

//                /*
//                var folders = client.GetFolders(client.PersonalNamespaces[0]);

//                foreach(var folder in folders)
//                {
//                    System.Diagnostics.Debug.WriteLine($"{folder.Name} - {folder.FullName}");
//                }
//                */

//                /*
//                var uids = client.Inbox.Search(SearchQuery.All);

//                foreach (var uid in uids)
//                {
//                    var message = client.Inbox.GetMessage(uid);

//                    // write the message to a file
//                    message.WriteTo(string.Format("{0}.eml", uid));
//                }
//                */

//                client.Disconnect(true);
//            }
//        }
//    }
//}
