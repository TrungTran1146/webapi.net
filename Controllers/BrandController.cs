using WebAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Redis;
using WebAPI.Model.Auth;

namespace APIDemo.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class BrandController : ControllerBase
    //{
    //    private readonly DataContext _dbContext;

    //    public BrandController(DataContext context)
    //    {
    //        _dbContext = context;
    //    }
    //    // [Authorize(Policy = "admin")]
    //    [Authorize]
    //    [HttpGet("GetBrandAll")]
    //    public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
    //    {
    //        return await _dbContext.Brands.ToListAsync();
    //    }

    //    [Authorize(Policy = "admin")]
    //    [HttpGet("GetBrandById/{id}")]
    //    public async Task<ActionResult<Brand>> GetBrand(int id)
    //    {
    //        var brand = await _dbContext.Brands.FindAsync(id);

    //        if (brand == null)
    //        {
    //            return NotFound();
    //        }

    //        return brand;
    //    }

    //    [Authorize(Policy = "admin")]
    //    [HttpPost("CreateBrand")]
    //    public async Task<ActionResult<Brand>> CreateBrand(Brand brand)
    //    {
    //        _dbContext.Brands.Add(brand);
    //        await _dbContext.SaveChangesAsync();

    //        return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
    //    }

    //    [HttpPut("UpdateBrand/{id}")]
    //    public async Task<IActionResult> UpdateBrand(int id, Brand brand)
    //    {
    //        if (id != brand.Id)
    //        {
    //            return BadRequest();
    //        }

    //        _dbContext.Entry(brand).State = EntityState.Modified;
    //        await _dbContext.SaveChangesAsync();

    //        return NoContent();
    //    }

    //    [HttpDelete("DeleteBrand/{id}")]
    //    public async Task<IActionResult> DeleteBrand(int id)
    //    {
    //        var brand = await _dbContext.Brands.FindAsync(id);

    //        if (brand == null)
    //        {
    //            return NotFound();
    //        }

    //        _dbContext.Brands.Remove(brand);
    //        await _dbContext.SaveChangesAsync();

    //        return NoContent();
    //    }
        /////////////////////////////////Redis
        [Route("api/[controller]")]
        [ApiController]
        public class BrandController : ControllerBase
        {

            private readonly DataContext _dbContext;
            private readonly IRedisCacheService _cacheService;
            public BrandController(DataContext dbContext, IRedisCacheService cacheService)
            {
                _dbContext = dbContext;
                _cacheService = cacheService;
            }

        //Tìm ALL
       
        [HttpGet("GetBrandAll")]
            public async Task<IActionResult> GetAll()
            {
                var cacheData = _cacheService.GetData<IEnumerable<Brand>>("brand");
                if (cacheData != null && cacheData.Count() > 0)
                {
                    return Ok(cacheData);
                }
                cacheData = await _dbContext.Brands.ToListAsync();
                var expirationTime = DateTimeOffset.Now.AddMinutes(5);

                _cacheService.SetData<IEnumerable<Brand>>("brand", cacheData, expirationTime);
                return Ok(cacheData);
            }

        //Tìm theo ID
        
        [HttpGet("GetBrandByID/{id}")]
            public async Task<IActionResult> GetByID(int id)
            {
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
                    return Ok(filteredData);
                }
                else
                {
                    return NotFound("Không tìm thấy sản phẩm");
                }
                return default;
            }
        //Thêm
        [Authorize(Policy = "admin")]
        [HttpPost("CreateBrand")]
            public async Task<IActionResult> Post(Brand value)
            {
                var obj = await _dbContext.Brands.AddAsync(value);

                //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
                //  _cacheService.SetData<Brand>("brand",obj.Entity,expirationTime);
                _cacheService.RemoveData("brand");
                await _dbContext.SaveChangesAsync();
                return Ok(obj.Entity);
            }

        //Sửa theo Id ?
        [Authorize(Policy = "admin")]
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

                return Ok("Update Success");
            }
        //Xóa
        [Authorize(Policy = "admin")]
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
