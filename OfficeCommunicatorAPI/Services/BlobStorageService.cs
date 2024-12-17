using Azure.Storage.Blobs;

namespace OfficeCommunicatorAPI.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobStorageService(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string uniqueName)
        {
            var blobClient = _containerClient.GetBlobClient(uniqueName);
            await blobClient.UploadAsync(file.OpenReadStream(), true);
            return blobClient.Uri.AbsoluteUri;
        }


        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }


        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            return await blobClient.DeleteIfExistsAsync();
        }

        public string GenerateFileName(string realName, int messegeId)
        {
            return $"{Guid.NewGuid}_{realName}";
        }

        public string GetRealFileName(string fileName)
        {
            var index = fileName.IndexOf('_');
            if (index == -1) return fileName;
            return fileName[(index + 1)..];
        }
    }
}
