using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zipkin.Models;
using NLog;

namespace zipkin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private static readonly Logger logger = LogManager.GetLogger(nameof(TodoController));
        private readonly TodoContext _todoContext;

        public TodoController(TodoContext todoContext) {
            _todoContext = todoContext;

            if (_todoContext.TodoItems.Count() == 0) {
                _todoContext.TodoItems.Add(new TodoItem {
                    name = "Figure out OpenZipkin"
                });
                _todoContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems() {
            logger.Info("Finding all Todo items");
            return await _todoContext.TodoItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id) {
            logger.Info($"Looking for Todo Item id {id}");
            var item = await _todoContext.TodoItems.FindAsync(id);

            if (item == null) {
                logger.Error("Todo item not found!");
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item) {
            _todoContext.TodoItems.Add(item);
            await _todoContext.SaveChangesAsync();

            logger.Info($"Added Todo item: {item}");

            return CreatedAtAction(nameof(GetTodoItem), new { id = item.id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem item) {
            if (id != item.id)
                return BadRequest();

            _todoContext.Entry(item).State = EntityState.Modified;
            await _todoContext.SaveChangesAsync();

            logger.Info($"Updated Todo item {id}: {item}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id) {
            var item = await _todoContext.TodoItems.FindAsync(id);

            if (item == null) {
                logger.Error("Todo item not found!");
                return NotFound();
            }

            _todoContext.TodoItems.Remove(item);
            await _todoContext.SaveChangesAsync();

            logger.Info($"Removed Todo item {id}");

            return NoContent();
        }
    }
}