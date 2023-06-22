using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio;
using Nest;
using WebAPI.Model;
using WebAPI.Redis;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //LOGSTASH
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductController> _logger;
        //MINIO
        //private readonly MinioService _minioService;
        private readonly MinioClient _minioClient;
        private readonly string _bucketName;

        //POSGRESQL
        private readonly DataContext _dbContext;
        private readonly ProductService _productService;
        //REDIS
        private readonly IRedisCacheService _cacheService;
        public ProductController(
            IElasticClient elasticClient,
            ILogger<ProductController> logger,
            DataContext dbContext,
            ProductService productService,
            IRedisCacheService cacheService,
            //MinioService minioService,
            MinioClient minioClient,

        IConfiguration configuration)
        {
            _dbContext = dbContext;
            _productService = productService;
            _cacheService = cacheService;
            _logger = logger;
            _elasticClient = elasticClient;
            //_minioService = minioService;
            _minioClient = minioClient;
            _bucketName = configuration.GetValue<string>("Minio:BucketName");
        }

        //Tìm ALL
        [HttpGet("GetProductAll")]
        public async Task<IActionResult> GetAll()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            cacheData = await _dbContext.Products.ToListAsync();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);

            _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
            return Ok(cacheData);
        }



        [HttpGet("products/paged")]
        public async Task<IActionResult> GetPagedProducts(int page = 1, int pageSize = 8)
        {
            var products = _productService.GetPageds(page, pageSize);
            var result = new PagedResult<Product>
            {
                Items = products,
                TotalItems = _productService.Count(),
                CurrentPage = page,
                PageSize = pageSize
            };
            return Ok(result);
        }


        /// //////////////////////////////////

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

        //    //   var expirationTime = DateTimeOffset.Now.AddMinutes(5);
        //    //  _cacheService.SetData<Product>("product",obj.Entity,expirationTime);
        //    _cacheService.RemoveData("product");
        //    await _dbContext.SaveChangesAsync();
        //    return Ok(obj.Entity);
        //}

        //[Authorize(Policy = "admin")]
        [HttpPost("CreateProduct")]

        public async Task<ActionResult<Product>> Post([FromForm] ProductCreateModel model)
        {
            try
            {
                string imageUrl = null;



                if (model.Image != null && model.Image.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(model.Image.FileName)}";
                    var stream = model.Image.OpenReadStream();

                    await _minioClient.PutObjectAsync(_bucketName, fileName, stream, stream.Length, "application/octet-stream");

                    imageUrl = $"{"http://localhost:9000"}/{_bucketName}/{fileName}";
                }
                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    Status = model.Status,
                    Image = imageUrl,
                    Description = model.Description,
                    Quantity = model.Quantity,
                    BrandId = model.BrandId,
                    TypeCarId = model.TypeCarId,

                };

                _dbContext.Products.Add(product);
                _cacheService.RemoveData("product");
                await _dbContext.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        //Sửa theo Id ?
        //[Authorize(Policy = "admin")]
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductCreateModel model)
        {
            var sp_update = await _dbContext.Products.FindAsync(id);


            if (sp_update == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }
            else
            {
                string imageUrl = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(model.Image.FileName)}";
                    var stream = model.Image.OpenReadStream();

                    await _minioClient.PutObjectAsync(_bucketName, fileName, stream, stream.Length, "application/octet-stream");

                    imageUrl = $"{"http://localhost:9000"}/{_bucketName}/{fileName}";
                }

                if (!String.IsNullOrEmpty(model.Name))
                    sp_update.Name = model.Name;

                if (model.Price != null)
                    sp_update.Price = model.Price;

                if (!String.IsNullOrEmpty(model.Status))
                    sp_update.Status = model.Status;

                if (!String.IsNullOrEmpty(imageUrl))
                    sp_update.Image = imageUrl;

                if (!String.IsNullOrEmpty(model.Description))
                    sp_update.Description = model.Description;

                if (!String.IsNullOrEmpty(model.Quantity.ToString()))
                    sp_update.Quantity = model.Quantity;

                if (!String.IsNullOrEmpty(model.BrandId.ToString()))
                    sp_update.BrandId = model.BrandId;
                if (!String.IsNullOrEmpty(model.TypeCarId.ToString()))
                    sp_update.TypeCarId = model.TypeCarId;

                _dbContext.Products.Update(sp_update);

                await _dbContext.SaveChangesAsync();
                _cacheService.RemoveData("product");
            }

            return Ok(sp_update);
        }
        //Xóa
        //[Authorize(Policy = "admin")]
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

        [HttpGet("GetTypeProduct/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // Get list of products for the specified type

            var product = _dbContext.Products.AsQueryable()
              .Join(_dbContext.TypeCars.AsQueryable(), p => p.TypeCarId, t => t.Id, (p, t) => new { Product = p, TypeCar = t })
            //.FirstOrDefault(pt => pt.Product.Id == id);
            .Where(pt => pt.TypeCar.Id == id)
            .Select(pt => new
            {
                Id = pt.Product.Id,
                Name = pt.Product.Name,
                Image = pt.Product.Image,
                Price = pt.Product.Price,
                Type = pt.TypeCar.NameType
            });

            //return Ok(product);
            return Ok(product.ToList());
        }


    }

    public class ProductCreateModel
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Status { get; set; }
        public IFormFile Image { get; set; }
        public string? Description { get; set; }

        public int Quantity { get; set; }
        public int BrandId { get; set; }
        public int TypeCarId { get; set; }
    }

    //public class PagedResult<T>
    //{
    //    public IEnumerable<T> Items { get; set; }
    //    public int TotalItems { get; set; }
    //    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    //    public int CurrentPage { get; set; }
    //    public int PageSize { get; set; }
    //}


}

