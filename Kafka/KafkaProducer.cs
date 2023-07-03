using Confluent.Kafka;

namespace WebAPI.Kafka
{
    public class KafkaProducer
    {
        public static IProducer<Null, string>? producer { get; set; }
        public static void SetUpProducer()
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",
                LingerMs = 50,
                MessageSendMaxRetries = 10,
                EnableIdempotence = true,
                RetryBackoffMs = 500,
                MessageMaxBytes = 20000000
            };

            var _producer = new ProducerBuilder<Null, string>(config);
            producer = _producer.Build();
        }
    }
}