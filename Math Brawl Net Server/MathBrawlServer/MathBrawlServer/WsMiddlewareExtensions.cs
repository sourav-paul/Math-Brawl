using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MathBrawlServer
{
    public static class WsMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WsMiddleware>();
        }
        
        public static IServiceCollection AddWebSocketServerConnectionManager(this IServiceCollection services)
        {   
            services.AddSingleton<WsConnectionManager>();
            return services;
        }
    }
}