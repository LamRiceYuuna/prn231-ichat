using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Web;
using WebAPI.Constants;
using WebAPI.DTOs.Files;
using WebAPI.DTOs.Users;
using WebAPI.Hubs;
using WebAPI.DTOs.Chats;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;
        private readonly IMapper mapper;

        public FileController(FileService fileService, IMapper mapper) 
        {
            _fileService = fileService;
            this.mapper = mapper;
        }

        [HttpGet("document/{path}/download")]
        public async Task<IActionResult> DownLoadDocumentInchat(string path) 
        {
            //string documentName = await _fileService.CheckFileOfUserAsync(path, FileType.DOCUMENT);
            //if(documentName == null) { 
            //    return NotFound();
            //}
            var fileBytes = _fileService.GetDocument(path);
            return File(fileBytes, "application/octet-stream", path);
        }

        [HttpGet("image/{path}")]
        public async Task<IActionResult> GetImageInChat(string path) {
            //string imgName = await _fileService.CheckFileOfUserAsync(path, FileType.IMAGE);
            //if (imgName == null) {
            //    return NotFound();
            //}
            var imageBytes = _fileService.GetImage(path);
            return File(imageBytes, "image/jpeg");
        }

        [HttpGet("video/{path}")]
        public async Task<IActionResult> GetVideoInChat(string path) {
            //string videoName = await _fileService.CheckFileOfUserAsync(path, FileType.VIDEO);
            //if (videoName == null) {
            //    return NotFound();
            //};
            var memory = await _fileService.GetVideo(path);
            return File(memory, "video/mp4", enableRangeProcessing: true);
        }

        [HttpGet("GetFileByUUID/{UUID}")]
        public async Task<IActionResult> GetFileByUUID(String UUID)
        {
            var file = await _fileService.GetFileByUUID(UUID);
            var fileDto = mapper.Map<List<FileResponseDto>>(file);

            fileDto.ForEach(fileResponse => {
                if (fileResponse.Type == FileType.IMAGE)
                {
                    fileResponse.Path = Url.Action("GetImageInChat", "File", new { path = fileResponse.Path }, Request.Scheme);
                }
                else if (fileResponse.Type == FileType.VIDEO)
                {
                    fileResponse.Path = Url.Action("GetVideoInChat", "File", new { path = fileResponse.Path }, Request.Scheme);
                }
                else
                {
                    fileResponse.Path = Url.Action("DownLoadDocumentInchat", "File", new { path = fileResponse.Path }, Request.Scheme);
                }
            });

            return Ok(fileDto);
        }
    }
}
