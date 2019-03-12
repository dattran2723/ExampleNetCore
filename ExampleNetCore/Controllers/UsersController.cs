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
        //public UsersController() { }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<List<UserViewModel>>> GetUsers()
        {
            List<UserViewModel> list = new List<UserViewModel>();
            var users = await _context.Users.ToListAsync();
            foreach (var item in users)
            {
                UserViewModel userView = MapperToUserViewModel(item);
                userView.ListTodo = await GetTodosByUserId(item.Id);
                list.Add(userView);
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
            UserViewModel userView = MapperToUserViewModel(user);
            userView.ListTodo = await GetTodosByUserId(user.Id);

            return userView;
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
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
            return await _context.TodoItems.Where(x => x.UserIdAssign == id).ToListAsync();
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
