using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryTracker.Models;

namespace InventoryTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeIngredientController : ControllerBase
    {
        private readonly InventoryTrackerContext _context;

        public RecipeIngredientController(InventoryTrackerContext context)
        {
            _context = context;
        }

        // GET: api/RecipeIngredient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeIngredient>>> GetRecipeIngredients()
        {
          if (_context.RecipeIngredients == null)
          {
              return NotFound();
          }
            return await _context.RecipeIngredients.ToListAsync();
        }

        // GET: api/RecipeIngredient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeIngredient>> GetRecipeIngredient(int id)
        {
          if (_context.RecipeIngredients == null)
          {
              return NotFound();
          }
            var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);

            if (recipeIngredient == null)
            {
                return NotFound();
            }

            return recipeIngredient;
        }

        // PUT: api/RecipeIngredient/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipeIngredient(int id, RecipeIngredient recipeIngredient)
        {
            if (id != recipeIngredient.RecipeIngredientId)
            {
                return BadRequest();
            }

            _context.Entry(recipeIngredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeIngredientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RecipeIngredient
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RecipeIngredient>> PostRecipeIngredient(RecipeIngredient recipeIngredient)
        {
          if (_context.RecipeIngredients == null)
          {
              return Problem("Entity set 'InventoryTrackerContext.RecipeIngredients'  is null.");
          }
            _context.RecipeIngredients.Add(recipeIngredient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecipeIngredient", new { id = recipeIngredient.RecipeIngredientId }, recipeIngredient);
        }

        // DELETE: api/RecipeIngredient/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipeIngredient(int id)
        {
            if (_context.RecipeIngredients == null)
            {
                return NotFound();
            }
            var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);
            if (recipeIngredient == null)
            {
                return NotFound();
            }

            _context.RecipeIngredients.Remove(recipeIngredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecipeIngredientExists(int id)
        {
            return (_context.RecipeIngredients?.Any(e => e.RecipeIngredientId == id)).GetValueOrDefault();
        }
    }
}
