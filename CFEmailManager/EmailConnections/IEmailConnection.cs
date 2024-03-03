using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CFEmailManager.EmailConnections
{
    /// <summary>
    /// Email server connection
    /// </summary>
    internal interface IEmailConnection
    {
        /// <summary>
        /// Downloads emails to local
        /// </summary>
        /// <param name="server"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="downloadFolder"></param>
        /// <param name="downloadAttachments"></param>
        /// <param name="emailRepository"></param>
        /// <param name="folderStartAction"></param>
        /// <param name="folderEndAction"></param>
        void Download(string server, string username, string password, string downloadFolder, bool downloadAttachments, IEmailRepository emailRepository,
                     CancellationToken cancellationToken,
                     Action<string> folderStartAction = null, Action<string> folderEndAction = null);
    }
}
