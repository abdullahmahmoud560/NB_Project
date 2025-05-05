using System.ComponentModel.DataAnnotations.Schema;

namespace NB_Project.Model
{
    public class Car
    {
        [Column("CarID")]
        public int Id { get; set; }
        public string? typeCar { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public string? YearManufacture { get; set; }
        public string? chassisآumber { get; set; }
        public string? PlateNumber { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
    }
}
