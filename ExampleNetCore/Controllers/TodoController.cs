using ExampleNetCore.Models;
using ExampleNetCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }
        
        [HttpGet("get")]
        public async Task<ActionResult<PagedResult<TodoViewModel>>> GetTodoItems(string name, int? page = 1)
        {
            name = name != null ? name : "";
            var skip = (int)(page - 1) * 2;
            var results = await _context.TodoItems.Where(x=>x.Name.Contains(name)).Skip(skip).Take(2).ToListAsync();
            var rowCount = await _context.TodoItems.Where(x => x.Name.Contains(name)).CountAsync();

            List<TodoViewModel> list = new List<TodoViewModel>();
            foreach (var item in results)
            {
                list.Add(new TodoViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsComplete = item.IsComplete,
                    UserAssign = MapperUserToUserViewModel(await GetUserById(item.UserIdAssign))
                });
            }

            return SetPageResult(list, (int)page, rowCount);
        }

        public PagedResult<TodoViewModel> SetPageResult(List<TodoViewModel> results, int page, int rowCount)
        {
            var result = new PagedResult<TodoViewModel>();
            result.CurrentPage = (int)page;
            result.PageSize = 2;
            result.RowCount = rowCount;

            var pageCount = (double)result.RowCount / 2;
            result.PageCount = (int)Math.Ceiling(pageCount);

            result.Results = results;
            return result;
        }


        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoViewModel>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }
            var user = await GetUserById(todoItem.UserIdAssign);
            if (user == null)
                return NotFound();

            return new TodoViewModel
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete,
                UserAssign = MapperUserToUserViewModel(user)
            };
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoViewModel>> PostTodoItem(TodoItem item)
        {
            var user = await GetUserById(item.UserIdAssign);
            if (user == null)
                return NotFound();

            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return new TodoViewModel
            {
                Id = item.Id,
                Name = item.Name,
                IsComplete = item.IsComplete,
                UserAssign = MapperUserToUserViewModel(user)
            };
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            if (GetUserById(item.UserIdAssign) == null)
                return NotFound();

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<User> GetUserById(long id)
        {
            return await _context.Users.FindAsync(id);
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