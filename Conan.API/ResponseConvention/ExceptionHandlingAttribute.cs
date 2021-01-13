using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Conan.API.ResponseConvention
{
    public class ExceptionHandlingAttribute : ActionFilterAttribute
    {
        public ExceptionHandlingAttribute()
        {
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var e = context.Exception;
            if (e == null)
                return;

            context.ExceptionHandled = true;

            if (e is BadRequestException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    message = e.Message
                });
            }
            else if (e is UnauthorizedException)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = e.Message
                });
            }
            else if (e is ForbidException)
            {
                var result = new ObjectResult(new
                {
                    message = e.Message
                });

                result.StatusCode = 403;
                context.Result = result;
            }
            else if (e is NotFoundException)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    message = e.Message
                });
            }
            else
            {
                context.ExceptionHandled = false;
            }

        }
    }
}
