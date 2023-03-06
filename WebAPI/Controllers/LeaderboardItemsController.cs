using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/LeaderboardItems")]
    [ApiController]
    public class LeaderboardItemsController : ControllerBase
    {
        private readonly LeaderboardContext _context;

        public LeaderboardItemsController(LeaderboardContext context)
        {
            _context = context;
        }

        // GET: api/LeaderboardItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaderboardItem>>> GetLeaderboardItems()
        {
            return await _context.LeaderboardItems.ToListAsync();
        }

        // GET: api/LeaderboardItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaderboardItem>> GetLeaderboardItem(long id)
        {
            var leaderboardItem = await _context.LeaderboardItems.FindAsync(id);

            if (leaderboardItem == null)
            {
                return NotFound();
            }

            return leaderboardItem;
        }

        // PUT: api/LeaderboardItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaderboardItem(long id, LeaderboardItem leaderboardItem)
        {
            if (id != leaderboardItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(leaderboardItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaderboardItemExists(id))
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

        // POST: api/LeaderboardItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LeaderboardItem>> PostLeaderboardItem(LeaderboardItem leaderboardItem)
        {
            _context.LeaderboardItems.Add(leaderboardItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeaderboardItem), new { id = leaderboardItem.Id }, leaderboardItem);
        }

        // DELETE: api/LeaderboardItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaderboardItem(long id)
        {
            var leaderboardItem = await _context.LeaderboardItems.FindAsync(id);
            if (leaderboardItem == null)
            {
                return NotFound();
            }

            _context.LeaderboardItems.Remove(leaderboardItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaderboardItemExists(long id)
        {
            return _context.LeaderboardItems.Any(e => e.Id == id);
        }
    }
}
