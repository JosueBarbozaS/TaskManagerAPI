using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, CreateCategoryDto updateCategoryDto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    }
}