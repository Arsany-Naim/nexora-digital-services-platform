
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuggyController : ControllerBase
    {
        [HttpGet("notfound")]
        public IActionResult GetNotFoundResponse()
        {
            //Logic
            return NotFound();
        }
        [HttpGet("badrequest")]
        public IActionResult GetBadRequestResponse()
        {
            //Logic
            return BadRequest();
        }
        [HttpGet("badrequest/{id}")]
        public IActionResult GetValidationErrorResponse(int id)
        {
            //Logic
            return BadRequest();
        }
        [HttpGet("serveererror")]
        public IActionResult GetServerErrorResponse(int id)
        {
            //Logic
            throw new Exception();
        }
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorizedResponse()
        {
            //Logic
            return Unauthorized();
        }
    }
}
