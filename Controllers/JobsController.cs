using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTJOB.Models;
using Microsoft.AspNetCore.Authorization;

namespace FPTJOB.Controllers
{
    public class JobsController : Controller
    {
        private readonly DBMyContext _context;

        public JobsController(DBMyContext context)
        {
            _context = context;
        }

        // GET: Jobs
        [Authorize(Roles = "Admin, Employer")]
        public async Task<IActionResult> Index()
        {
            var dBMyContext = _context.Jobs.Include(j => j.Category);
            return View(await dBMyContext.ToListAsync());
        }

        [Authorize(Roles = "Admin, Seeker")]
        public async Task<IActionResult> ListJob()
        {
            var dBMyContext = _context.Jobs.Include(j => j.Category).Where(j => j.Deadline >= DateTime.Now);
            return View(await dBMyContext.ToListAsync());
        }

        // GET: Jobs/Details/5
        [Authorize(Roles = "Admin, Seeker")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            var proJob = _context.Profile_Job.Include(p => p.Profile).Where(p => p.JobId == id);
            var profile = _context.Profiles.Where(p => p.UserID == User.Identity.Name).FirstOrDefault();

            if (proJob.Where(p => p.ProfileId == profile.Id).Count() > 0 && proJob.Count() > 0)

            {
                ViewBag.Apply = true;
            }
            else
            {
                ViewBag.Apply = false;
            }

            return View(job);
        }

        // GET: Jobs/Create
        [Authorize(Roles = "Admin, Employer")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Industry,Location,Description,Requirement,Deadline,CategoryId")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", job.CategoryId);
            return View(job);
        }

        // GET: Jobs/Edit/5
        [Authorize(Roles = "Admin, Employer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", job.CategoryId);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Industry,Location,Description,Requirement,Deadline,CategoryId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", job.CategoryId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        [Authorize(Roles = "Admin, Employer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'DBMyContext.Jobs'  is null.");
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
