using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    // One reusable CRUD controller for every master table.
    // Records are never hard-deleted (old bills refer to them) - they are activated / deactivated.
    public abstract class MasterController<T> : Controller where T : class, new()
    {
        protected readonly BillingDbContext Db;
        protected MasterController(BillingDbContext db) { Db = db; }

        protected abstract string Heading { get; }
        protected virtual IQueryable<T> Query() => Db.Set<T>();
        protected virtual Task LoadLookups() => Task.CompletedTask;

        private async Task<IActionResult> Form(T model)
        {
            ViewBag.Heading = Heading;
            await LoadLookups();
            return View("Form", model);
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Heading = Heading;
            return View(await Query().ToListAsync());
        }

        [HttpGet] public Task<IActionResult> Create() => Form(new T());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(T model)
        {
            if (!ModelState.IsValid) return await Form(model);
            Db.Add(model);
            await Db.SaveChangesAsync();
            TempData["Msg"] = "Saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var e = await Db.Set<T>().FindAsync(id);
            if (e == null) return NotFound();
            return await Form(e);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(T model)
        {
            if (!ModelState.IsValid) return await Form(model);
            Db.Update(model);
            await Db.SaveChangesAsync();
            TempData["Msg"] = "Updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var e = await Db.Set<T>().FindAsync(id);
            if (e == null) return NotFound();
            var p = Db.Entry(e).Property("IsActive");
            p.CurrentValue = !(bool)p.CurrentValue;
            await Db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
