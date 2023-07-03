using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Model;
using WebAPI.Redis;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportProductController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly IRedisCacheService _cacheService;
        public ImportProductController(DataContext dbContext, IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        //Tìm ALL

        [HttpGet("GetImportProductAll")]
        public async Task<IActionResult> GetAll()
        {
            var cacheData = _cacheService.GetData<IEnumerable<ImportProduct>>("ImportProduct");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            cacheData = await _dbContext.ImportProducts.ToListAsync();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);

            _cacheService.SetData<IEnumerable<ImportProduct>>("ImportProduct", cacheData, expirationTime);
            return Ok(cacheData);
        }

        //Tìm theo ID

        [HttpGet("GetImportProductByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            ImportProduct filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<ImportProduct>>("ImportProduct");
            if (cacheData != null && cacheData.Count() > 0)
            {
                filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                return Ok(filteredData);

            }

            filteredData = await _dbContext.ImportProducts.FirstOrDefaultAsync(x => x.Id == id);
            if (filteredData != null)
            {
                return Ok(filteredData);
            }
            else
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

        }
        //Thêm
        //[Authorize(Policy = "admin")]
        [HttpPost("CreateImportProduct")]
        public async Task<IActionResult> Post(ImportProduct value)
        {
            var obj = await _dbContext.ImportProducts.AddAsync(value);

            //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            //  _cacheService.SetData<ImportProduct>("ImportProduct",obj.Entity,expirationTime);
            _cacheService.RemoveData("ImportProduct");
            await _dbContext.SaveChangesAsync();
            return Ok(obj.Entity);
        }

        //Sửa theo Id ?
        //[Authorize(Policy = "admin")]
        [HttpPut("UpdateImportProduct/{id}")]
        public async Task<IActionResult> Update(int id, ImportProduct ImportProduct)
        {
            var sp_update = await _dbContext.ImportProducts.FindAsync(id);


            if (sp_update == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }
            else
            {

                if (!String.IsNullOrEmpty(ImportProduct.ProductId.ToString()))
                    sp_update.ProductId = ImportProduct.ProductId;

                if (!String.IsNullOrEmpty(ImportProduct.Quantity.ToString()))
                    sp_update.Quantity = ImportProduct.Quantity;

                if (!String.IsNullOrEmpty(ImportProduct.OrderDate.ToString()))
                    sp_update.OrderDate = ImportProduct.OrderDate;


                _dbContext.ImportProducts.Update(sp_update);

                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("ImportProduct");
            }

            return Ok("Update Success");
        }
        //Xóa
        //[Authorize(Policy = "admin")]
        [HttpDelete("DeleteImportProduct/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ImportProduct = await _dbContext.ImportProducts.FindAsync(id);

            if (ImportProduct == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            _dbContext.ImportProducts.Remove(ImportProduct);
            _cacheService.RemoveData("ImportProduct");
            await _dbContext.SaveChangesAsync();

            return Ok("Delete Success");
            // return NoContent();
        }
    }
}
