using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public class TaskManagerDbContext : DbContext
    {
        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Color).HasMaxLength(7).HasDefaultValue("#3498db");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Task Configuration
            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IsCompleted).HasDefaultValue(false);
                entity.Property(e => e.Priority).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Foreign Key Relationships
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Tasks)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Indexes
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.IsCompleted);
                entity.HasIndex(e => e.DueDate);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Trabajo", Description = "Tareas relacionadas con el trabajo", Color = "#e74c3c" },
                new Category { Id = 2, Name = "Personal", Description = "Tareas personales", Color = "#2ecc71" },
                new Category { Id = 3, Name = "Estudios", Description = "Tareas académicas", Color = "#f39c12" },
                new Category { Id = 4, Name = "Hogar", Description = "Tareas domésticas", Color = "#9b59b6" }
            );

            // Seed Users (contraseña: password123)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@taskmanager.com",
                    Password = "$2a$11$8H5K5J5K5J5K5J5K5J5K5OeRYwjJq1q1q1q1q1q1q1q1q1q1q1q1q", // hashed password
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "johndoe",
                    Email = "john@example.com",
                    Password = "$2a$11$8H5K5J5K5J5K5J5K5J5K5OeRYwjJq1q1q1q1q1q1q1q1q1q1q1q1q",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed Tasks
            modelBuilder.Entity<Models.Task>().HasData(
                new Models.Task
                {
                    Id = 1,
                    Title = "Completar proyecto API",
                    Description = "Desarrollar API REST con C# y Entity Framework",
                    IsCompleted = false,
                    Priority = 3,
                    DueDate = DateTime.UtcNow.AddDays(6),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = 1,
                    CategoryId = 1
                },
                new Models.Task
                {
                    Id = 2,
                    Title = "Comprar víveres",
                    Description = "Comprar ingredientes para la cena",
                    IsCompleted = false,
                    Priority = 2,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = 1,
                    CategoryId = 4
                },
                new Models.Task
                {
                    Id = 3,
                    Title = "Estudiar para examen",
                    Description = "Repasar conceptos de programación",
                    IsCompleted = false,
                    Priority = 3,
                    DueDate = DateTime.UtcNow.AddDays(3),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = 2,
                    CategoryId = 3
                }
            );
        }
    }
}