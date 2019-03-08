﻿using ExampleNetCore.Models;
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
        public async Task<ActionResult<PagedResult<TodoItem>>> GetTodoItems(string name, int? page = 1)
        {
            name = name != null ? name : "";
            var skip = (int)(page - 1) * 2;
            var results = await _context.TodoItems.Where(x=>x.Name.Contains(name)).Skip(skip).Take(2).ToListAsync();
            var rowCount = await _context.TodoItems.Where(x => x.Name.Contains(name)).CountAsync();
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


        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
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