using CFEmailManager.Model;
using System;
using System.Collections.Generic;
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
        /// Downloads emails to email storage
        /// </summary>
        /// <param name="server"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>        
        /// <param name="downloadAttachments"></param>
        /// <param name="emailRepository"></param>
        /// <param name="folderStartAction"></param>
        /// <param name="folderEndAction"></param>
        EmailDownloadStatistics Download(string server, string username, string password, 
                     bool downloadAttachments,
                     List<string> topLevelFoldersToIgnore,
                     IEmailStorageService emailStorageService,
                     CancellationToken cancellationToken,
                     Action<string> folderStartAction = null, Action<string> folderEndAction = null);
    }
}
