using System.Net;
using System.Net.Mime;
using SPPA.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Npgsql;

namespace SPPA.Web.Filters;

public class HttpResponseMfExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is MfCommonException)
        {
            var httpResult = new ObjectResult(context.Exception.Message)
            {
                ContentTypes = new MediaTypeCollection { MediaTypeNames.Application.Json },
                Value = ((MfCommonException)context.Exception).ViewModel
            };

            switch (context.Exception)
            {
                case MfBadRequestException ex:
                    httpResult.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case MfConcurrencyException ex:
                    httpResult.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                case MfLicenseException ex:
                    httpResult.StatusCode = (int)HttpStatusCode.PaymentRequired;
                    break;
                case MfNotAcceptableException ex:
                    httpResult.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    break;
                case MfNotFoundException ex:
                    httpResult.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case MfPermissionException ex:
                    httpResult.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                default:
                    throw new Exception("Unhandled exception " + context.Exception.GetType());
            }

            context.Result = httpResult;
            context.ExceptionHandled = true;
        }

        if (context.Exception is Microsoft.EntityFrameworkCore.DbUpdateException efException)
        {
            if (efException.InnerException is PostgresException postgresException)
            {
                if (postgresException?.Routine == "_bt_check_unique")
                {
                    context.Result = new ObjectResult(postgresException.Detail)
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        ContentTypes = new MediaTypeCollection { MediaTypeNames.Text.Plain }
                    };
                    context.ExceptionHandled = true;
                }
            }
        }
    }

}