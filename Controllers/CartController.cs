using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Model;
using WebAPI.Redis;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly IRedisCacheService _cacheService;

        //private readonly KafkaProducer _kafkaProducer;
        //private readonly KafkaConsumer _kafkaConsumer;
        public CartController(
            DataContext dbContext,
            IRedisCacheService cacheService
            //KafkaProducer kafkaProducer,
            //KafkaConsumer kafkaConsumer
            )
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            //_kafkaProducer = kafkaProducer;
            //_kafkaConsumer = kafkaConsumer;
        }

        //Tìm ALL
        [HttpGet("GetCartAll")]
        public async Task<IActionResult> GetAll()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Cart>>("cart");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            cacheData = await _dbContext.Carts.ToListAsync();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);

            _cacheService.SetData<IEnumerable<Cart>>("cart", cacheData, expirationTime);
            return Ok(cacheData);
        }

        //Tìm theo ID
        [HttpGet("GetCartByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {

            var cacheData = _cacheService.GetData<IEnumerable<Cart>>("cart");
            if (cacheData != null && cacheData.Count() > 0)
            {
                var cache = cacheData.FirstOrDefault(x => x.UserId == id);
                return Ok(cache);

            }
            //else
            //{
            //    return NotFound("Không tìm thấy sản phẩm trong giỏ");
            //}

            var filteredData = await _dbContext.Carts.Where(x => x.UserId == id).ToListAsync();
            if (filteredData != null)
            {
                return Ok(filteredData);
            }
            else
            {
                return NotFound("Không tìm thấy sản phẩm sản phẩm trong giỏ");
            }

        }
        //Thêm
        [HttpPost("CreateCart")]
        public async Task<IActionResult> Post(Cart cart)
        {
            var obj = await _dbContext.Carts.AddAsync(cart);

            //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            //  _cacheService.SetData<Cart>("cart",obj.Entity,expirationTime);
            _cacheService.RemoveData("cart");
            await _dbContext.SaveChangesAsync();
            return Ok(obj.Entity);
        }

        //[HttpPost("CreateCart")]
        //public async Task<IActionResult> Post(Cart cart)
        //{
        //    // Serialize cart object thành JSON string
        //    //var cartJson = JsonSerializer.Serialize(cart);

        //    //try
        //    //{
        //    //    // Gửi message đến Kafka để thông báo có sản phẩm mới được thêm vào
        //    //    var topicName = "cart-added";
        //    //    var result = await _producer.ProduceAsync(topicName, new Message<Null, string>
        //    //    {
        //    //        Value = cartJson
        //    //    });

        //    //    // Lưu thông tin sản phẩm vào Redis
        //    //    var expirationTime = DateTimeOffset.Now.AddMinutes(5);
        //    //    var key = $"cart:{cart.Id}";

        //    //    _cacheService.SetData(key, cartJson, expirationTime);

        //    //    return Ok();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return StatusCode(500, ex.Message);
        //    //}
        //    // Add new product to Kafka topic
        //    var message = JsonConvert.SerializeObject(cart);
        //    await _kafkaProducer.ProduceAsync("products", message);

        //    return Ok();
        //}

        /////////////////////
        //Sửa theo Id ?
        [HttpPut("UpdateCart/{id}")]
        public async Task<IActionResult> Update(int id, Cart cart)
        {
            var sp_update = await _dbContext.Carts.FindAsync(id);


            if (sp_update == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            else
            {

                if (!String.IsNullOrEmpty(cart.Quantity.ToString()))
                    sp_update.Quantity = cart.Quantity;
                if (!String.IsNullOrEmpty(cart.Name))
                    sp_update.Name = cart.Name;
                if (!String.IsNullOrEmpty(cart.Image))
                    sp_update.Image = cart.Image;
                _dbContext.Carts.Update(sp_update);
                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("cart");
            }

            return Ok("Update Success");
        }
        //Xóa
        [HttpDelete("DeleteCart/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cart = await _dbContext.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            _dbContext.Carts.Remove(cart);
            _cacheService.RemoveData("cart");
            await _dbContext.SaveChangesAsync();

            return Ok("Delete Success");
            // return NoContent();
        }
    }
}


