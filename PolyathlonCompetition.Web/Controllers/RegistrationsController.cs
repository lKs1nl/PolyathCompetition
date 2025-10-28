using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyathlonCompetition.Web.Data;
using PolyathlonCompetition.Web.Models;

namespace PolyathlonCompetition.Web.Controllers
{
    [Authorize]
    public class RegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Регистрации для соревнования
        [Authorize(Roles = "Admin,Organizer,Judge")]
        public async Task<IActionResult> Index(int? competitionId)
        {
            if (competitionId == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionRegistrations!)
                .ThenInclude(r => r.Competitor!)
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            if (competition == null)
            {
                return NotFound();
            }

            ViewData["CompetitionName"] = competition.Name;
            ViewData["CompetitionId"] = competition.Id;

            var registrations = competition.CompetitionRegistrations?.ToList() ?? new List<CompetitionRegistration>();
            return View(registrations);
        }

        // GET: Регистрация участника на соревнование
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create(int competitionId)
        {
            var competition = await _context.Competitions.FindAsync(competitionId);
            if (competition == null)
            {
                return NotFound();
            }

            ViewData["CompetitionName"] = competition.Name;
            ViewData["CompetitionId"] = competition.Id;

            // Получаем список участников, которые еще не зарегистрированы на это соревнование
            var registeredCompetitorIds = await _context.CompetitionRegistrations
                .Where(r => r.CompetitionId == competitionId)
                .Select(r => r.CompetitorId)
                .ToListAsync();

            var availableCompetitors = await _context.Competitors
                .Where(c => !registeredCompetitorIds.Contains(c.Id))
                .ToListAsync();

            ViewData["CompetitorId"] = availableCompetitors
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.LastName} {c.FirstName} {c.MiddleName} ({c.CompetitorNumber})"
                })
                .ToList();

            return View();
        }

        // POST: Регистрация участника на соревнование
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create([Bind("CompetitionId,CompetitorId,Notes")] CompetitionRegistration registration)
        {
            if (ModelState.IsValid)
            {
                // Проверяем, не зарегистрирован ли уже участник
                var existingRegistration = await _context.CompetitionRegistrations
                    .FirstOrDefaultAsync(r => r.CompetitionId == registration.CompetitionId && 
                                            r.CompetitorId == registration.CompetitorId);

                if (existingRegistration != null)
                {
                    ModelState.AddModelError("", "Участник уже зарегистрирован на это соревнование.");
                }
                else
                {
                    registration.RegistrationDate = DateTime.UtcNow;
                    registration.Status = RegistrationStatus.Approved;

                    _context.Add(registration);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Competitions", new { id = registration.CompetitionId });
                }
            }

            // Если что-то пошло не так, заново заполняем ViewData
            var competition = await _context.Competitions.FindAsync(registration.CompetitionId);
            ViewData["CompetitionName"] = competition?.Name;
            ViewData["CompetitionId"] = registration.CompetitionId;

            var availableCompetitors = await _context.Competitors.ToListAsync();
            ViewData["CompetitorId"] = availableCompetitors
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.LastName} {c.FirstName} {c.MiddleName} ({c.CompetitorNumber})"
                })
                .ToList();

            return View(registration);
        }

        // GET: Изменение статуса регистрации
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.CompetitionRegistrations
                .Include(r => r.Competition!)
                .Include(r => r.Competitor!)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: Изменение статуса регистрации
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompetitionId,CompetitorId,Status,Notes")] CompetitionRegistration registration)
        {
            if (id != registration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRegistration = await _context.CompetitionRegistrations.FindAsync(id);
                    if (existingRegistration != null)
                    {
                        existingRegistration.Status = registration.Status;
                        existingRegistration.Notes = registration.Notes;
                        
                        _context.Update(existingRegistration);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationExists(registration.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Competitions", new { id = registration.CompetitionId });
            }
            return View(registration);
        }

        // GET: Удаление регистрации
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registration = await _context.CompetitionRegistrations
                .Include(r => r.Competition!)
                .Include(r => r.Competitor!)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: Удаление регистрации
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.CompetitionRegistrations.FindAsync(id);
            if (registration != null)
            {
                var competitionId = registration.CompetitionId;
                _context.CompetitionRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Competitions", new { id = competitionId });
            }
            return RedirectToAction("Index", "Competitions");
        }

        private bool RegistrationExists(int id)
        {
            return _context.CompetitionRegistrations.Any(e => e.Id == id);
        }
    }
}