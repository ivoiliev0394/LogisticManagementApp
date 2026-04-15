using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("400")]
        public IActionResult BadRequestPage()
        {
            Response.StatusCode = 400;
            return View("BadRequest");
        }

        [Route("401")]
        public IActionResult UnauthorizedPage()
        {
            Response.StatusCode = 401;
            return View("Unauthorized");
        }

        [Route("403")]
        public IActionResult Forbidden()
        {
            Response.StatusCode = 403;
            return View("Forbidden");
        }

        [Route("404")]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        [Route("500")]
        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View("ServerError");
        }
    }
}
