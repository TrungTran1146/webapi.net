using Confluent.Kafka;
using Newtonsoft.Json;
namespace WebAPI.Kafka
{
    public class KafkaConsumer
    {
        public int id { get; set; }
        public ConsumerConfig config { get; set; }
        public string topic { get; set; }
        public void Listen()
        {
            var consumeTask = Task.Run(() =>
            {
                //đối tượng `ConsumerBuilder` để xây dựng một đối tượng consumer mới,
                //sử dụng `config` đã được thiết lập trước đó và cài đặt chế độ không
                //phản hồi để theo dõi các yêu cầu hủy bỏ từ người dùng.
                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, e) =>
                    {
                        e.Cancel = true;
                        cts.Cancel();
                    };

                    //phương thức `Subscribe()` để đăng ký chủ đề mà bạn muốn nghe thông
                    //điệp và tạo một vòng lặp lắng nghe các thông điệp.
                    consumer.Subscribe(topic);
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                //phương thức `Consume()` để nhận thông điệp từ chủ đề Kafka
                                //và lưu trữ nó trong một biến tạm thời `consumeResult`
                                var consumeResult = consumer.Consume(cts.Token);
                                var message = consumeResult.Message.Value;
                                Console.WriteLine("Consumer");
                                EventHandler(message);

                                //bạn lưu trữ vị trí của thông điệp bằng cách gọi
                                //phương thức `StoreOffset()` để đảm bảo rằng consumer sẽ không nhận thông điệp cũ.
                                consumer.StoreOffset(consumeResult);
                            }
                            catch (ConsumeException ex)
                            {

                            }
                        }
                    }
                    catch (OperationCanceledException op)
                    {
                        consumer.Close();
                        cts.Cancel();
                    }
                }
            });
        }

        public static void EventHandler(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                try
                {
                    var kafkaModel = JsonConvert.DeserializeObject<KafkaModel>(message);
                    if (kafkaModel != null)
                    {
                        var kafkaWorkflow = new KafkaWorkflow();
                        if (kafkaModel.Topic == TopicList.Create)
                        {
                            kafkaWorkflow.CreateProduct(kafkaModel.Data);
                        }

                        if (kafkaModel.Topic == TopicList.Update)
                        {

                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }

        }

    }
}