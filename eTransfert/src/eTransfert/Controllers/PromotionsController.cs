using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using eTransfert.Models;
using System;

namespace eTransfert.Controllers
{
    public class PromotionsController : Controller
    {
        private ApplicationDbContext _context;

        public PromotionsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Promotions
        public IActionResult Index()
        {
            return View(_context.Promotion.ToList());
        }

        // GET: Promotions/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Promotion promotion = _context.Promotion.Single(m => m.Id == id);
            if (promotion == null)
            {
                return HttpNotFound();
            }

            return View(promotion);
        }

        // GET: Promotions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Promotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                promotion.Id = Guid.NewGuid().ToString();
                _context.Promotion.Add(promotion);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(promotion);
        }

        // GET: Promotions/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Promotion promotion = _context.Promotion.Single(m => m.Id == id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Update(promotion);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(promotion);
        }

        // GET: Promotions/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Promotion promotion = _context.Promotion.Single(m => m.Id == id);
            if (promotion == null)
            {
                return HttpNotFound();
            }

            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            Promotion promotion = _context.Promotion.Single(m => m.Id == id);
            _context.Promotion.Remove(promotion);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
