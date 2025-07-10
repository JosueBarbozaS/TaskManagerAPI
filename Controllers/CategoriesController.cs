using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<CategoryDto>.ErrorResponse("Datos inválidos", errors));
            }

            var result = await _categoryService.CreateCategoryAsync(createCategoryDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetCategory), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<CategoryDto>.ErrorResponse("Datos inválidos", errors));
            }

            var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}