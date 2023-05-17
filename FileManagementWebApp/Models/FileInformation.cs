namespace FileManagementWebApp.Models
{
    public class FileInformation
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public TimeSpan ArchivingTime { get; set; }
        public string Status { get; set; }
        public string ArchiveFilePath { get; set; }
    }
}
