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
using RabbitMQ.Client.Events;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace ChatBotJob.Services
{
    public class ChatQueueConsumer : BackgroundService
    {
        private readonly IServiceProvider _sp;
        public ChatQueueConsumer(IServiceProvider sp) => _sp = sp;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            channel.QueueDeclare(queue: "chatQueue", durable: false, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var reply = System.Text.Json.JsonSerializer.Deserialize<BotReply>(json);
                    var roomId = string.IsNullOrWhiteSpace(reply.RoomId) ? "general" : reply.RoomId;

                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    db.ChatMessages.Add(new ChatMessage
                    {
                        UserId = null,          
                        Username = "bot",
                        Message = reply.Message,
                        IsBot = true,
                        Timestamp = DateTime.UtcNow,
                        RoomId = roomId
                    });

                    await db.SaveChangesAsync(stoppingToken);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };
            channel.BasicConsume(queue: "chatQueue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}