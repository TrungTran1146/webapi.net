using Confluent.Kafka;

namespace WebAPI.Kafka
{
    public class KafkaService
    {
        public ProducerConfig? Config { get; set; }
        public KafkaConsumer[]? Consumers { get; set; }
        public KafkaService()
        {
        }
        public void Start()
        {
            var listTopic = new List<string>
            {
                TopicList.Create,
                TopicList.Update,
            };


            Consumers = new KafkaConsumer[listTopic.Count];
            for (int j = 0; j < listTopic.Count; j++) // listtopic.count = 2
            {
                ConsumerConfig config = new ConsumerConfig
                {
                    GroupId = listTopic[j],
                    BootstrapServers = "localhost:9092",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoOffsetStore = false

                };

                Consumers[j] = new KafkaConsumer();
                Consumers[j].config = config;
                Consumers[j].topic = listTopic[j];
                Consumers[j].id = j;
                Consumers[j].Listen();
            }
        }
    }
}
