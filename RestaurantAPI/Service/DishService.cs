using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exeptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Service
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        DishDto GetDish(int restaurantId, int dishId);
        List<DishDto> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);
        void RemoveDish(int restaurantId, int dishId);
    }
    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private ILogger<DishService> _logger;
        public DishService(RestaurantDbContext context, IMapper mapper, ILogger<DishService> logger)
        {
            _dbContext = context;
            _mapper = mapper;
            _logger = logger;
        }
        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            return restaurant;
        }
        private Dish GetDishById(int restaurantId, int dishId)
        {
            var dish = _dbContext.Dishes.FirstOrDefault(d => d.ID == dishId);
            if (dish is null || dish.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }
            return dish;
        }
        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dishentity = _mapper.Map<Dish>(dto);
            dishentity.RestaurantId = restaurantId;
            _dbContext.Dishes.Add(dishentity);
            _dbContext.SaveChanges();
            return dishentity.ID;
        }
        public DishDto GetDish(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurantId, dishId);
            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }
        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
            return dishDtos;
        }
        public void RemoveAll(int restaurantId)
        {
            _logger.LogWarning($"Restaurant with id: {restaurantId} ALL DISHES DELETE action invoked");
            var restaurant = GetRestaurantById(restaurantId);
            _dbContext.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();
        }
        public void RemoveDish(int restaurantId, int dishId)
        {
            _logger.LogWarning($"Restaurant with id: {restaurantId}DISH whith id: {dishId} DELETE action invoked");
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurantId, dishId);
            _dbContext.Dishes.Remove(dish);
            _dbContext.SaveChanges();

        }
    }
}
