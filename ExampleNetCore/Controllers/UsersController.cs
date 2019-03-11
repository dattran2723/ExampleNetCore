using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExampleNetCore.Models;
using ExampleNetCore.ViewModels;

namespace ExampleNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TodoContext _context;

        public UsersController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<List<UserViewModel>>> GetUsers()
        {
            List<UserViewModel> list = new List<UserViewModel>();
            var users = await _context.Users.ToListAsync();
            foreach (var item in users)
            {
                list.Add(MapperUserToUserViewModel(item));
            }
            return list;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return MapperUserToUserViewModel(user);
        }
        

        // POST: api/Users
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<UserViewModel>> Create(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(user);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapperUserToUserViewModel(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(long id, User user)
        {
            if (id != user.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!UserExists(id))
                return NotFound();

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserViewModel>> Delete(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return MapperUserToUserViewModel(user);
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        private UserViewModel MapperUserToUserViewModel(User user)
        {
            var dateTimeOffset = new DateTimeOffset(user.DateOfBirth);
            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DateOfBirth = dateTimeOffset.ToUnixTimeSeconds(),
                Gender = user.Gerder,
                Phone = user.Phone,
                Address = user.Address
            };
        }
    }
}
