namespace CFEmailManager.Model
{
    public class EmailDownloadStatistics
    {       
        public int CountEmailsDownloadSuccess { get; set; }

        public int CountEmailsDownloadError { get; set; }

        public void AppendFrom(EmailDownloadStatistics emailDownloadStatistics)
        {
            CountEmailsDownloadSuccess += emailDownloadStatistics.CountEmailsDownloadSuccess;
            CountEmailsDownloadError += emailDownloadStatistics.CountEmailsDownloadError;
        }
    }
}
