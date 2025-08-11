using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatBotJob.Models;

namespace ChatBotJob.Interfaces
{
    public interface IStockCommandPublisher
    {
        Task PublishAsync(StockCommand cmd, CancellationToken ct = default);
    }
}