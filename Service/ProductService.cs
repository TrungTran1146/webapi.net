using WebAPI.Model;

namespace WebAPI.Service
{
    public class ProductService
    {
        public readonly DataContext _dbContext;
        public ProductService(DataContext context)
        {
            _dbContext = context;
        }
        public IEnumerable<Product> GetAllProducts()
        {
            return _dbContext.Products.ToList();
        }

        public IEnumerable<Product> GetPageds(int page, int pageSize)
        {
            return _dbContext.Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int Count()
        {
            return _dbContext.Products.Count();
        }

        public async Task<bool> Create(Product product)
        {
            try
            {
                var result = await _dbContext.Products.AddAsync(product);
            }
            catch (Exception ex)
            {
                var exx = ex;
            }

            return _dbContext.SaveChanges() == 1 ? true : false;
        }
    }
}
