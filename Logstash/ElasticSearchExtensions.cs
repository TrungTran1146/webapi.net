using Nest;
using WebAPI.Model;

namespace WebAPI.Logstash
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticsearch(
           this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ELKConfiguration:Uri"];
            var defaultIndex = configuration["ELKConfiguration:index"];

            var settings = new ConnectionSettings(new Uri(url))
                //.BasicAuthentication(userName, pass)
                .PrettyJson()
                .DefaultIndex(defaultIndex);

            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<Brand>(m => m
                   // .Ignore(p => p.Price)
                   // .Ignore(p => p.Image)
                    .Ignore(p => p.Description)
                  //  .Ignore(p => p.Image)
                );
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            //var createIndexResponse =
           // client.Indices.Create(indexName,index => index.Map<Brand>(x => x.AutoMap()));              
           client.Indices.Create(indexName, index => index.Map<Brand>(x => x.AutoMap()));


        }
    }
}
