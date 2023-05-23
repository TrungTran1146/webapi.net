using WebAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WebAPI.Migrations;
using WebAPI.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace APIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly DataContext _dbContext;
        private readonly IRedisCacheService _cacheService;
        public ProductController(DataContext dbContext, IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        //Tìm ALL
        [HttpGet("GetProductAll")]
        public async Task<IActionResult> GetAll()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null && cacheData.Count()>0)
            {
                return Ok(cacheData);
            }
                cacheData =await _dbContext.Products.ToListAsync();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            
            _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
            return Ok(cacheData);
        }
       
        //Tìm theo ID
        [HttpGet("GetProductByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            Product filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null && cacheData.Count() > 0)
            {
                filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                return Ok(filteredData);
                  
            }

             filteredData = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
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
        //[Authorize(Policy = "admin")]
        //[HttpPost("CreateProduct")]
        //public async Task<IActionResult> Post(Product value)
        //{
        //    var obj = await _dbContext.Products.AddAsync(value);

        // //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
        //  //  _cacheService.SetData<Product>("product",obj.Entity,expirationTime);
        //    _cacheService.RemoveData("product");
        //    await _dbContext.SaveChangesAsync();
        //    return Ok(obj.Entity);
        //}
        //[Authorize(Policy = "admin")]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Post([FromForm] string datajson,IFormFile fileimage)
        {
            
            var product =JsonConvert.DeserializeObject<Product>(datajson);
            var findP = _dbContext.Products.Find(product.Id);
            if (findP != null) {
                return Ok("Mã sản phẩm này đã tồn tại");
            }
            else
            {
                if (fileimage.Length >0) {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileimage.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await fileimage.CopyToAsync(stream);
                    }
                    product.Image = "/images/" + fileimage.FileName;
                }
                else
                {
                    product.Image = "";
                }
                _dbContext.Products.Add(product);
                _cacheService.RemoveData("product");
                await _dbContext.SaveChangesAsync();
                return Ok(product);
            }
         //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
          //  _cacheService.SetData<Product>("product",obj.Entity,expirationTime);
           
          
        }

        //Sửa theo Id ?
        [Authorize(Policy = "admin")]
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var sp_update = await _dbContext.Products.FindAsync(id);
            

            if (sp_update == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }
            else
            {
                if (!String.IsNullOrEmpty(product.Name)) 
                    sp_update.Name = product.Name;
                if (!String.IsNullOrEmpty(product.Price.ToString()))
                    sp_update.Price = product.Price;
                if (!String.IsNullOrEmpty(product.Status)) 
                    sp_update.Status = product.Status;
                if (!String.IsNullOrEmpty(product.Image))
                    sp_update.Image = product.Image;
                if (!String.IsNullOrEmpty(product.Description))
                    sp_update.Description = product.Description;
                
                if (!String.IsNullOrEmpty(product.Quantity.ToString()))
                    sp_update.Quantity = product.Quantity;
                
                if (!String.IsNullOrEmpty(product.BrandId.ToString()))
                    sp_update.BrandId = product.BrandId;
                if (!String.IsNullOrEmpty(product.TypeCarId.ToString()))
                    sp_update.TypeCarId = product.TypeCarId;

                _dbContext.Products.Update(sp_update);

                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("product");
            }

            return Ok("Update Success");
        }
        //Xóa
        [Authorize(Policy = "admin")]
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            _dbContext.Products.Remove(product);
            _cacheService.RemoveData("product");
            await _dbContext.SaveChangesAsync();

            return Ok("Delete Success");
           // return NoContent();
        }

      
    }
}

