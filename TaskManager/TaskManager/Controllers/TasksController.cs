using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskManagerContext _context;
        DateTime dt = new DateTime(2017, 01, 03);
        
        public int GetOfWeek(DateTime dt)
        {
            System.Globalization.CultureInfo cult_info = System.Globalization.CultureInfo.CreateSpecificCulture("no");
            Calendar cal = cult_info.Calendar;
            return (cal.GetWeekOfYear(dt, cult_info.DateTimeFormat.CalendarWeekRule, cult_info.DateTimeFormat.FirstDayOfWeek));
        }

        public TasksController(TaskManagerContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var ActiveTask = _context.Task.Where(s => s.startDate.Year == DateTime.Now.Year);
            ActiveTask =  ActiveTask.Where(s => s.startDate.Hour == DateTime.Now.Hour);
            if (ActiveTask.Count()!=0) // To check whether user has active task
            {
                ViewBag.Message = "You have currently active task!";   
            }
            var context = _context.Task.Where(s => s.endDate >= DateTime.Now);
            context = context.OrderBy(s => s.startDate).Take(5); // To take upcoming 5 tasks from all future tasks
            return View(context.ToList());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,title,notes,startDate,endDate")] TaskManager.Models.Task task)
        {
            if (ModelState.IsValid)
            {
                if (task.startDate >= task.endDate)
                {
                    return Json($"A user named already exists.");
                }

                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,notes,startDate,endDate,isCompleted")] TaskManager.Models.Task task)
        {
            if (id != task.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.id))
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
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Task.FindAsync(id);
            _context.Task.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> List(string accToWhat, bool pastTask, bool completedTask) // accordingToWhat = Day / Week / Year 
        {
            ViewBag.DateTime = System.DateTime.Now; 
            ViewBag.AccTo = accToWhat;

            var context = _context.Task.Where(s => s.endDate.Year == System.DateTime.Now.Year); // To list all tasks on current year

            if (accToWhat == "Weekly")
            {
                return View(context.AsEnumerable().Where(s => GetOfWeek(s.endDate) == GetOfWeek(DateTime.Now)).ToList());
                // To list tasks by weekly
            }
            else if (accToWhat == "Daily")
            {
                context = context.Where(s => s.endDate.Day == System.DateTime.Now.Day); // To list tasks by daily
            }
            else if (accToWhat == "Monthly")
            {
                context = context.Where(s => s.endDate.Month == System.DateTime.Now.Month); // To list tasks by yearly
            }
            else if (accToWhat == "All")
            {
                context = _context.Task.Where(s => s.endDate.Year >= 0); // To list tasks by yearly
            }

            if (!pastTask)
                context = context.Where(s => s.endDate >= DateTime.Now); // To hide past task
                
            if (!completedTask)
                context = context.Where(s => s.isCompleted != true); // To hide completed task
            
            return View(context.OrderByDescending(s=>s.startDate).ToList());
        }

        private bool TaskExists(int id)
        {
            return _context.Task.Any(e => e.id == id);
        }

    }
}
