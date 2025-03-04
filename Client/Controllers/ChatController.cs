using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;

namespace Client.Controllers
{
    public partial class ChatController : Controller
    {
        private readonly HttpClient _httpClient;
        public ChatController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet("/Chat/HomeChat")]
        public async Task<IActionResult> HomeChatAsync(string? uuidU, string? uuidF)
        {
           if(uuidU != null && uuidF != null)
            {
                var apiUrl = $"https://localhost:7119/api/ChatMember/GetChatUUID/{uuidU}/{uuidF}";
                HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var chatUuidList = response.Content.ReadFromJsonAsync<List<string>>().Result;
                    var chatUuid = chatUuidList.FirstOrDefault();
                    if(chatUuid == null)
                    {
                        chatUuid = "Default";
                        ViewBag.uuidU = uuidU;
                        ViewBag.uuidF = uuidF;
                    }
                    ViewBag.ChatUuid = chatUuid;
                }
                else
                {
                    ViewBag.Error = "Unable to retrieve chat UUIDs.";
                }
           }           
            return View();
        }
    }
}
