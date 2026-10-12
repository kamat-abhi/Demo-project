using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Helpers;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    // NOTE: BillController must be declared "public partial class BillController"
    public partial class BillController
    {
        private async Task FillLookups(BillCreateViewModel m)
        {
            m.Companies = await _db.Companies.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            m.Clients = await _db.Clients.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            m.ServiceMaster = await _db.Services.Where(s => s.IsActive).OrderBy(s => s.Description).ToListAsync();
            m.TermMaster = await _db.Terms.Where(t => t.IsActive).OrderBy(t => t.TermId).ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var m = new BillCreateViewModel();
            await FillLookups(m);
            m.CompanyId = m.Companies.FirstOrDefault()?.CompanyId ?? 0;
            m.SelectedTermIds = m.TermMaster.Select(t => t.TermId).ToList();
            m.Lines.Add(new LineInput());
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillCreateViewModel m)
        {
            m.Lines = (m.Lines ?? new()).Where(l => !string.IsNullOrWhiteSpace(l.Description)).ToList();

            if (m.BillType != "GST" && m.BillType != "DEPOT")
                ModelState.AddModelError(nameof(m.BillType), "Invalid bill type.");
            if (!m.Lines.Any())
                ModelState.AddModelError("", "Add at least one line item.");
            if (m.Lines.Any(l => l.Quantity <= 0 || l.Rate < 0))
                ModelState.AddModelError("", "Quantity must be greater than 0 and rate cannot be negative.");
            if (m.BillType == "DEPOT" && m.Lines.Any(l => l.DepotId == null))
                ModelState.AddModelError("", "Select a depot on every line of a Depot bill.");
            if (!string.IsNullOrWhiteSpace(m.BillNo) && await _db.Bills.AnyAsync(b => b.BillNo == m.BillNo.Trim()))
                ModelState.AddModelError(nameof(m.BillNo), "This bill number already exists.");

            if (!ModelState.IsValid)
            {
                await FillLookups(m);
                if (!m.Lines.Any()) m.Lines.Add(new LineInput());
                return View(m);
            }

            // ---- totals (same rules as BillViewModel) ----
            decimal sub = m.Lines.Sum(l => l.Quantity * l.Rate);
            decimal cgst = Math.Round(sub * m.CgstRate / 100, 2);
            decimal sgst = Math.Round(sub * m.SgstRate / 100, 2);
            decimal grand = sub + cgst + sgst;
            decimal rounded = Math.Round(grand, 0, MidpointRounding.AwayFromZero);
            decimal payable = m.BillType == "GST" ? rounded : grand;

            var bill = new Bill
            {
                BillNo = m.BillNo.Trim(),
                BillType = m.BillType,
                Title = string.IsNullOrWhiteSpace(m.Title) ? "Invoice" : m.Title.Trim(),
                CopyType = m.BillType == "GST" ? m.CopyType : null,
                BillDate = m.BillDate,
                OrderNo = m.OrderNo,
                OrderDate = m.OrderDate,
                CompanyId = m.CompanyId,
                ClientId = m.ClientId,
                ShipToClientId = m.BillType == "GST" ? m.ShipToClientId : null,
                CgstRate = m.CgstRate,
                SgstRate = m.SgstRate,
                SubTotal = sub,
                Cgst = cgst,
                Sgst = sgst,
                GrandTotal = grand,
                RoundedTotal = rounded,
                AmountInWords = AmountInWords.Convert(payable),
                SignatoryName = m.SignatoryName
            };

            foreach (var l in m.Lines)
                bill.Items.Add(new BillItemEntity
                {
                    ServiceId = l.ServiceId,
                    DepotId = m.BillType == "DEPOT" ? l.DepotId : null,
                    Description = l.Description.Trim(),
                    HsnCode = l.HsnCode,
                    Quantity = l.Quantity,
                    Rate = l.Rate
                });

            var terms = await _db.Terms.Where(t => m.SelectedTermIds.Contains(t.TermId)).OrderBy(t => t.TermId).ToListAsync();
            foreach (var t in terms) bill.Terms.Add(new BillTerm { Text = t.Text });

            _db.Bills.Add(bill);
            await _db.SaveChangesAsync();

            TempData["Msg"] = "Bill " + bill.BillNo + " created.";
            return RedirectToAction(nameof(Details), new { id = bill.BillId });
        }

        // ---- small JSON endpoints used by the form ----

        // suggests the next number for the financial year (Apr-Mar), e.g. MT/26-27/2
        [HttpGet]
        public async Task<IActionResult> NextNo(DateTime date)
        {
            int y = date.Month >= 4 ? date.Year : date.Year - 1;
            var start = new DateTime(y, 4, 1);
            var end = start.AddYears(1);
            string fy = $"{y % 100:00}-{(y + 1) % 100:00}";
            int n = await _db.Bills.CountAsync(b => b.BillDate >= start && b.BillDate < end) + 1;
            string no;
            do { no = $"MT/{fy}/{n++}"; } while (await _db.Bills.AnyAsync(b => b.BillNo == no));
            return Json(new { billNo = no });
        }

        [HttpGet]
        public async Task<IActionResult> ClientDepots(int clientId)
        {
            var list = await _db.Depots.Where(d => d.ClientId == clientId && d.IsActive)
                                       .OrderBy(d => d.Name)
                                       .Select(d => new { id = d.DepotId, name = d.Name })
                                       .ToListAsync();
            return Json(list);
        }
    }
}
