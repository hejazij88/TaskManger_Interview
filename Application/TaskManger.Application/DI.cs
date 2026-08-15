using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Services;

namespace TaskManager.Application
{
    public static class DI
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<TaskService>();

            return services;
        }
    }
}
