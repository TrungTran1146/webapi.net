using WebAPI.Model;

namespace WebAPI.Service
{
    public class OrderService
    {
        private readonly DataContext _dbContext;

        public OrderService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Order> GetAllOrders()
        {
            return _dbContext.Orders.ToList();
        }

        public IEnumerable<Order> GetPageds(int page, int pageSize)
        {
            return _dbContext.Orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int Count()
        {
            return _dbContext.Orders.Count();
        }
    }
}
