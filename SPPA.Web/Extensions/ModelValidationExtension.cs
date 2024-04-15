using SPPA.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net;
using System.Net.Mime;
using SPPA.Domain.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SPPA.Web.Extensions
{
    public static class ModelValidationExtension
    {

        public static IMvcBuilder UseMfValidationResponseModel(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.ConfigureApiBehaviorOptions(
                options =>
                {
                    options.InvalidModelStateResponseFactory = (context) =>
                    {
                        var validateErrors = context.ModelState
                                                    .Where(x => x.Value!.ValidationState == ModelValidationState.Invalid)
                                                    .ToDictionary
                                                    (
                                                        x => x.Key.ToCamelCase(),
                                                        x => GetMfValidationModel(x.Value!.Errors.First().ErrorMessage)
                                                    );

                        var model = new MfErrorModel("server-error.bad-request.common", null, validateErrors, "Validation error");
                        var httpResult = new ObjectResult(null)
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            ContentTypes = new MediaTypeCollection { MediaTypeNames.Application.Json },
                            Value = model
                        };

                        return httpResult;
                    };
                });

            return mvcBuilder;
        }

        private static MfErrorValidationMessage GetMfValidationModel(string srcMessage)
        {
            Dictionary<string, string>? args = null;
            var message = srcMessage;
            string? debugMsg = null;

            if (srcMessage.StartsWith("server-error."))
            {
                if (srcMessage.Contains(";"))
                {
                    var splitSrcMessage = srcMessage.Split(';');
                    message = splitSrcMessage[0];
                    args = new Dictionary<string, string>() { { "n", splitSrcMessage[1] } };
                }
            }
            else
            {
                message = "server-error.validation.common";
                debugMsg = srcMessage;
            }

            return new MfErrorValidationMessage(message, args, debugMsg);
        }

        public static MvcOptions UseMfValidationMessage(this MvcOptions options)
        {
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) =>
                "server-error.validation.value-not-valid");

            options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) =>
                "server-error.validation.value-was-not-provided;" + x);

            options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() =>
                "server-error.validation.value-required");

            options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() =>
                "server-error.validation.request-body-required");

            options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) =>
                "server-error.validation.value-not-valid");

            options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() =>
                "server-error.validation.value-not-valid");

            options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() =>
                "server-error.validation.must-be-number");

            options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) =>
                "server-error.validation.value-not-valid");

            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) =>
                "server-error.validation.value-not-valid");

            options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) =>
                "server-error.validation.must-be-number");

            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) =>
                "server-error.validation.value-not-valid;" + x);

            return options;
        }

    }
}
