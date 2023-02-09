using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Entities
{
    public class Dish
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Price { get; set; }
        public int? RestaurantId { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
    }
}
