using Newtonsoft.Json;
using WebAPI.Model;
using WebAPI.Service;

namespace WebAPI.Kafka
{
    public class KafkaWorkflow
    {
        //private DmitdemoContext context;
        //private IRedisService redisService;
        public KafkaWorkflow()
        {
        }

        public async Task<bool> CreateProduct(string data)
        {
            //IRedisCacheService _cacheService = new IRedisCacheService();
            var context = new DataContext();
            var temp = new ProductService(context);
            var jsonData = JsonConvert.DeserializeObject<Product>(data);
            var result = await temp.Create(jsonData);
            //_cacheService.RemoveData("product");
            return result;
        }


        //public async Task<bool> UpdateProduct(string data)
        //{
        //    await _productService.Create
        //      var temp = new ProductService(context, redisService);
        //    var modelReceive = JsonConvert.DeserializeObject<Product>(data);
        //    var result = await temp.UpdateProductAsync(modelReceive, 22);
        //    return result;
        //}
    }
}
