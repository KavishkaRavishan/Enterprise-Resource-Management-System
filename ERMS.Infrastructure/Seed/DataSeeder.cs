using ERMS.Application.Interfaces;
using ERMS.Domain.Entities;
using ERMS.Domain.Enums;
using ERMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERMS.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await context.Database.MigrateAsync();

            if (await context.Users.AnyAsync())
                return; // Already seeded

            // Create Admin
            var admin = new User("Admin", "User", "admin@erms.com", Role.Admin);
            admin.SetPassword(passwordHasher.Hash("Admin@123"));

            // Create Managers
            var manager1 = new User("Sarah", "Johnson", "sarah.johnson@erms.com", Role.Manager);
            manager1.SetPassword(passwordHasher.Hash("Manager@123"));

            var manager2 = new User("Mike", "Chen", "mike.chen@erms.com", Role.Manager);
            manager2.SetPassword(passwordHasher.Hash("Manager@123"));

            // Create Employees
            var emp1 = new User("Emily", "Davis", "emily.davis@erms.com", Role.Employee);
            emp1.SetPassword(passwordHasher.Hash("Employee@123"));

            var emp2 = new User("James", "Wilson", "james.wilson@erms.com", Role.Employee);
            emp2.SetPassword(passwordHasher.Hash("Employee@123"));

            var emp3 = new User("Lisa", "Brown", "lisa.brown@erms.com", Role.Employee);
            emp3.SetPassword(passwordHasher.Hash("Employee@123"));

            await context.Users.AddRangeAsync(admin, manager1, manager2, emp1, emp2, emp3);
            await context.SaveChangesAsync();

            // Create Projects
            var project1 = new Project(
                "Website Redesign",
                "Complete redesign of the company website with modern UI/UX",
                DateTime.UtcNow.AddDays(-30),
                DateTime.UtcNow.AddDays(60),
                manager1.Id);
            project1.UpdateStatus(ProjectStatus.InProgress);

            var project2 = new Project(
                "Mobile App Development",
                "Build a cross-platform mobile application for customers",
                DateTime.UtcNow.AddDays(-7),
                DateTime.UtcNow.AddDays(120),
                manager2.Id);

            await context.Projects.AddRangeAsync(project1, project2);
            await context.SaveChangesAsync();

            // Add members
            var members = new[]
            {
                new ProjectMember(project1.Id, manager1.Id),
                new ProjectMember(project1.Id, emp1.Id),
                new ProjectMember(project1.Id, emp2.Id),
                new ProjectMember(project2.Id, manager2.Id),
                new ProjectMember(project2.Id, emp2.Id),
                new ProjectMember(project2.Id, emp3.Id),
            };
            await context.ProjectMembers.AddRangeAsync(members);
            await context.SaveChangesAsync();

            // Create Tasks for Project 1
            var task1 = new ProjectTask("Design Homepage Mockup", "Create wireframes and high-fidelity mockups for the new homepage", project1.Id, TaskPriority.High, DateTime.UtcNow.AddDays(7));
            task1.Assign(emp1.Id);
            task1.UpdateStatus(TaskItemStatus.Done);

            var task2 = new ProjectTask("Implement Navigation", "Build responsive navigation component with dropdown menus", project1.Id, TaskPriority.High, DateTime.UtcNow.AddDays(14));
            task2.Assign(emp2.Id);
            task2.UpdateStatus(TaskItemStatus.InProgress);

            var task3 = new ProjectTask("Setup CI/CD Pipeline", "Configure GitHub Actions for automated deployment", project1.Id, TaskPriority.Medium, DateTime.UtcNow.AddDays(10));
            task3.Assign(emp1.Id);

            // Create Tasks for Project 2
            var task4 = new ProjectTask("Define App Architecture", "Plan the technical architecture and choose frameworks", project2.Id, TaskPriority.High, DateTime.UtcNow.AddDays(5));
            task4.Assign(emp3.Id);
            task4.UpdateStatus(TaskItemStatus.InProgress);

            var task5 = new ProjectTask("Design Database Schema", "Create ER diagram and define all tables and relationships", project2.Id, TaskPriority.Medium, DateTime.UtcNow.AddDays(12));
            task5.Assign(emp2.Id);

            var task6 = new ProjectTask("Create User Stories", "Write detailed user stories for sprint planning", project2.Id, TaskPriority.Low, DateTime.UtcNow.AddDays(3));
            task6.Assign(emp3.Id);
            task6.UpdateStatus(TaskItemStatus.Done);

            await context.ProjectTasks.AddRangeAsync(task1, task2, task3, task4, task5, task6);
            await context.SaveChangesAsync();

            // Add some comments
            var comment1 = new TaskComment("Looking great! The mockups are almost ready for review.", task1.Id, emp1.Id);
            var comment2 = new TaskComment("I've pushed the initial navigation component. Please review.", task2.Id, emp2.Id);
            var comment3 = new TaskComment("Let's use React Native for cross-platform support.", task4.Id, manager2.Id);

            await context.TaskComments.AddRangeAsync(comment1, comment2, comment3);
            await context.SaveChangesAsync();
        }
    }
}
