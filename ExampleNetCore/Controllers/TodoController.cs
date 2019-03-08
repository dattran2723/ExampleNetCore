using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleNetCore.Models;
using ExampleNetCore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        [HttpGet("page")]
        public async Task<ActionResult<PagedResult<TodoItem>>> GetTodo(int? page = 1)
        {            
            var skip = (int)(page - 1) * 2;
            var results = await _context.TodoItems.Skip(skip).Take(2).ToListAsync();
            var rowCount = _context.TodoItems.Count();
            return SetPageResult(results, (int)page, rowCount);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<TodoItem>>> GetTodoItem(string name, int? page = 1)
        {
            var skip = (int)(page - 1) * 2;
            var results = await _context.TodoItems.Where(x => x.Name.ToLower().Contains(name.ToLower())).Skip(skip).Take(2).ToListAsync();
            var rowCount = results.Count();
            return SetPageResult(results, (int)page, rowCount);
        }

        public PagedResult<TodoItem> SetPageResult(List<TodoItem> results, int page, int rowCount)
        {
            var result = new PagedResult<TodoItem>();
            result.CurrentPage = (int)page;
            result.PageSize = 2;
            result.RowCount = rowCount;

            var pageCount = (double)result.RowCount / 2;
            result.PageCount = (int)Math.Ceiling(pageCount);

            result.Results = results;
            return result;
        }

        [HttpGet("/api/todo/numberItem")]
        public ActionResult<int> GetNumberItem()
        {
            return _context.TodoItems.Count();
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

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
    }
}