using FinancialChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FinancialChat.Hubs;
using FinancialChat.RabbitMQ;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Bot;
using FinancialChat.DB;

namespace FinancialChat.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RabbitMQService _rabbbitMQService;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly BotService _bot;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, RabbitMQService rabbitMQService, IHubContext<ChatHub> chatHub, BotService bot)
        {
            _logger = logger;
            _userManager = userManager;
            _rabbbitMQService = rabbitMQService;
            _chatHub = chatHub;
            _bot = bot;
            _rabbbitMQService.MessageConsumed += ShowBotMessageToChat;
            _bot.NotCommand += SaveMessageInDataBase;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserName = currentUser.UserName;
                ViewBag.UserId = currentUser.Id;
            }
            return View();
        }

        [HttpGet]
        [Route("/Home/GetMessages")]
        public async Task<IActionResult> GetMessages()
        {
            List<Message> messages = null;
            try
            {
                messages = await DbService.GetMessages();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
            return Json(JsonConvert.SerializeObject(messages));
        }

        [HttpPost("/Home/Bot")]
        public IActionResult CheckChatMessage(string text)
        {
            _bot.CheckStocksCommand(text);
            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async void ShowBotMessageToChat(object sender, RabbitMQServiceEventArgs e)
        {
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", new Message { Date = e.Date, Text = e.Text, UserID = "Bot", UserName = "Bot" });
        }

        private void SaveMessageInDataBase(object sender, NotCommandEventArgs e)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<Message>(e.Text);
                DbService.SaveMessage(message).Wait();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }
    }
}
