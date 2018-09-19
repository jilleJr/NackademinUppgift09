﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nauktion.Helpers;
using Nauktion.Models;
using Nauktion.Services;

namespace Nauktion.Controllers
{
    [AuthorizeRole(NauktionRoles.Regular)]
    public class AuktionController : Controller
    {
        private readonly IAuktionService _service;
        private readonly UserManager<NauktionUser> _userManager;

        public AuktionController(IAuktionService service,
            UserManager<NauktionUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index()
        {
            List<AuktionBudViewModel> model = await _service.ListAuktionBudsAsync();

            return View(model);
        }
        
        public async Task<IActionResult> View(int id)
        {
            AuktionBudViewModel model = await _service.GetAuktionBudsAsync(id);

            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        public async Task<IActionResult> Bid(BiddingViewModel bid)
        {
            TempData["BidModel"] = bid;

            // Validate against API
            if (ModelState.IsValid)
            {
                IActionResult result = await new AuktionAPIController(_service, _userManager)
                    .VerifyBudSumma(bid.AuktionID, bid.Summa);

                if (result is JsonResult json && json.Value is string error)
                {
                    ModelState.AddModelError(nameof(bid.Summa), error);
                }
            }

            // Redirect if invalid
            if (!ModelState.IsValid)
            {
                bid.Errors = ModelState[nameof(bid.Summa)].Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return RedirectToAction("View", new {id = bid.AuktionID});
            }

            // Valid! Let's create that bid!

            TempData["BidSuccess"] = true;
            return RedirectToAction("View", new {id = bid.AuktionID});
        }
    }
}