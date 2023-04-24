using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Web.Hosting;
using System.Web.Mvc;

using Umbraco.Web.Mvc;

namespace development.camtc.org.App_Code
{
    public class BackupController : SurfaceController
    {
        private readonly string path;
        private readonly string destinationZip;
        private readonly string destinationDb;
        private const string DatabaseNameLocal = "camtc";
        private const string DatabaseNameDev = "CAMTD-DEV";
        private const string DatabaseName = "D-Dev";
        public BackupController()
        {
            path = HostingEnvironment.MapPath("~/");
            destinationZip = Path.Combine(path, "../Backup/BackupFiles.zip");
            destinationDb = Path.Combine(path, "../Backup/BackupDb.bak");
            if (!Directory.Exists(Path.Combine(path, "../Backup/")))
            {
                Directory.CreateDirectory(Path.Combine(path, "../Backup/"));
            }
        }

        public void FullBackup()
        {
            if (System.IO.File.Exists(destinationZip))
            {
                System.IO.File.Delete(destinationZip);
            }
            ZipFile.CreateFromDirectory(path, destinationZip);
            var conn = ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString;

            SqlConnection Connection = new SqlConnection(conn);
            SqlCommand command;

            command = new SqlCommand("backup database " + DatabaseNameDev + " to disk ='" + destinationDb + "' with init,stats=10", Connection);
            Connection.Open();
            command.ExecuteNonQuery();
            Connection.Close();
        }

        public FileResult BackupFiles()
        {
            return new FilePathResult(destinationZip, "application/zip, application/octet-stream, application/x-zip-compressed, multipart/x-zip");
        }
        public FileResult BackupDb()
        {
            return new FilePathResult(destinationDb, "application/octet-stream");
        }
    }
}
