using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Model;
using WebAPI.Redis;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly DataContext _dbContext;
        private readonly IRedisCacheService _cacheService;
        public OrderController(DataContext dbContext, IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        //Tìm ALL
        [HttpGet("GetOrderAll")]
        public async Task<IActionResult> GetAll()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Order>>("Order");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            cacheData = await _dbContext.Orders.ToListAsync();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);

            _cacheService.SetData<IEnumerable<Order>>("Order", cacheData, expirationTime);
            return Ok(cacheData);
        }

        //Tìm theo ID
        [HttpGet("GetOrderByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            Order filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<Order>>("Order");
            if (cacheData != null && cacheData.Count() > 0)
            {
                filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                return Ok(filteredData);

            }

            filteredData = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (filteredData != null)
            {
                return Ok(filteredData);
            }
            else
            {
                return NotFound("Không tìm thấy đơn hàng");
            }
            return default;
        }
        //Thêm
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> Post(Order value)
        {
            var obj = await _dbContext.Orders.AddAsync(value);

            //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            //  _cacheService.SetData<Order>("Order",obj.Entity,expirationTime);
            _cacheService.RemoveData("Order");
            await _dbContext.SaveChangesAsync();
            return Ok(obj.Entity);
        }

        //Sửa theo Id ?
        [HttpPut("UpdateOrder/{id}")]
        public async Task<IActionResult> Update(int id, Order Order)
        {
            var sp_update = await _dbContext.Orders.FindAsync(id);


            if (sp_update == null)
            {
                return NotFound("Không tìm thấy đơn hàng");
            }
            else
            {
                if (!String.IsNullOrEmpty(Order.UserId.ToString()))
                    sp_update.UserId = Order.UserId;
                if (!String.IsNullOrEmpty(Order.Name))
                    sp_update.Name = Order.Name;
                if (!String.IsNullOrEmpty(Order.Phone.ToString()))
                    sp_update.Phone = Order.Phone;
                if (!String.IsNullOrEmpty(Order.Status))
                    sp_update.Status = Order.Status;
                if (!String.IsNullOrEmpty(Order.Address))
                    sp_update.Address = Order.Address;
                if (!String.IsNullOrEmpty(Order.Date.ToString()))
                    sp_update.Date = Order.Date;

                if (!String.IsNullOrEmpty(Order.TotalOder.ToString()))
                    sp_update.TotalOder = Order.TotalOder;
               

                _dbContext.Orders.Update(sp_update);

                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("Order");
            }

            return Ok("Update Success");
        }
        //Xóa
        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Order = await _dbContext.Orders.FindAsync(id);

            if (Order == null)
            {
                return NotFound("Không tìm thấy đơn hàng");
            }

            _dbContext.Orders.Remove(Order);
            _cacheService.RemoveData("Order");
            await _dbContext.SaveChangesAsync();

            return Ok("Delete Success");
            // return NoContent();
        }
    }
}
