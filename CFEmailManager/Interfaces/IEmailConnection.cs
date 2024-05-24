using System;
using System.Threading;

namespace CFEmailManager.Interfaces
{
    /// <summary>
    /// Email server connection
    /// </summary>
    public interface IEmailConnection
    {
        string ServerType { get; }

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
