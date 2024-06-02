using CFEmailManager.Model;
using System;
using System.Threading.Tasks;

namespace CFEmailManager.Interfaces
{
    /// <summary>
    /// Email downloader interface
    /// </summary>
    public interface IEmailDownloaderService
    {
        Task DownloadEmailsAsync(EmailAccount emailAccount,
                        IEmailStorageService emailRepository,
                        bool downloadAttachments,
                        Action<string> actionFolderStart,
                        Action<string> actionFolderEnd,
                        Action downloadStart,
                        Action downloadEnd);

        void Cancel();
    }
}
