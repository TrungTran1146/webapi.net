using Confluent.Kafka;
using Newtonsoft.Json;
using WebAPI.Model;
using WebAPI.Redis;

namespace WebAPI.Kafka
{
    public class KafkaConsumer
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumer(string bootstrapServers, string groupId, IServiceProvider serviceProvider)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _serviceProvider = serviceProvider;
        }

        public void Subscribe(string topic)
        {
            _consumer.Subscribe(topic);
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    var message = consumeResult.Message.Value;
                    Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {message}");

                    // Deserialize the message to cart object
                    var cart = JsonConvert.DeserializeObject<Cart>(message);

                    // Handle the message
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                        var redisCache = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();

                        if (cart.Id == 0)
                        {
                            // Add new cart to database
                            dbContext.Carts.Add(cart);
                            await dbContext.SaveChangesAsync();

                            // Add new cart to Redis cache
                            var cacheKey = $"cart:{cart.Id}";
                            var cacheValue = JsonConvert.SerializeObject(cart);
                            var expirationTime = DateTimeOffset.Now.AddMinutes(5);
                            redisCache.SetData(cacheKey, cacheValue, expirationTime);
                        }
                        else
                        {
                            // Update existing cart in database
                            var existingcart = await dbContext.Carts.FindAsync(cart.Id);

                            if (existingcart == null)
                            {
                                Console.WriteLine($"cart with id {cart.Id} not found");
                            }
                            else
                            {
                                //existingcart.Status = cart.Status;
                                existingcart.Quantity = cart.Quantity;
                                //existingcart.Price = cart.Price;

                                await dbContext.SaveChangesAsync();

                                // Update existing cart in Redis cache
                                var cacheKey = $"cart:{cart.Id}";
                                var cacheValue = JsonConvert.SerializeObject(existingcart);
                                var expirationTime = DateTimeOffset.Now.AddMinutes(5);
                                redisCache.SetData(cacheKey, cacheValue, expirationTime);
                            }
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error while consuming message: {e.Error.Reason}");
                }
            }
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}