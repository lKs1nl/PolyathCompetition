using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyathlonCompetition.Web.Data;
using PolyathlonCompetition.Web.Models;

namespace PolyathlonCompetition.Web.Controllers
{
    [Authorize]
    public class CompetitionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompetitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Competitions
        public async Task<IActionResult> Index()
        {
            var competitions = await _context.Competitions
                .Include(c => c.CompetitionRegistrations!)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
            return View(competitions);
        }

        // GET: Competitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionRegistrations!)
                .ThenInclude(r => r.Competitor!)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (competition == null)
            {
                return NotFound();
            }

            // Статистика по регистрациям
            ViewData["ApprovedCount"] = competition.CompetitionRegistrations?
                .Count(r => r.Status == RegistrationStatus.Approved) ?? 0;
            ViewData["PendingCount"] = competition.CompetitionRegistrations?
                .Count(r => r.Status == RegistrationStatus.Pending) ?? 0;
            ViewData["TotalRegistrations"] = competition.CompetitionRegistrations?.Count ?? 0;

            return View(competition);
        }

        // GET: Competitions/Create
        [Authorize(Roles = "Admin,Organizer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Competitions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create(Competition competition)
        {
            if (ModelState.IsValid)
            {
                competition.Status = CompetitionStatus.Planned;
                _context.Add(competition);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Соревнование успешно создано!";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["ErrorMessage"] = "Ошибка при создании соревнования. Проверьте введенные данные.";
            return View(competition);
        }

        // GET: Competitions/Edit/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }
            return View(competition);
        }

        // POST: Competitions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int id, Competition competition)
        {
            if (id != competition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competition);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Соревнование успешно обновлено!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionExists(competition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            TempData["ErrorMessage"] = "Ошибка при обновлении соревнования.";
            return View(competition);
        }

        // GET: Competitions/Delete/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionRegistrations!)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (competition == null)
            {
                return NotFound();
            }

            // Проверяем есть ли связанные регистрации
            if (competition.CompetitionRegistrations?.Any() == true)
            {
                TempData["ErrorMessage"] = "Невозможно удалить соревнование, так как есть зарегистрированные участники. Сначала удалите все регистрации.";
                return RedirectToAction(nameof(Index));
            }

            return View(competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var competition = await _context.Competitions
                .Include(c => c.CompetitionRegistrations!)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (competition != null)
            {
                // Удаляем связанные регистрации сначала
                if (competition.CompetitionRegistrations?.Any() == true)
                {
                    _context.CompetitionRegistrations.RemoveRange(competition.CompetitionRegistrations);
                }
                
                _context.Competitions.Remove(competition);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Соревнование успешно удалено!";
            }
            else
            {
                TempData["ErrorMessage"] = "Соревнование не найдено.";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Competitions/OpenRegistration/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> OpenRegistration(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            competition.Status = CompetitionStatus.RegistrationOpen;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Регистрация на соревнование открыта!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Competitions/CloseRegistration/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> CloseRegistration(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            competition.Status = CompetitionStatus.Planned;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Регистрация на соревнование закрыта!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Competitions/Start/5
        [Authorize(Roles = "Admin,Organizer,Judge")]
        public async Task<IActionResult> Start(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            competition.Status = CompetitionStatus.Ongoing;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Соревнование началось!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Competitions/Complete/5
        [Authorize(Roles = "Admin,Organizer,Judge")]
        public async Task<IActionResult> Complete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            competition.Status = CompetitionStatus.Completed;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Соревнование завершено!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Competitions/Cancel/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            competition.Status = CompetitionStatus.Cancelled;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Соревнование отменено!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Competitions/Statistics/5
        [Authorize(Roles = "Admin,Organizer,Judge")]
        public async Task<IActionResult> Statistics(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionRegistrations!)
                .ThenInclude(r => r.Competitor!)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (competition == null)
            {
                return NotFound();
            }

            // Собираем статистику
            var statistics = new CompetitionStatisticsViewModel
            {
                Competition = competition,
                TotalRegistrations = competition.CompetitionRegistrations?.Count ?? 0,
                ApprovedRegistrations = competition.CompetitionRegistrations?
                    .Count(r => r.Status == RegistrationStatus.Approved) ?? 0,
                PendingRegistrations = competition.CompetitionRegistrations?
                    .Count(r => r.Status == RegistrationStatus.Pending) ?? 0,
                RejectedRegistrations = competition.CompetitionRegistrations?
                    .Count(r => r.Status == RegistrationStatus.Rejected) ?? 0,
                CancelledRegistrations = competition.CompetitionRegistrations?
                    .Count(r => r.Status == RegistrationStatus.Cancelled) ?? 0,
                Teams = competition.CompetitionRegistrations?
                    .Where(r => r.Status == RegistrationStatus.Approved)
                    .Select(r => r.Competitor?.Team)
                    .Where(team => !string.IsNullOrEmpty(team))
                    .Distinct()
                    .ToList() ?? new List<string?>(),
                Categories = competition.CompetitionRegistrations?
                    .Where(r => r.Status == RegistrationStatus.Approved)
                    .Select(r => r.Competitor?.Category)
                    .Where(category => !string.IsNullOrEmpty(category))
                    .Distinct()
                    .ToList() ?? new List<string?>()
            };

            return View(statistics);
        }

        // GET: Competitions/Search
        public async Task<IActionResult> Search(string searchString, CompetitionStatus? status)
        {
            var competitions = _context.Competitions.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                competitions = competitions.Where(c => 
                    c.Name.Contains(searchString) || 
                    c.Location.Contains(searchString) ||
                    (c.Description != null && c.Description.Contains(searchString)));
            }

            if (status.HasValue)
            {
                competitions = competitions.Where(c => c.Status == status.Value);
            }

            competitions = competitions
                .Include(c => c.CompetitionRegistrations!)
                .OrderByDescending(c => c.Date);

            ViewData["SearchString"] = searchString;
            ViewData["StatusFilter"] = status;

            return View("Index", await competitions.ToListAsync());
        }

        // GET: Competitions/Upcoming
        public async Task<IActionResult> Upcoming()
        {
            var upcomingCompetitions = await _context.Competitions
                .Where(c => c.Date >= DateTime.Today && c.Status != CompetitionStatus.Cancelled)
                .OrderBy(c => c.Date)
                .Include(c => c.CompetitionRegistrations!)
                .ToListAsync();

            ViewData["Title"] = "Предстоящие соревнования";
            return View("Index", upcomingCompetitions);
        }

        // GET: Competitions/Completed
        public async Task<IActionResult> Completed()
        {
            var completedCompetitions = await _context.Competitions
                .Where(c => c.Status == CompetitionStatus.Completed)
                .OrderByDescending(c => c.Date)
                .Include(c => c.CompetitionRegistrations!)
                .ToListAsync();

            ViewData["Title"] = "Завершенные соревнования";
            return View("Index", completedCompetitions);
        }

        private bool CompetitionExists(int id)
        {
            return _context.Competitions.Any(e => e.Id == id);
        }
    }

    // ViewModel для статистики
    public class CompetitionStatisticsViewModel
    {
        public Competition Competition { get; set; } = null!;
        public int TotalRegistrations { get; set; }
        public int ApprovedRegistrations { get; set; }
        public int PendingRegistrations { get; set; }
        public int RejectedRegistrations { get; set; }
        public int CancelledRegistrations { get; set; }
        public List<string?> Teams { get; set; } = new List<string?>();
        public List<string?> Categories { get; set; } = new List<string?>();
        
        public double ApprovalRate => TotalRegistrations > 0 ? 
            (double)ApprovedRegistrations / TotalRegistrations * 100 : 0;
    }
}