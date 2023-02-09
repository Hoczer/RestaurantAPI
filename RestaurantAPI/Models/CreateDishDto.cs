using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class CreateDishDto
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Price { get; set; }
        public int? RestaurantId { get; set; }
    }
}
