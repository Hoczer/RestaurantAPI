using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exeptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Service
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        IEnumerable<RestaurantDto> GetAll();
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Update(UpdateRestaurantDto dto, int id);
    }
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }
        public void Update(UpdateRestaurantDto dto, int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(x => x.Id == id);
            if (restaurant is null) throw new NotFoundException("Restaurant not found");
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery= dto.HasDelivery;
            _dbContext.Restaurants.Update(restaurant);
            _dbContext.SaveChanges();
            
        }
        public void Delete(int id)
        {
            _logger.LogWarning($"Restaurant with id: {id} DELETE action invoked");
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(x => x.Id == id);
            if (restaurant is null) throw new NotFoundException("Restaurant not found");
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
            

        }
        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Adress)
                .Include(r => r.Dishes)
                .FirstOrDefault(x => x.Id == id);
            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return restaurantDto;
        }
        public IEnumerable<RestaurantDto> GetAll()
        {
            var restaurants = _dbContext.Restaurants
                .Include(r => r.Adress)
                .Include(r => r.Dishes)
                .ToList();
            var restaurantDto = _mapper.Map<List<RestaurantDto>>(restaurants);
            return restaurantDto;
        }
        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}
