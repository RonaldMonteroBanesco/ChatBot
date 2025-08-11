using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatBotJob.Interfaces;
using ChatBotJob.Data;
using ChatBotJob.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using RabbitMQ.Client;
using System.Text;

namespace ChatBotJob.Services
{
    public class StockCommandPublisher : IStockCommandPublisher, IDisposable
    {
        private readonly IConnection _conn;
        private readonly IModel _channel;

        public StockCommandPublisher()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare("stockQueue", durable: false, exclusive: false, autoDelete: false);
        }

        public Task PublishAsync(StockCommand cmd, CancellationToken ct = default)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(cmd);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish("", "stockQueue", null, body);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _conn?.Dispose();
        }
    }
}