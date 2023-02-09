using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Service;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dish;
        public DishController(IDishService dish)
        {
            _dish = dish;
        }
        [HttpPost]
        public ActionResult Post([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {
            var dish = _dish.Create(restaurantId, dto);
            return Created($"api/restaurant/{restaurantId}/dish/{dish}", null);
        }
        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = _dish.GetDish(restaurantId,dishId);
            return Ok(dish);
        }
        [HttpGet]
        public ActionResult <List<DishDto>> GetAll([FromRoute] int restaurantId)
        {
            var result = _dish.GetAll(restaurantId);
            return Ok(result);
        }
        [HttpDelete]
        public ActionResult Delete([FromRoute] int restaurantId)
        {
            _dish.RemoveAll(restaurantId);
            return NoContent();
        }
        [HttpDelete("{dishId}")]
        public ActionResult DeleteById ([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dish.RemoveDish(restaurantId, dishId);
            return NoContent();
        }
    }
}
