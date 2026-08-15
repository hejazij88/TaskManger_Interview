using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManager.API.Filters
{
    public class ValidationFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                    continue;

                var argumentType = argument.GetType();

                var validatorType = typeof(IValidator<>)
                    .MakeGenericType(argumentType);

                var validator = context.HttpContext
                    .RequestServices
                    .GetService(validatorType);

                if (validator is null)
                    continue;

                var validationContextType =
                    typeof(ValidationContext<>)
                        .MakeGenericType(argumentType);

                var validationContext =
                    Activator.CreateInstance(
                        validationContextType,
                        argument);

                var validateAsyncMethod =
                    validatorType.GetMethod(
                        nameof(IValidator<object>.ValidateAsync),
                        new[]
                        {
                        validationContextType,
                        typeof(CancellationToken)
                        });

                if (validateAsyncMethod is null)
                    continue;

                var task = (Task)validateAsyncMethod.Invoke(
                    validator,
                    new[]
                    {
                    validationContext,
                    context.HttpContext.RequestAborted
                    })!;

                await task;

                var resultProperty = task.GetType()
                    .GetProperty("Result");

                var result = resultProperty?.GetValue(task);

                if (result is null)
                    continue;

                var errorsProperty = result.GetType()
                    .GetProperty("Errors");

                var errors = errorsProperty?.GetValue(result)
                    as IEnumerable<FluentValidation.Results.ValidationFailure>;

                if (errors is null)
                    continue;

                var validationErrors = errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .Select(x => x.ErrorMessage)
                            .ToArray());

                if (validationErrors.Count == 0)
                    continue;

                context.Result = new BadRequestObjectResult(
                    new ValidationProblemDetails(validationErrors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "One or more validation errors occurred."
                    });

                return;
            }

            await next();
        }
    }
}
