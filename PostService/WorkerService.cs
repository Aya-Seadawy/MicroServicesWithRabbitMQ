using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PostService.Data;
using PostService.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PostService;

public class WorkerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ListenForIntegrationEvents();
    }

    public static void ListenForIntegrationEvents()
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var contextOptions = new DbContextOptionsBuilder<PostServiceContext>()
                .UseSqlite(@"Data Source=post.db")
                .Options;
            var dbContext = new PostServiceContext(contextOptions);

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);

            var data = JObject.Parse(message);
            var type = ea.RoutingKey;
            if (type == "user.add")
            {
                dbContext.User.Add(new User()
                {
                    ID = data["id"].Value<int>(),
                    Name = data["name"].Value<string>()
                });
                dbContext.SaveChanges();
            }
            else if (type == "user.update")
            {
                var user = dbContext.User.First(a => a.ID == data["id"].Value<int>());
                user.Name = data["newname"].Value<string>();
                dbContext.SaveChanges();
            }
        };
        channel.BasicConsume(queue: "user.postservice",
                                 autoAck: true,
                                 consumer: consumer);
    }
}