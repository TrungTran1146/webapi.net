using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Model;
using WebAPI.Redis;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly DataContext _dbContext;
        private readonly OrderService _orDerService;

        private readonly IRedisCacheService _cacheService;

        public List<OrderItem> Items { get; private set; }

        public OrderController(DataContext dbContext, OrderService orDerService, IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _orDerService = orDerService;
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

        /// ////////


        [HttpGet("orders/paged")]
        public async Task<IActionResult> GetPagedProducts(int page = 1, int pageSize = 8)
        {
            var orDers = _orDerService.GetPageds(page, pageSize);

            var result = new PagedResult<Order>
            {
                Items = orDers,
                TotalItems = _orDerService.Count(),
                CurrentPage = page,
                PageSize = pageSize
            };
            return Ok(result);
        }


        //Tìm theo ID
        [HttpGet("GetOrderByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            //Order filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<Order>>("Order");
            if (cacheData != null && cacheData.Count() > 0)
            {
                var cache = cacheData.FirstOrDefault(x => x.Id == id);
                return Ok(cache);

            }

            var filteredData = await _dbContext.Orders.Where(x => x.UserId == id).ToListAsync();
            if (filteredData != null)
            {
                return Ok(filteredData);
            }
            else
            {
                return NotFound("Không tìm thấy đơn hàng");
            }

        }
        //Thêm
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> Post(Order value)
        {
            var obj = await _dbContext.Orders.AddAsync(value);
            Items = value.Items.Select(x => new OrderItem
            {
                OrderId = x.Id,
                ProductId = x.ProductId,
                Quantity = x.Quantity
            }).ToList();

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
                int userId = (int)(Order.UserId ?? sp_update.UserId);
                sp_update.UserId = userId;

                if (!String.IsNullOrEmpty(Order.Name))
                    sp_update.Name = Order.Name;

                int phone = (int)(Order.Phone ?? sp_update.Phone);
                sp_update.Phone = phone;

                if (!String.IsNullOrEmpty(Order.Status))
                    sp_update.Status = Order.Status;

                if (!String.IsNullOrEmpty(Order.Address))
                    sp_update.Address = Order.Address;

                if (!String.IsNullOrEmpty(Order.Date))
                    sp_update.Date = Order.Date;

                //if (!String.IsNullOrEmpty(Order.Quantity.ToString()))
                //    sp_update.Quantity = Order.Quantity;
                int quantity = (int)(Order.Quantity ?? sp_update.Quantity);
                sp_update.Quantity = quantity;

                if (!String.IsNullOrEmpty(Order.NameUser))
                    sp_update.NameUser = Order.NameUser;

                if (Order.TotalOrder != null)
                    sp_update.TotalOrder = Order.TotalOrder;

                _dbContext.Orders.Update(sp_update);

                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("Order");
            }

            return Ok(sp_update);
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
