using WebAPI.Model;

namespace WebAPI.Service
{
    public class ProductService
    {
        private readonly DataContext _dbContext;

        public ProductService(DataContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
