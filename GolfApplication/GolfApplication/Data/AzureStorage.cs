using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Data
{
    public class AzureStorage
    {
        public static string ConnString()  //Login userInfo
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var StorageKey = configuration.GetSection("StorageAccount").Value;

            return StorageKey;
        }

        public static async Task<string> UploadImage(IFormFile file, string DocumentName, string Folder)
        {
            string uri = "";

            try
            {
                var AccountKey = ConnString();

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(AccountKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(Folder);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(DocumentName);
                //cloudBlockBlob.Properties.ContentType = ContentType == "jpg" ? "image/jpg" : "video/mp4";
                //byte[] byteArray = Encoding.ASCII.GetBytes(DocumentBytes);
                //MemoryStream stream = new MemoryStream(byteArray);
                var fileStream = file.OpenReadStream();
                //Stream fileStream = new MemoryStream(fileStram);
                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                uri = Convert.ToString(cloudBlockBlob.Uri);
                return uri;
            }

            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
