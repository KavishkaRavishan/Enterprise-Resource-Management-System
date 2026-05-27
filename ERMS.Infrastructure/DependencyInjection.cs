using ERMS.Application.Interfaces;
using ERMS.Application.Interfaces.Repositories;
using ERMS.Application.Services;
using ERMS.Infrastructure.Auth;
using ERMS.Infrastructure.Data;
using ERMS.Infrastructure.Data.Interceptors;
using ERMS.Infrastructure.Repositories;
using ERMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Context Accesor & User context
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Interceptors
            services.AddScoped<AuditSaveChangesInterceptor>();

            // Database
            services.AddDbContext<AppDbContext>((sp, options) =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ITimeLogRepository, TimeLogRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            // Auth
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // File Storage
            services.AddSingleton<IFileStorageService, FileStorageService>();

            // Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITimeLogService, TimeLogService>();
            services.AddScoped<IAttachmentService, AttachmentService>();

            return services;
        }
    }
}
