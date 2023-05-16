using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Migrations;
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
        public CartController(DataContext dbContext, IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
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
            Cart filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<Cart>>("cart");
            if (cacheData != null && cacheData.Count() > 0)
            {
                filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                return Ok(filteredData);

            }

            filteredData = await _dbContext.Carts.FirstOrDefaultAsync(x => x.Id == id);
            if (filteredData != null)
            {
                return Ok(filteredData);
            }
            else
            {
                return NotFound("Không tìm thấy sản phẩm");
            }
            return default;
        }
        //Thêm
        [HttpPost("CreateCart")]
        public async Task<IActionResult> Post(Cart value)
        {
            var obj = await _dbContext.Carts.AddAsync(value);

            //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            //  _cacheService.SetData<Cart>("cart",obj.Entity,expirationTime);
            _cacheService.RemoveData("cart");
            await _dbContext.SaveChangesAsync();
            return Ok(obj.Entity);
        }

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
