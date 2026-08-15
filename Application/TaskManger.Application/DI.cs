using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Services;
using TaskManager.Application.Validators;

namespace TaskManager.Application
{
    public static class DI
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<TaskService>();

            services.AddValidatorsFromAssemblyContaining<
                CreateTaskRequestValidator>();

            return services;
        }
    }
}
