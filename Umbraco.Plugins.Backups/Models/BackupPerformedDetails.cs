using System;
using System.Collections.Generic;

namespace Umbraco.Plugins.Backups.Models
{
    public class BackupPerformedDetails
    {
        public BackupPerformedDetails()
        {
            FileBackups = new Dictionary<string, FileDetails>();
            DatabaseBackups = new Dictionary<string, FileDetails>();
        }
        public Dictionary<string, FileDetails> FileBackups { get; set; }
        public Dictionary<string, FileDetails> DatabaseBackups { get; set; }
    }

    public class FileDetails
    {
        public string Fullname { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
