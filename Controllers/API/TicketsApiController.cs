using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmnitakSupportHub.Models;
using OmnitakSupportHub.Models.ViewModels;

namespace OmnitakSupportHub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsApiController : ControllerBase
    {
        private readonly OmnitakContext _context;

        public TicketsApiController(OmnitakContext context)
        {
            _context = context;
        }

        // GET: api/tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            return await _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .ToListAsync();
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();
            return ticket;
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket(TicketDto dto)
        {
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                StatusID = dto.StatusID,
                PriorityID = dto.PriorityID,
                CategoryID = dto.CategoryID,
                CreatedBy = dto.CreatedBy,
                AssignedTo = dto.AssignedTo,
                TeamID = dto.TeamID,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.TicketID }, ticket);
        }

        // PUT: api/tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, TicketDto dto)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.Title = dto.Title;
            ticket.Description = dto.Description;
            ticket.StatusID = dto.StatusID;
            ticket.PriorityID = dto.PriorityID;
            ticket.CategoryID = dto.CategoryID;
            ticket.AssignedTo = dto.AssignedTo;
            ticket.TeamID = dto.TeamID;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
