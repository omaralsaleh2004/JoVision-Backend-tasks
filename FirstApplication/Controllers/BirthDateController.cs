using Microsoft.AspNetCore.Mvc;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BirthDateController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAge(
            [FromQuery] string? name,
            [FromQuery] int? years,
            [FromQuery] int? months,
            [FromQuery] int? days)
        {
            name = string.IsNullOrWhiteSpace(name) ? "Anonymous" : name;

            if (years == null || months == null || days == null)
            {
                return Ok($"Hello {name}, can’t calculate your age without knowing your birthdate");
            }


            var birthDate = DateTime.Today.AddYears(-years.Value).AddMonths(-months.Value).AddDays(-days.Value);
            var age = CalculateAge(birthDate);

            return Ok ($"Hello {name}  Your Age is : {age} ");
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            return age;
        }
    }
}
