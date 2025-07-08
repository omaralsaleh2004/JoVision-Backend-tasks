using Microsoft.AspNetCore.Mvc;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreeterController : ControllerBase
    {
        [HttpGet]
        public string GetGreeting([FromQuery] string name)
        {
            var greeter = new GreeterApp();
            return greeter.GetGreeting(name);
        }
    }
}
