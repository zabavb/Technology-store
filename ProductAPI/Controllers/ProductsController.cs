using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using ProductAPI.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ProductsController(ProductDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (_context.Products == null)
                return NotFound();

            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            if (_context.Products == null)
                return NotFound();

            var model = await _context.Products.FindAsync(id);

            if (model == null)
                return NotFound();

            return model;
        }

        // GET: api/Products/many?5&ids=6&ids=7
        [HttpGet("many")]
        public ActionResult<List<Product>> GetProductsByIds([FromQuery] string[] ids)
        {
            if (_context.Products == null)
                return NotFound();

            List<Product> products = new();
            ids.ToList().ForEach(id => {
                products.Add(_context.Products.FirstOrDefaultAsync(p => p.Id.ToString().Equals(id)).Result!);
            });

            if (products.Count == 0)
                return NotFound();

            return products;
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Products == null)
                    return Problem("Entity set 'Products' is null.");
                _context.Products.Add(model);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product model)
        {
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                    return NotFound();

                product.Brand = model.Brand;
                product.Model = model.Model;
                product.Name = model.Brand + " " + model.Model;
                product.Producer = model.Producer;
                product.Price = model.Price;
                product.Details = model.Details;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                        return NotFound();
                    else
                        throw;
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            if (_context.Products == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ProductExists(long id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
