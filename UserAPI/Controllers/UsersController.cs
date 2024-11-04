﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using UserAPI.Models;
using System.Text;
using System.Security.Cryptography;
using Library.Infrastructure;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UsersController(UserDbContext context)
        {
            _context = context;
        }

        //============================= GET =============================

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
                return NotFound();

            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            if (_context.Users == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            return user;
        }

        // GET: api/Users/username/password
        [HttpGet("{username}, {password}")]
        public async Task<ActionResult<User>> GetUserByUsernamePassword(string username, string password)
        {
            if (_context.Users == null)
                return NotFound();

            string hashedPassword = UserExtension.HashPassword(password);
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Username.Equals(username) &&
                u.Password.Equals(hashedPassword)
            );

            if (user == null)
                return NotFound();

            return user;
        }

        // GET: api/Users/username/basket
        [HttpGet("{username}/basket")]
        public async Task<ActionResult<List<Product>>> GetUserBasket(string username)
        {
            if (_context.Users == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));

            if (user == null)
                return NotFound();

            return user.Basket;
        }

        //============================= POST =============================

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users == null)
                    return Problem("Entity set 'Users' is null.");
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                return Ok();

            }
            return BadRequest(ModelState);
        }

        // POST: api/Users/username/basket
        [HttpPost("{username}/basket")]
        public async Task<ActionResult<User>> PostToBasket(string username, Product product)
        {
            if (_context.Users == null)
                return NotFound();

            var user = _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(username)).Result;
            if (user == null)
                return NotFound();

            user.Basket.Add(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //============================= PUT =============================

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User model)
        {
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                    return NotFound();

                user.Username = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Age = model.Age;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Password = model.Password;
                user.Role = model.Role;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                        return NotFound();
                    else
                        throw;
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }

        //============================= DELETE =============================

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (_context.Users == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Users/username/basket/id
        [HttpDelete("{username}/{id}")]
        public async Task<IActionResult> DeleteFromUserBasket(string username, long id)
        {
            if (_context.Users == null)
                return NotFound();

            var user = _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(username)).Result;
            if (user == null)
                return NotFound();

            var product = user.Basket.FirstOrDefault(p => p.Id.Equals(id));
            if (product == null)
                return NotFound();

            user.Basket.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //============================= NonAction =============================

        [NonAction]
        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
