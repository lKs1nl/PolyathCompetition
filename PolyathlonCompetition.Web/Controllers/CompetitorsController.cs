using Microsoft.AspNetCore.Mvc;
using PolyathlonCompetition.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace PolyathlonCompetition.Web.Controllers
{
    public class CompetitorsController : Controller
    {
        private static List<Competitor> _competitors = new List<Competitor>();
        private static int _nextId = 1;

        public IActionResult Index()
        {
            return View(_competitors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Competitor competitor)
        {
            if (ModelState.IsValid)
            {
                competitor.Id = _nextId++;
                competitor.CompetitorNumber = $"P{competitor.Id:000}";
                _competitors.Add(competitor);
                return RedirectToAction(nameof(Index));
            }
            return View(competitor);
        }

        public IActionResult Edit(int id)
        {
            var competitor = _competitors.FirstOrDefault(c => c.Id == id);
            if (competitor == null)
                return NotFound();
            
            return View(competitor);
        }

        [HttpPost]
        public IActionResult Edit(Competitor competitor)
        {
            if (ModelState.IsValid)
            {
                var existingCompetitor = _competitors.FirstOrDefault(c => c.Id == competitor.Id);
                if (existingCompetitor != null)
                {
                    existingCompetitor.FirstName = competitor.FirstName;
                    existingCompetitor.LastName = competitor.LastName;
                    existingCompetitor.MiddleName = competitor.MiddleName;
                    existingCompetitor.BirthDate = competitor.BirthDate;
                    existingCompetitor.Team = competitor.Team;
                    existingCompetitor.Category = competitor.Category;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(competitor);
        }

        public IActionResult Delete(int id)
        {
            var competitor = _competitors.FirstOrDefault(c => c.Id == id);
            if (competitor == null)
                return NotFound();
            
            return View(competitor);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var competitor = _competitors.FirstOrDefault(c => c.Id == id);
            if (competitor != null)
            {
                _competitors.Remove(competitor);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}