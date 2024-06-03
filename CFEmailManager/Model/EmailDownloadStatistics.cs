namespace CFEmailManager.Model
{
    public class EmailDownloadStatistics
    {       
        public int CountEmailsDownloaded { get; set; }

        public void AppendFrom(EmailDownloadStatistics emailDownloadStatistics)
        {
            CountEmailsDownloaded += emailDownloadStatistics.CountEmailsDownloaded;
        }
    }
}
