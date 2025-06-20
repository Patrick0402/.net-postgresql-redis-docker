using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using ProductsApi.Services;
using ProductsApi.Repository.Products;
using ProductsApi.Models.DTOs;
using System.Globalization;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductInterface _repository;
        private readonly ProductCacheService _cacheService;

        public ProductsController(IProductInterface repository, ProductCacheService cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price
            };

            var created = await _repository.AddAsync(product);

            var readDto = new ProductReadDto
            {
                Id = created.Id,
                Name = created.Name,
                Price = created.Price,
                ServerTime = DateTime.Now
            };

            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, readDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync();

            var readDtos = products.Select(p => new ProductReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();

            return Ok(readDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var cacheKey = $"product:{id}";

            var cachedProduct = await _cacheService.GetCachedProductAsync<ProductReadDto>(cacheKey);
            if (cachedProduct != null)
                return Ok(cachedProduct);

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var readDto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };

            await _cacheService.CacheProductAsync(cacheKey, readDto);

            return Ok(readDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCreateDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            product.Name = dto.Name;
            product.Price = dto.Price;

            await _repository.UpdateAsync(product);

            var readDto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };

            var cacheKey = $"product:{id}";
            await _cacheService.CacheProductAsync(cacheKey, readDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            await _repository.DeleteAsync(product);

            var cacheKey = $"product:{id}";
            await _cacheService.RemoveProductFromCacheAsync(cacheKey);

            return NoContent();
        }
    }
}
