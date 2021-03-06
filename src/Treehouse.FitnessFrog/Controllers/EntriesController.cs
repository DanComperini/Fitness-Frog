﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();                      // instantiating a class of EntriesRepository 
        }

        public ActionResult Index()                                            // Index Actionmethod
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
            //ModelState.AddModelError("","This is a global message");

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



        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            // TODO Validate the entry.
            ValidateEntry(entry);

            // TODO If the entry is valid...
            // 1) Use the repository to update the entry
            // 2) Redirect the user to the "Entries" list page/
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);

                TempData["Message"] = "Your entry was successfully updated!";

                return RedirectToAction("Index");
            }


            // TODO Populate the activities select list items ViewBag property
            SetupActivitiesSelectListItems();

            return View(entry);
        }


        public ActionResult Edit(int? id)                                     // '?' allows id to be nullable
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get the requested entry from the repository.
            Entry entry = _entriesRepository.GetEntry((int)id);

            // TODO Return a status of not found if the entry wasn't found
            if (entry == null)
            {
                return HttpNotFound();
            }

            SetupActivitiesSelectListItems();

            // TODO Pass the entry into the view
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Retrieve entry from the provided id parameter value. 
            Entry entry = _entriesRepository.GetEntry((int)id);

            // TODO Return "not found" if the entry not found

            if (entry == null)
            {
                return HttpNotFound();
            }


            // TODO Pass the entry into the view
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO Delete the entry
            _entriesRepository.DeleteEntry(id);

            // TODO Redirect to the "Entries" list page

            TempData["Message"] = "Your entry was successfully deleted!";

            return RedirectToAction("Index");

            return null;
        }

        private void ValidateEntry(Entry entry)
        {
            // If there are not any duration field validation errors then make sure the duration > 0
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                Data.Data.Activities, "Id", "Name");
        }

    }
}