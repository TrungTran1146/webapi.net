namespace WebAPI.Redis
{
    public class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }
        public static IServiceCollection services { get; }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
            // Kafka configuration
            //var kafkaConfig = new ProducerConfig();
            //AppSetting.Bind("Kafka", kafkaConfig);
            //services.AddSingleton(kafkaConfig);
        }
    }
}
