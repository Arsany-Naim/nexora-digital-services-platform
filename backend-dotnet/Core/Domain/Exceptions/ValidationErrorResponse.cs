using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ValidationErrorResponse
    {
        public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;
        public string ErrorMessage { get; set; } = "Validation Error";
        public IEnumerable<ValidationErrorr> Errors { get; set; }
    }
    public class ValidationErrorr
    {
        public string Field { get; set; }
        public IEnumerable<string> Error { get; set; }
    }
}
