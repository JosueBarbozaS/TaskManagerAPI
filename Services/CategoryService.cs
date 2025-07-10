using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly TaskManagerDbContext _context;

        public CategoryService(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Tasks)
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Color = c.Color,
                        CreatedAt = c.CreatedAt,
                        TaskCount = c.Tasks.Count
                    })
                    .ToListAsync();

                return ApiResponse<List<CategoryDto>>.SuccessResponse(categories);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryDto>>.ErrorResponse($"Error al obtener categorías: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Tasks)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<CategoryDto>.ErrorResponse("Categoría no encontrada");
                }

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Color = category.Color,
                    CreatedAt = category.CreatedAt,
                    TaskCount = category.Tasks.Count
                };

                return ApiResponse<CategoryDto>.SuccessResponse(categoryDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDto>.ErrorResponse($"Error al obtener categoría: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            try
            {
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == createCategoryDto.Name);

                if (existingCategory != null)
                {
                    return ApiResponse<CategoryDto>.ErrorResponse("Ya existe una categoría con ese nombre");
                }

                var category = new Category
                {
                    Name = createCategoryDto.Name,
                    Description = createCategoryDto.Description,
                    Color = createCategoryDto.Color,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Color = category.Color,
                    CreatedAt = category.CreatedAt,
                    TaskCount = 0
                };

                return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Categoría creada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDto>.ErrorResponse($"Error al crear categoría: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, CreateCategoryDto updateCategoryDto)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<CategoryDto>.ErrorResponse("Categoría no encontrada");
                }

                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == updateCategoryDto.Name && c.Id != id);

                if (existingCategory != null)
                {
                    return ApiResponse<CategoryDto>.ErrorResponse("Ya existe una categoría con ese nombre");
                }

                category.Name = updateCategoryDto.Name;
                category.Description = updateCategoryDto.Description;
                category.Color = updateCategoryDto.Color;

                await _context.SaveChangesAsync();

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Color = category.Color,
                    CreatedAt = category.CreatedAt,
                    TaskCount = await _context.Tasks.CountAsync(t => t.CategoryId == category.Id)
                };

                return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Categoría actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryDto>.ErrorResponse($"Error al actualizar categoría: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Categoría no encontrada");
                }

                // Verificar si hay tareas asociadas
                var hasAssociatedTasks = await _context.Tasks.AnyAsync(t => t.CategoryId == id);

                if (hasAssociatedTasks)
                {
                    return ApiResponse<bool>.ErrorResponse("No se puede eliminar la categoría porque tiene tareas asociadas");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Categoría eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al eliminar categoría: {ex.Message}");
            }
        }
    }
}