using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
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
        private readonly string path;
        private readonly string destinationZip;
        private readonly string destinationDb;
        private readonly SqlConnection sqlConnection;
        public SqlConnectionDetails SqlConnectionDetails { get; private set; }
        public BackupController()
        {
            sqlConnection = GetSqlConnection();
            SqlConnectionDetails = new SqlConnectionDetails
            {
                ConnectionString = sqlConnection.ConnectionString,
                Database = sqlConnection.Database,
                Server = sqlConnection.DataSource
            };
            path = HostingEnvironment.MapPath("~/");
            destinationZip = Path.Combine(path, "../Backup/BackupFiles.zip");
            destinationDb = Path.Combine(path, "../Backup/BackupDb.bak");
            if (!Directory.Exists(Path.Combine(path, "../Backup/")))
            {
                Directory.CreateDirectory(Path.Combine(path, "../Backup/"));
            }
        }

        [HttpGet]
        public async Task<SqlConnectionDetails> GetBackupDetails()
        {
            await Task.FromResult(0);
            return SqlConnectionDetails;
        }

        [HttpPost]
        public async Task<bool> FilesBackup()
        {
            await Task.FromResult(0);

            if (File.Exists(destinationZip))
            {
                File.Delete(destinationZip);
            }
            ZipFile.CreateFromDirectory(path, destinationZip);

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

            SqlCommand command = new SqlCommand("backup database " + sqlConnection.Database + " to disk ='" + destinationDb + "' with init,stats=10", sqlConnection);
            sqlConnection.Open();
            command.ExecuteNonQuery();
            sqlConnection.Close();
            await Task.FromResult(0);
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
            return new FilePathResult(destinationZip, "application/zip, application/octet-stream, application/x-zip-compressed, multipart/x-zip");
        }

        [HttpGet]
        public FileResult DownloadBackupDb()
        {
            return new FilePathResult(destinationDb, "application/octet-stream");
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
