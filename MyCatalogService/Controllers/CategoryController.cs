using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCatalogService.Data;
using MyCatalogService.Model;
using MyCatalogService.Model.Request;
using MyCatalogService.Model.Response;

namespace MyCatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyCatalogServiceAPIDbContext dbContext;

        public CategoryController(MyCatalogServiceAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await dbContext.Category.ToListAsync();
            List<CategoryResponse> responses = new List<CategoryResponse>();
            foreach (var category in categories)
            {
                responses.Add(new CategoryResponse { Id = category.Id, Name = category.Name });
            }

            return Ok(responses);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            var category = await dbContext.Category.FindAsync(id);
            if (category != null)
            {
                return Ok(category);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryRequest category)
        {
            var newCategory = new Category()
            {
                Name = category.Name
            };
            try
            {
                await dbContext.Category.AddAsync(newCategory);
                await dbContext.SaveChangesAsync();
                return Ok(newCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateCategoryName([FromRoute] int id, UpdateCategoryRequest updateCategory)
        {
            var category = await dbContext.Category.FindAsync(id);
            if (category != null)
            {
                category.Name = updateCategory.Name;

                await dbContext.SaveChangesAsync();
                return Ok(category);
            }
            return NotFound();
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var category = await dbContext.Category.FindAsync(id);
            if (category != null)
            {
                dbContext.Remove(category);
                await dbContext.SaveChangesAsync();

                return Ok(category);
            }
            return NotFound();
        }
    }
}
