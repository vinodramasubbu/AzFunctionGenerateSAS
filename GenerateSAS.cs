using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace GenerateSAS.Function
{
    public static class GenerateSAS
    {
        [FunctionName("GenerateSAS")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)

        {

            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=xxxxxxx;AccountKey=xxxxxxxx;EndpointSuffix=core.windows.net";
            string containerName = "mylargefiles";
            Guid myuuid = Guid.NewGuid();
            string blobName = myuuid.ToString();
   
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                

                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b", // "b" indicates a blob resource
                    StartsOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddHours(1), // Set the desired expiry time
                    Protocol = SasProtocol.Https // Use HTTPS protocol for enhanced security
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Write);

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);       
                return new OkObjectResult(sasUri);
        }
    }
}
