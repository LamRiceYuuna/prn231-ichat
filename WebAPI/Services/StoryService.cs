using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Repositories;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebAPI.Authentication;

namespace WebAPI.Services
{
    public class StoryService : BaseService<Story, StoryRepository>
    {
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtHandler _jwtHandler;
        public StoryService(StoryRepository repository, JwtHandler jwtHandler, IHttpContextAccessor httpContextAccessor) : base(repository) {
            _jwtHandler = jwtHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Story> DeleteStory(int id)
        {
            try
            {
                var story = await _repository.DeleteStory(id);
                if (story != null)
                {
                    await DeleteAsync(id);
                }
                return story;
            }catch( Exception ex)
            {
                throw new Exception("An error occurred while deleting the story.", ex);
            }
        }
        public async Task<Story> CreateStoryAsync(IFormFile file,long  userId)
        {
            try {
            
            if (file == null)
            {
                throw new ArgumentNullException("Invalid story data or file");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException("File size exceeds the maximum limit of 10MB.");
            }

            string type = GetFileType(file);
            string fileName = $"{Guid.NewGuid()}{GetFileExtension(file)}";
            string savedFileName = await SaveFile(file, fileName, type);

            // Create a Story object to save in the database
            var story = new Story
            {
                Status = Status.ACCEPTED,
                UserId = userId,
                Type = type,
                Path = savedFileName
            };

            await AddAsync(story);

            return story;
            }
            catch (Exception ex)
            {
                throw new Exception("Error ", ex);
            }
        }

        private async Task<string> SaveFile(IFormFile file, string fileName, string type)
        {
            try
            {
                string filePath;
                if (type == "image")
                {
                    filePath = Path.Combine(FilePath.IMAGES_URL, fileName);
                }
                else if (type == "video")
                {
                    filePath = Path.Combine(FilePath.VIDEO_URL, fileName);
                }
                else
                {
                    throw new ArgumentException("Unsupported file type");
                }

                // Using FileStream to write the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving file", ex);
            }
        }

        private string GetFileType(IFormFile file)
        {
            if (file.ContentType.StartsWith("image/"))
            {
                return "image";
            }
            else if (file.ContentType.StartsWith("video/"))
            {
                return "video";
            }
            else
            {
                throw new ArgumentException("Unsupported file format. Please upload an image or video.");
            }
        }

        private string GetFileExtension(IFormFile file)
        {
            return Path.GetExtension(file.FileName);
        }

        public async Task<List<Story>> GetStoryByUUID(string uUID)
        {
            var storyList = await _repository.GetStoryByUUID(uUID);
            return storyList;
        }
    }  
}
