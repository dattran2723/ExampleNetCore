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

        private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public UsersController(TodoContext context)
        {
            _context = context;
        }
        //public UsersController() { }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<List<UserViewModel>>> GetUsers()
        {
            var user = await _context.Users
                        .Select(x => new UserViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Email = x.Email,
                            DateOfBirth = Convert.ToInt64((x.DateOfBirth - epoch).TotalSeconds),
                            Phone = x.Phone,
                            Address = x.Address,
                            ListTodo = _context.TodoItems.Where(t => t.UserIdAssign == x.Id)
                                        .Select(t => new TodoViewModel
                                        {
                                            Id = t.Id,
                                            Name = t.Name,
                                            IsComplete = t.IsComplete
                                        }).ToList(),
                        }).ToListAsync();

            return user;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(long id)
        {
            var user = await _context.Users.Where(x => x.Id == id)
                        .Select(x => new UserViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Email = x.Email,
                            DateOfBirth = Convert.ToInt64((x.DateOfBirth - epoch).TotalSeconds),
                            Phone = x.Phone,
                            Address = x.Address,
                            ListTodo = _context.TodoItems.Where(t => t.UserIdAssign == x.Id)
                                        .Select(t => new TodoViewModel
                                        {
                                            Id = t.Id,
                                            Name = t.Name,
                                            IsComplete = t.IsComplete
                                        }).ToList(),
                        }).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        // POST: api/Users
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<UserViewModel>> Create(UserViewModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(user);


            var result = _context.Users.Add(MapperToUser(user));
            await _context.SaveChangesAsync();

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(long id, UserViewModel user)
        {
            if (id != user.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!UserExists(id))
                return NotFound();

            _context.Entry(MapperToUser(user)).State = EntityState.Modified;

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

            return MapperToUserViewModel(user);
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private UserViewModel MapperToUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DateOfBirth = Convert.ToInt64((user.DateOfBirth - epoch).TotalSeconds),
                Gender = user.Gerder,
                Phone = user.Phone,
                Address = user.Address
            };
        }

        private User MapperToUser(UserViewModel user)
        {
            return new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DateOfBirth = epoch.AddSeconds(user.DateOfBirth),
                Gerder = user.Gender,
                Phone = user.Phone,
                Address = user.Address
            };
        }

        public async Task<List<TodoItem>> GetTodosByUserId(long id)
        {
            var listTodo = await _context.TodoItems.Where(x => x.UserIdAssign == id).ToListAsync();
            return listTodo;
        }

        [HttpGet]
        [Route("checkexistingemail")]
        public async Task<ActionResult> CheckExistingEmail(string email)
        {
            if (await _context.Users.AnyAsync(x => x.Email == email))
                return Ok();
            return NotFound();
        }

        [HttpGet]
        [Route("checkexistingphone")]
        public async Task<ActionResult> CheckExistingPhone(string phone)
        {
            if (await _context.Users.AnyAsync(x => x.Phone == phone))
                return Ok();
            return NotFound();
        }
    }
}
