using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;

using Umbraco.Plugins.Backups.Models;
using Umbraco.Web.WebApi;

namespace development.camtc.org.App_Code
{
    public class BackupController : UmbracoAuthorizedApiController
    {
        public string DestinationPath { get; private set; }
        public string RootPath { get; private set; }
        public string DatabasePath { get; private set; }
        public string FilesPath { get; private set; }
        private readonly SqlConnection sqlConnection;
        private const string backupFolder = "Media";
        private const string files = "Files_{0}.files.zip";
        private const string database = "Database_{0}__{1}.bak";
        private const string TIMESTAMP = "DATE--yyyy-MM-dd---HH-mm-ss";
        private const string MEDIA_VIRTUAL_PATH = "/media/";
        public BackupDetails BackupDetails { get; private set; }
        public BackupController()
        {
            sqlConnection = GetSqlConnection();

            RootPath = HostingEnvironment.MapPath("~/");
            DestinationPath = Path.Combine(RootPath, backupFolder);
            DatabasePath = Path.Combine(DestinationPath, database);
            FilesPath = Path.Combine(DestinationPath, files);

            if (!Directory.Exists(DestinationPath))
            {
                Directory.CreateDirectory(DestinationPath);
            }

            BackupDetails = new BackupDetails
            {
                ConnectionString = sqlConnection?.ConnectionString,
                Database = sqlConnection?.Database,
                Server = sqlConnection?.DataSource,
                DatabasePath = DatabasePath,
                FilesPath = FilesPath,
                RootPath = RootPath,
                DestinationPath = DestinationPath
            };
        }

        [HttpGet]
        public async Task<BackupDetails> GetBackupDetails()
        {
            await Task.FromResult(0);
            return BackupDetails;
        }

        [HttpDelete]
        public async Task<bool> DeleteBackup(string filename)
        {
            var path = Path.Combine(DestinationPath, filename);
            File.Delete(path);
            await Task.FromResult(0);
            return true;
        }

        [HttpGet]
        public async Task<BackupPerformedDetails> GetBackupsPerformed()
        {
            await Task.FromResult(0);
            var backupsPerformed = new BackupPerformedDetails();
            if (Directory.Exists(DestinationPath))
            {
                var files = Directory.GetFiles(DestinationPath, "*.files.zip");
                var dbs = Directory.GetFiles(DestinationPath, "*.bak.zip");

                foreach (var file in files)
                {
                    var d = new FileInfo(file);
                    backupsPerformed.FileBackups.Add(d.Name, new FileDetails
                    {
                        Fullname = $"{MEDIA_VIRTUAL_PATH}{d.Name}",
                        CreateDate = d.CreationTimeUtc
                    });
                }

                foreach (var db in dbs)
                {
                    var d = new FileInfo(db);
                    backupsPerformed.DatabaseBackups.Add(d.Name, new FileDetails
                    {
                        Fullname = $"{MEDIA_VIRTUAL_PATH}{d.Name}",
                        CreateDate = d.CreationTimeUtc
                    });
                }
            }
            return backupsPerformed;
        }

        [HttpPost]
        public async Task<bool> FilesBackup()
        {
            await Task.FromResult(0);

            if (File.Exists(string.Format(FilesPath, DateTime.Now.ToString(TIMESTAMP))))
            {
                File.Delete(string.Format(FilesPath, DateTime.Now.ToString(TIMESTAMP)));
            }
            ZipExtensions.CreateFromDirectory(RootPath, string.Format(FilesPath, DateTime.Now.ToString(TIMESTAMP)), CompressionLevel.Fastest, true, Encoding.UTF8, folderName => !folderName.Contains(@"TEMP"));

            return true;
        }

        [HttpPost]
        public async Task<bool> DatabaseBackup()
        {
            if (sqlConnection is null)
            {
                //Db file will be backed up along with the files
                return true;
            }

            var path = string.Format(DatabasePath, sqlConnection.Database, DateTime.Now.ToString(TIMESTAMP));
            SqlCommand command = new SqlCommand("BACKUP DATABASE [" + sqlConnection.Database + "] TO DISK ='" + path + "';", sqlConnection);
            sqlConnection.Open();
            var result = await command.ExecuteNonQueryAsync();
            sqlConnection.Close();
            if (result == -1)
            {
                var entryName = string.Format(database, sqlConnection.Database, DateTime.Now.ToString(TIMESTAMP));
                using (ZipArchive zip = ZipFile.Open($"{path}.zip", ZipArchiveMode.Create))
                {
                    zip.CreateEntryFromFile(path, entryName);
                    File.Delete(path);
                }
            }
            return true;
        }

        [HttpPost]
        public async Task<Response> FullBackup()
        {
            try
            {
                var success = await FilesBackup();
                success = success && await DatabaseBackup();

                if (success)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.OK,
                        Success = true
                    };
                }
                else
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Details = ex,
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [HttpGet]
        public FileResult DownloadBackupFiles()
        {
            return new FilePathResult(FilesPath, "application/zip, application/octet-stream, application/x-zip-compressed, multipart/x-zip");
        }

        [HttpGet]
        public FileResult DownloadBackupDb()
        {
            return new FilePathResult(DatabasePath, "application/octet-stream");
        }

        #region Helpers
        private string GetConnectionString()
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringSettings connStringSettings = configuration.ConnectionStrings.ConnectionStrings["umbracoDbDSN"];
            return connStringSettings.ConnectionString;
        }

        private SqlConnection GetSqlConnection()
        {
            var connectionString = GetConnectionString();
            if (connectionString.Contains("|DataDirectory|")) return null;
            return new SqlConnection(connectionString);
        }
        #endregion
    }
}
