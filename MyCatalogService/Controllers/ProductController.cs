using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCatalogService.Data;
using MyCatalogService.Filter;
using MyCatalogService.Model;
using MyCatalogService.Model.Request;
using MyCatalogService.Model.Response;

namespace MyCatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MyCatalogServiceAPIDbContext dbContext;
        private PaginationFilter paginationFilter;

        public ProductController(MyCatalogServiceAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
            paginationFilter = new PaginationFilter();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await dbContext.Product.ToListAsync();
            List<ProductResponse> responses = new List<ProductResponse>();
            foreach (var product in products)
            {
                responses.Add(CreateProductResponse(product));
            }

            return Ok(responses);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await dbContext.Product.FindAsync(id);
            if (product != null)
            {
                return Ok(CreateProductResponse(product));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductRequest product)
        {
            var newproduct = new Product()
            {
                Name = product.Name,
                CategoryId = product.CategoryId
            };
            try
            {
                await dbContext.Product.AddAsync(newproduct);
                await dbContext.SaveChangesAsync();
                return Ok(CreateProductResponse(newproduct));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, UpdateProductRequest updateproduct)
        {
            var product = await dbContext.Product.FindAsync(id);
            if (product != null)
            {
                product.Name = updateproduct.Name;
                product.CategoryId = updateproduct.CategoryId;
                await dbContext.SaveChangesAsync();
                return Ok(CreateProductResponse(product));
            }
            return NotFound();
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await dbContext.Product.FindAsync(id);
            if (product != null)
            {
                dbContext.Remove(product);
                await dbContext.SaveChangesAsync();
                return Ok(CreateProductResponse(product));
            }
            return NotFound();
        }

        [HttpGet]
        [Route("ItemByCategory/{id:int}")]
        public async Task<IActionResult> GetProductByCategoryId([FromRoute] int id)
        {
            var products = await dbContext.Product.Where(product => product.CategoryId == id)
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
               .Take(paginationFilter.PageSize).ToListAsync();

            List<ProductResponse> responses = new List<ProductResponse>();
            if (products.Count != 0)
            {
                foreach (var product in products)
                {
                    responses.Add(CreateProductResponse(product));
                }
                return Ok(responses);
            }
            return NotFound();
        }

        private ProductResponse CreateProductResponse(Product product)
        {
            return new ProductResponse { Id = product.Id, Name = product.Name, CategoryId = product.CategoryId };
        }
    }
}
