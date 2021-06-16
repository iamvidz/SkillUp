using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkillUp.Data;
using SkillUp.EF;
using SkillUp.Models;

namespace SkillUp.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Browse()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);
            //var applicationDbContext = _context.Courses.Include(c => c.Teacher);
            return View(await _context.Courses.Where(x=>x.TeacherId== userId).ToListAsync());
        }

        public async Task<IActionResult> Enroll(Guid id)
        {
            var course = await _context.Courses.Include(x => x.Teacher).Include(c => c.CourseStudents)
                .ThenInclude(v => v.Student).SingleOrDefaultAsync(m => m.CourseId == id);
            string userId = _userManager.GetUserId(User);
            if(course.TeacherId==userId || course.CourseStudents.Any(x=>x.StudentId == userId))
            {
                return View("Playlist", course.WistiaId);
            }
            else
            {
                return View(course);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(Guid? courseId=null)
        {
            var course = _context.Courses.FirstOrDefault(x => x.CourseId == courseId);
            course.Revenue += course.Price;
            string userId = _userManager.GetUserId(User);
            var model = new CourseStudent();
            model.StudentId = userId;
            model.CourseId = course.CourseId;
            _context.CourseStudents.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Enroll", new { id = courseId });
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewBag.TeacherId = _userManager.GetUserId(User);
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<IActionResult> Create([Bind("CourseId,Title,Description,WistiaId,Price,TeacherId,ImagePath,Revenue")] Course course, IFormFile formFile)
        {
            if (ModelState.IsValid)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath + "\\Files\\", fileName);
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        course.ImagePath = "/Files/" + fileName;
                        await formFile.CopyToAsync(stream);
                    }
                }

                course.CourseId = Guid.NewGuid();
                _context.Add(course);
                if(User.IsInRole(Helper.EnumRole.Instructor)==false)
                {
                    var v = await _userManager.AddToRoleAsync(await _userManager.GetUserAsync(User), Helper.EnumRole.Instructor);
                    Helper.EnumRole.IsInstructor = true;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CourseId,Title,Description,WistiaId,Price,TeacherId,ImagePath,Revenue")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
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
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(Guid id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}
