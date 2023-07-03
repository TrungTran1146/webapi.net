using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using Serilog;
using WebAPI.Model;
using WebAPI.Redis;

namespace WebAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {

        private readonly IElasticClient _elasticClient;
        private readonly ILogger<BrandController> _logger;

        private readonly DataContext _dbContext;
        private readonly IRedisCacheService _cacheService;
        public BrandController(
                IElasticClient elasticClient,
                ILogger<BrandController> logger,
                DataContext dbContext,
                IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _logger = logger;
            _elasticClient = elasticClient;
        }

        //Tìm ALL

        [HttpGet("GetBrandAll")]
        public async Task<IActionResult> GetAll()
        //(string keyword)
        {
            Log.Information("trung dep trai");

            try
            {
                //Log.Information("Logged on {PlaceHolderName:MMMM dd, yyyy}", DateTimeOffset.UtcNow);

                // Lấy danh sách tất cả sản phẩm từ CSDL
                // ...
                var cacheData = _cacheService.GetData<IEnumerable<Brand>>("brand");
                if (cacheData != null && cacheData.Count() > 0)
                {
                    return Ok(cacheData);
                }
                cacheData = await _dbContext.Brands.ToListAsync();
                var expirationTime = DateTimeOffset.Now.AddMinutes(5);

                _cacheService.SetData<IEnumerable<Brand>>("brand", cacheData, expirationTime);
                //Log.Debug("Total products: {ProductCount}", products.Count);
                // Ghi log số lượng sản phẩm
                // ...

                //Log.InformatKion("TrungTran", cacheData);
                //Log.Information("trungdep trai", cacheData);
                return Ok(cacheData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "trung");
                return StatusCode(500, "An error occurred");
            }
        }

        //Tìm theo ID

        [HttpGet("GetBrandByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            Log.Information("Logged on {PlaceHolderName:MMMM dd, yyyy}", DateTimeOffset.UtcNow);


            try
            {
                // Lấy danh sách tất cả sản phẩm từ CSDL
                // ...
                Log.Information("Trung Api ID");

                Brand filteredData;
                var cacheData = _cacheService.GetData<IEnumerable<Brand>>("brand");
                if (cacheData != null && cacheData.Count() > 0)
                {
                    filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                    return Ok(filteredData);

                }

                filteredData = await _dbContext.Brands.FirstOrDefaultAsync(x => x.Id == id);
                if (filteredData != null)
                {
                    Log.Information("API request processed successfully");
                    return Ok(filteredData);
                }
                else
                {
                    return NotFound("Không tìm thấy sản phẩm");
                }
                //Log.Debug("Total products: {ProductCount}", products.Count);
                // Ghi log số lượng sản phẩm
                // ...
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the API request");
                return StatusCode(500, "An error occurred");
            }
            ////////
            //_logger.LogInformation("Getting all brands");
            //Brand filteredData;
            //var cacheData = _cacheService.GetData<IEnumerable<Brand>>("brand");
            //if (cacheData != null && cacheData.Count() > 0)
            //{
            //    filteredData = cacheData.FirstOrDefault(x => x.Id == id);
            //    return Ok(filteredData);

            //}

            //filteredData = await _dbContext.Brands.FirstOrDefaultAsync(x => x.Id == id);
            //if (filteredData != null)
            //{
            //    return Ok(filteredData);
            //}
            //else
            //{
            //    return NotFound("Không tìm thấy sản phẩm");
            //}

        }
        //Thêm
        // [Authorize(Policy = "admin")]
        [HttpPost("CreateBrand")]
        public async Task<IActionResult> Post(Brand brand)
        {
            //await _elasticClient.IndexDocumentAsync(brand);

            //_logger.LogInformation("BrandController Post - ", DateTime.UtcNow);
            ///////////////////////
            //var obj = await _dbContext.Brands.AddAsync(brand);

            ////   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            ////  _cacheService.SetData<Brand>("brand",obj.Entity,expirationTime);
            //_cacheService.RemoveData("brand");
            //await _dbContext.SaveChangesAsync();
            ////return Ok(obj.Entity);
            //return Ok(obj.Entity);



            try
            {
                // Lấy danh sách tất cả sản phẩm từ CSDL
                // ...


                var obj = await _dbContext.Brands.AddAsync(brand);

                //var tam2 = JsonConvert.DeserializeObject();

                //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
                //  _cacheService.SetData<Brand>("brand",obj.Entity,expirationTime);
                _cacheService.RemoveData("brand");
                await _dbContext.SaveChangesAsync();
                Log.Warning("{@Brand} at {time}", brand, DateTime.UtcNow.ToString());
                //return Ok(obj.Entity);
                return Ok(obj.Entity);
                //Log.Debug("Total products: {ProductCount}", products.Count);
                // Ghi log số lượng sản phẩm
                // ...
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing the API request");
                return StatusCode(500, "An error occurred");
            }
        }

        //Sửa theo Id ?
        //  [Authorize(Policy = "admin")]
        [HttpPut("UpdateBrand/{id}")]
        public async Task<IActionResult> Update(int id, Brand brand)
        {
            var sp_update = await _dbContext.Brands.FindAsync(id);


            if (sp_update == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }
            else
            {
                if (!String.IsNullOrEmpty(brand.BrandName))
                    sp_update.BrandName = brand.BrandName;

                if (!String.IsNullOrEmpty(brand.Description))
                    sp_update.Description = brand.Description;

                _dbContext.Brands.Update(sp_update);
                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("brand");
            }

            return Ok(sp_update);
        }
        //Xóa
        //  [Authorize(Policy = "admin")]
        [HttpDelete("DeleteBrand/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dbContext.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            _dbContext.Brands.Remove(brand);
            _cacheService.RemoveData("brand");
            await _dbContext.SaveChangesAsync();

            return Ok("Delete Success");
            // return NoContent();
        }
    }
}
