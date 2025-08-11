using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatBotJob.Data;
using ChatBotJob.Models;
using System.Threading;
namespace ChatBotJob.Interfaces
{
    public interface IChatService
    {
        Task<(bool queued, string info)> SendAsync(string username, string text, string roomId, CancellationToken ct = default);
        Task<List<ChatMessage>> LatestAsync(string roomId, int take = 50, CancellationToken ct = default);
    }
}