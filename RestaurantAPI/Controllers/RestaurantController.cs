using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Service;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService= restaurantService;
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody]UpdateRestaurantDto dto, [FromRoute]int id)
        {
          
            _restaurantService.Update(dto,id);
            
            return Ok();
        }
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute]int id)
        {
            _restaurantService.Delete(id);
            
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles ="Admin,Manager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
           
            var id =_restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}",null);
        }

        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantDto = _restaurantService.GetAll();
            return Ok(restaurantDto);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);
            
            
            return Ok(restaurant);
        }
    }
}
