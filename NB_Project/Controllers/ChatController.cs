﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NB_Project.ApplicationDbContext;
using NB_Project.Services;

namespace NB_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ChatController : ControllerBase
    {

        private readonly DB _db;

        public ChatController(DB db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet("chat/{otherUserId}")]
        public async Task<IActionResult> GetChatWithUser(string otherUserId)
        {
            var currentUserId = User.FindFirst("id")?.Value;

            var messages = await _db.chatMessages
                .Where(c =>
                    (c.SenderId == currentUserId && c.ReceiverId == otherUserId) ||
                    (c.SenderId == otherUserId && c.ReceiverId == currentUserId))
                .OrderBy(c => c.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }


        [Authorize]
        [HttpGet("Gat-All-Chats")]
        public async Task<IActionResult> GetAllChats()
        {
            try
            {
                var chatList = await _db.chatSummaries.ToListAsync();

                return Ok(chatList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
