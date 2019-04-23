using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
            };
            SetupActivitiesSelectListItems();
            return View(entry);
        }
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            //if there arent any duration field validation errors
            //then make sure that the duration is greater than 0.
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);
                TempData["Message"] = "Your entry was successfully added!";
                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();
            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // TODO Get the requested entry from the repository
            Entry entry = _entriesRepository.GetEntry((int)id);

            // TODO Return a status of not found if no entry is found
            if (entry == null) return HttpNotFound();

            //TODO  Populate the activities select list items Viewbag Property
            SetupActivitiesSelectListItems();
            //TODO pass entry to view
            return View(entry);
        }
        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            // TODO validate the entry
            ValidateEntry(entry);

            // TODO if the entry is valid
            //1) use the repository to update the entry
            //2) Redirect the user to the "Entries list Page"
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                TempData["Message"] = "Your entry was successfully updated!";
                return RedirectToAction("Index");
            }

            //TODO Populate the activities select list items viewBag Property
            SetupActivitiesSelectListItems();
            return View(entry);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO Retrieve entry for the provided id parameter value
            Entry entry = _entriesRepository.GetEntry((int)id);

            //TODO Return Not found if entry not found
            if(entry==null)
            {
                return HttpNotFound();
            }
            //Pass the entry to the View

            return View(entry);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //TODO Delete the entry
            _entriesRepository.DeleteEntry(id);

            TempData["Message"] = "Your entry was successfully deleted!";
            //TODO redirect to the "Entries list page"

            return RedirectToAction("Index");
        }
        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListitems = new SelectList(Data.Data.Activities, "ID", "Name");
        }

        private void ValidateEntry(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Durarion field value must be greater than 0");
            }
        }
    }
}