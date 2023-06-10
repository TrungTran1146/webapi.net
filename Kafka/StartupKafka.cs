//using Confluent.Kafka;

//namespace WebAPI.Kafka
//{
//    public class StartupKafka
//    {
//        public void ConfigureServices(IServiceCollection services)
//        {
//            // Đăng ký Kafka producer
//            //services.AddSingleton(new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build());
//            services.AddSingleton<IProducer<Null, string>>(new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build());
//            // Đăng ký ProductConsumer
//            services.AddSingleton<KafkaConsumer>();
//        }

//        public void Configure(IApplicationBuilder app
//            , IWebHostEnvironment env,
//            KafkaConsumer cartConsumer)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }

//            app.UseRouting();

//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapControllers();
//            });

//            // Bắt đầu Consumer Kafka
//            cartConsumer.Consume();
//        }
//    }
//}