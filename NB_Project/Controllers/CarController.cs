using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NB_Project.ApplicationDbContext;
using NB_Project.Model;
using NB_Project.Services;

namespace NB_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly DB _db;

        public CarController(DB db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet("Get-Cars")]
        public async Task<IActionResult> GetCars()
        {
            try
            {
                var cars = await _db.cars.ToListAsync();
                if (cars != null)
                {
                    return Ok(cars);
                }
                return NotFound("No Car found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Update-Car")]
        public async Task<IActionResult> UpdateCar(CarDTO carDTO)
        {
            try
            {
                if (carDTO != null && ModelState.IsValid)
                {

                    var car = await _db.cars.FindAsync(carDTO.id);
                    if (car != null)
                    {
                        car.typeCar = carDTO.typeCar;
                        car.Model = carDTO.Model;
                        car.Color = car.Color;
                        car.YearManufacture = carDTO.YearManufacture;
                        car.chassisآumber = carDTO.chassisآumber;
                        car.PlateNumber = carDTO.PlateNumber;
                        var count = await _db.SaveChangesAsync();
                        if(count > 0)
                        {
                            return Ok("Car Update Successfully");
                        }
                        else
                        {
                            return BadRequest("Car not updated try again");
                        }
                    }
                }
                return NotFound("Car not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Add-Car")]
        public async Task<IActionResult> AddCar(CarDTO carDTO)
        {
            try
            {
            if(ModelState.IsValid && carDTO != null)
            {
                    var car = new Car()
                    {
                        typeCar = carDTO.typeCar,
                        Model = carDTO.Model,
                        Color = carDTO.Color,
                        YearManufacture = carDTO.YearManufacture,
                        chassisآumber = carDTO.chassisآumber,
                        PlateNumber = carDTO.PlateNumber,
                        UserId = int.Parse(User.FindFirst("id")?.Value!)
                    };
                    await _db.cars.AddAsync(car);
                    var count = await _db.SaveChangesAsync();
                    if (count > 0)
                    {
                        return Ok("Car Added Successfully");
                    }
                    else
                    {
                        return BadRequest("Car not added try again");
                    }
                }
                return BadRequest("Data InValid");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Delete-Car/{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var car = await _db.cars.FindAsync(id);
                if (car != null)
                {
                    _db.cars.Remove(car);
                    var count = await _db.SaveChangesAsync();
                    if (count > 0)
                    {
                        return Ok("Car Deleted Successfully");
                    }
                    else
                    {
                        return BadRequest("Car not deleted try again");
                    }
                }
                return NotFound("Car not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
