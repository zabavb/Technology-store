using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using OrderAPI.Models;
using NuGet.Packaging.Signing;

namespace OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            if (_context.Orders == null)
                return NotFound();

            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/user/{receiverId}
        [HttpGet("user/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(long receiverId)
        {
            if (_context.Orders == null)
                return NotFound();

            return await _context.Orders.Where(o => o.ReceiverId.Equals(receiverId)).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(long id)
        {
            if (_context.Orders == null)
                return NotFound();

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();
            else
            {
                order.Items.ForEach(i => order.ItemsIds.Add(i.Id));
            }

            return order;
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                if (_context.Orders == null)
                    return Problem("Entity set 'Order' is null.");
                
                order.Items.ForEach(i => order.ItemsIds.Add(i.Id));

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(long id, Order model)
        {
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var order = await _context.Orders.FindAsync(id);

                if (order == null)
                    return NotFound();

                order.Items.ForEach(i => order.ItemsIds.Add(i.Id));
                order.Items = model.Items;
                order.Receiver = model.Receiver;
                order.Country = model.Country;
                order.Locality = model.Locality;
                order.Address = model.Address;
                order.DeliveryDate = model.DeliveryDate;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(id))
                        return NotFound();
                    else
                        throw;
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            if (_context.Orders == null)
                return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool OrderExists(long id)
        {
            return (_context.Orders?.Any(o => o.Id == id)).GetValueOrDefault();
        }
    }
}
