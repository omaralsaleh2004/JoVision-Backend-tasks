using Microsoft.AspNetCore.Mvc;
using FirstApplication.Models;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BirthDateFormDataController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetAge(
         [FromBody] BirthDatePostRequest request)

        {
            var name = string.IsNullOrWhiteSpace(request.Name) ? "Anonymous" : request.Name;

            if (request.Years == null || request.Months == null || request.Days == null)
            {
                return Ok($"Hello {name}, can’t calculate your age without knowing your birthdate");
            }


            var birthDate = DateTime.Today.AddYears(-request.Years.Value).AddMonths(-request.Months.Value).AddDays(-request.Days.Value);
            var age = CalculateAge(birthDate);

            return Ok($"Hello {name}  Your Age is : {age} ");
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            return age;
        }
    }
}
