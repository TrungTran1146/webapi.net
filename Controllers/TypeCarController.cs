using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Model;
using WebAPI.Redis;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeCarController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly IRedisCacheService _cacheService;
        public TypeCarController(DataContext dbContext, IRedisCacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        //Tìm ALL
        [HttpGet("GetTypeCarAll")]
        public async Task<IActionResult> GetAll()
        {
            var cacheData = _cacheService.GetData<IEnumerable<TypeCar>>("TypeCar");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            cacheData = await _dbContext.TypeCars.ToListAsync();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);

            _cacheService.SetData<IEnumerable<TypeCar>>("TypeCar", cacheData, expirationTime);
            return Ok(cacheData);
        }

        //Tìm theo ID
        [HttpGet("GetTypeCarByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            TypeCar filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<TypeCar>>("TypeCar");
            if (cacheData != null && cacheData.Count() > 0)
            {
                filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                return Ok(filteredData);

            }

            filteredData = await _dbContext.TypeCars.FirstOrDefaultAsync(x => x.Id == id);
            if (filteredData != null)
            {
                return Ok(filteredData);
            }
            else
            {
                return NotFound("Không tìm thấy loại xe");
            }
            return default;
        }
        //Thêm
        //[Authorize(Policy = "admin")]
        [HttpPost("CreateTypeCar")]
        public async Task<IActionResult> Post(TypeCar value)
        {
            var obj = await _dbContext.TypeCars.AddAsync(value);

            //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            //  _cacheService.SetData<TypeCar>("TypeCar",obj.Entity,expirationTime);
            _cacheService.RemoveData("TypeCar");
            await _dbContext.SaveChangesAsync();
            return Ok(obj.Entity);
        }

        //Sửa theo Id ?
        //[Authorize(Policy = "admin")]
        [HttpPut("UpdateTypeCar/{id}")]
        public async Task<IActionResult> Update(int id, TypeCar TypeCar)
        {
            var sp_update = await _dbContext.TypeCars.FindAsync(id);


            if (sp_update == null)
            {
                return NotFound("Không tìm thấy loại xe");
            }
            else
            {
                if (!String.IsNullOrEmpty(TypeCar.NameType))
                    sp_update.NameType = TypeCar.NameType;

                if (!String.IsNullOrEmpty(TypeCar.Description))
                    sp_update.Description = TypeCar.Description;

                _dbContext.TypeCars.Update(sp_update);
                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("TypeCar");
            }

            return Ok(sp_update);
        }
        //Xóa
        //[Authorize(Policy = "admin")]
        [HttpDelete("DeleteTypeCar/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var TypeCar = await _dbContext.TypeCars.FindAsync(id);

            if (TypeCar == null)
            {
                return NotFound("Không tìm thấy loại xe");
            }

            _dbContext.TypeCars.Remove(TypeCar);
            _cacheService.RemoveData("TypeCar");
            await _dbContext.SaveChangesAsync();

            return Ok("Delete Success");
            // return NoContent();
        }
    }
}
