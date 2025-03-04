using System.IO.Pipelines;
using System.Net.Http;
using WebAPI.Constants;
using WebAPI.Authentication;
using WebAPI.Constants;
using WebAPI.Repositories;

namespace WebAPI.Services
{
    public class FileService : BaseService<Models.File, FileRepository>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient httpClient;
        public FileService(FileRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository) {
            _httpContextAccessor = httpContextAccessor;
            httpClient = new HttpClient();
        }

        public byte[] GetDocument(string documentName)
        {
            var documentPath = Path.Combine(FilePath.DOCUMENT_URL, documentName);
            return File.ReadAllBytes(documentPath);
        }
        public byte[] GetImage(string imgName)
        {
            var imgPath = Path.Combine(FilePath.IMAGES_URL, imgName);
            return File.ReadAllBytes(imgPath);
        }
        public async Task<MemoryStream> GetVideo(string videoName)
        {
            var videoPath = Path.Combine(FilePath.VIDEO_URL, videoName);
            var memory = new MemoryStream();
            using (var stream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return memory;
        }

        public async Task<string> CheckFileOfUserAsync(string path, string type)
        {
            var userUUID = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID)?.Value;
            var file = await _repository.GetFileOfUserAsync(userUUID, path, type);
            if (file != null)
            {
                return file.Name;
            }
            return null;
        }

        public static async Task SaveFileFromBase64Async(string base64, string fileName, string type) {
            byte[] bytes = Convert.FromBase64String(base64);
            string path = "";
            if(type == FileType.DOCUMENT) {
                path = Path.Combine(FilePath.DOCUMENT_URL, fileName);
            }else if(type == FileType.VIDEO) {
                path = Path.Combine(FilePath.VIDEO_URL, fileName);
            }else if(type == FileType.IMAGE) {
                path = Path.Combine(FilePath.IMAGES_URL, fileName);
            }
            if(path != "") {
                await File.WriteAllBytesAsync(path, bytes);
            }
        }
        public async Task<List<Models.File>> GetFileByUUID(String UUID)
        {
            //var userUUID = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID)?.Value;

            //Testing
            //userUUID = "3ad5805b-2521-4e90-a2c4-50de632214f0";
            var files = await _repository.GetFileByUUID(UUID);
            return files;
        }

        public async Task<string> DownloadImageAsync(string imageUrl) {
            try {
                using (var response = await httpClient.GetAsync(imageUrl)) {
                    response.EnsureSuccessStatusCode();

                    string contentType = response.Content.Headers.ContentType?.MediaType;
                    string fileExtension = contentType switch {
                        "image/jpeg" => ".jpg",
                        "image/png" => ".png",
                        "image/gif" => ".gif",
                        _ => ".jpg"
                    };
                    string fileName = $"{Guid.NewGuid()}{fileExtension}";
                    string localFilePath = Path.Combine(FilePath.IMAGES_URL, fileName);
                    using (var stream = await response.Content.ReadAsStreamAsync()) {
                        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write)) {
                            await stream.CopyToAsync(fileStream);
                        }
                    }

                    return fileName;
                }
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}

