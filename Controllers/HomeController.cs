using FinancialChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FinancialChat.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinancialChat.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext db)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
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

        [HttpPost]
        public async Task<IActionResult> Create(string text)
        {
            //if (!User.Identity.IsAuthenticated) return Error();
            var sender = await _userManager.GetUserAsync(User);
            var message = new Message {UserName = User.Identity.Name, UserID = sender.Id, Text = text };
            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("/Home/GetMessages")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _db.Messages.OrderBy(m => m.Date).Take(50).ToListAsync();
            return Json(JsonConvert.SerializeObject(messages));
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
    }
}
