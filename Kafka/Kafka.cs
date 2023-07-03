using Confluent.Kafka;
using Newtonsoft.Json;

namespace WebAPI.Kafka
{
    public static class Kafka
    {
        public static async Task KafkaProducerAsync(string topic, KafkaModel message, CancellationToken cancellationToken)
        {
            try
            {
                await KafkaProducer.producer.ProduceAsync(topic, new Message<Null, string> { Value = JsonConvert.SerializeObject(message) });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
