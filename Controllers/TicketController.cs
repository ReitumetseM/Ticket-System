using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmnitakSupportHub.Models;
using OmnitakSupportHub.Models.ViewModels;

namespace OmnitakSupportHub.Controllers
{
    public class TicketController : Controller
    {
        private readonly OmnitakContext _context;

        public TicketController(OmnitakContext context)
        {
            _context = context;
        }

        // GET: /Ticket
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.AssignedToUser)
                .ToListAsync();

            return View(tickets);
        }

        // GET: /Ticket/Assign/5
        public async Task<IActionResult> Assign(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.TicketID == id);

            if (ticket == null)
                return NotFound();

            var agents = await _context.Users
                .Where(u => u.IsActive && u.Role!.RoleName == "Support Agent")
                .Select(u => new SelectListItem
                {
                    Value = u.UserID.ToString(),
                    Text = u.FullName
                }).ToListAsync();

            var viewModel = new AssignTicketViewModel
            {
                TicketID = ticket.TicketID,
                CurrentAssignee = ticket.AssignedToUser?.FullName,
                AvailableAgents = agents
            };

            return View(viewModel);
        }

        // POST: /Ticket/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignTicketViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ticket = await _context.Tickets.FindAsync(model.TicketID);
            if (ticket == null)
                return NotFound();

            ticket.AssignedTo = model.SelectedAgentID;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ticket successfully assigned!";
            return RedirectToAction(nameof(Index));
        }
        // GET: /Ticket/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.TicketID == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: /Ticket/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
