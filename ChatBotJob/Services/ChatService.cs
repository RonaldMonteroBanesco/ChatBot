using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatBotJob.Interfaces;
using ChatBotJob.Data;
using ChatBotJob.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.AspNetCore.Identity;
namespace ChatBotJob.Services
{
    public class ChatService : IChatService
    {
    private readonly IServiceProvider _sp;
    private readonly IStockCommandPublisher _publisher;
    
    private readonly UserManager<IdentityUser> _userManager;

    public ChatService(IServiceProvider sp, IStockCommandPublisher publisher, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _sp = sp;
            _publisher = publisher;
        }

        public async Task<(bool queued, string info)> SendAsync(string username, string text, string roomId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text)) return (false, "Empty message");
            if (string.IsNullOrWhiteSpace(roomId)) roomId = "general";

            // /stock= handling → publish JSON payload { stockCode, roomId }
            if (text.StartsWith("/stock=", StringComparison.OrdinalIgnoreCase))
            {
                var parts = text.Split('=', 2);
                var code = parts.Length == 2 ? parts[1].Trim() : "";
                if (string.IsNullOrWhiteSpace(code)) return (false, "Invalid stock command");

                await _publisher.PublishAsync(new StockCommand { StockCode = code, RoomId = roomId }, ct);
                return (true, "Stock command queued");
            }


            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            string userId = null;
            var user = await _userManager.FindByNameAsync(username);
            if (user != null) userId = user.Id;

            db.ChatMessages.Add(new ChatMessage
            {
                UserId = userId,
                Username = username,
                Message = text,
                IsBot = false,
                Timestamp = DateTime.UtcNow,
                RoomId = roomId
            });

            await db.SaveChangesAsync(ct);
            return (false, "Message saved");
        }

        public async Task<List<ChatMessage>> LatestAsync(string roomId, int take = 50, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(roomId)) roomId = "general";

            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await db.ChatMessages
                .Where(m => m.RoomId == roomId)
                .OrderByDescending(m => m.Timestamp)
                .Take(take)
                .OrderBy(m => m.Timestamp)
                .ToListAsync(ct);
        }
    }
}