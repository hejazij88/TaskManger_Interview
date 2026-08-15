using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Application.Validators;

namespace TaskManager.Application
{
    public static class DI
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>();

            services.AddValidatorsFromAssemblyContaining<
                CreateTaskRequestValidator>();

            return services;
        }
    }
}
