using CFEmailManager.Model;
using System;
using System.Threading.Tasks;

namespace CFEmailManager.Interfaces
{
    /// <summary>
    /// Email downloader interface
    /// </summary>
    public interface IEmailDownloader
    {
        Task DownloadEmailsAsync(EmailAccount emailAccount,
                        IEmailRepository emailRepository,
                        Action<string> actionFolderStart,
                        Action<string> actionFolderEnd,
                        Action downloadStart,
                        Action downloadEnd);

        void Cancel();
    }
}
