using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BillController : Controller
    {
        public IActionResult Index()
        {
            var items = new List<BillListItem>();
            var ssl = BuildSsl();
            items.Add(new BillListItem
            {
                Action = "SslInvoice",
                Kind = "GST Tax Invoice",
                Title = "SSL Certificate",
                BillNo = ssl.BillNo,
                Date = ssl.BillDate,
                Client = ssl.BilledToName + ", " + ssl.BilledToAddress,
                Total = ssl.RoundedTotal,
                Lines = ssl.Items.Count,
                Accent = "blue"
            });
            var h = BuildHardoi();
            items.Add(new BillListItem
            {
                Action = "HardoiBill",
                Kind = "Monthly Bill",
                Title = h.Title,
                BillNo = h.BillNo,
                Date = h.BillDate,
                Client = h.BilledToName + ", " + h.BilledToAddress,
                Total = h.GrandTotal,
                Lines = h.Items.Count,
                Accent = "green"
            });
            return View(items);
        }

        public IActionResult SslInvoice() { return View(BuildSsl()); }
        public IActionResult HardoiBill() { return View(BuildHardoi()); }

        // /Bill/SslInvoice
        private BillViewModel BuildSsl()
        {
            var model = new BillViewModel
            {
                CompanyAddress = "5/552, Sector-5, Gomati Nagar Extension, Lucknow",
                CompanyEmail = "info@mectoi.in",
                Gstin = "09ABBFM2085K1Z1",
                CopyType = "Original Copy",
                BillNo = "MT/26-27/PB1",
                BillDate = new DateTime(2026, 10, 3),
                OrderNo = "524/ ूरो/िव० ०/2026/ई-मेल",
                OrderDate = new DateTime(2026, 9, 22),
                BilledToName = "Director General",
                BilledToAddress = "Bureau of Public Enterprises, Uttar Pradesh, Lucknow",
                ShippedToName = "Director General",
                ShippedToAddress = "Bureau of Public Enterprises, Uttar Pradesh, Lucknow",
                AmountInWords = "Rupees Eight Thousand Nine Hundred Sixty Eight Only.",
                BankAccountNo = "083305500634",
                BankName = "ICICI",
                Ifsc = "ICIC0000833",
                Branch = "Naka Hindola, Lucknow",
                Pan = "ABBFM2085K",
                SignatoryName = "Authorised Signatory",
                StampUrl = "~/img/1.png"
            };
            model.Items.Add(new BillItem
            {
                Description = "SSL for the domain \"https://sarvjanikudyam.up.nic.in\"",
                HsnCode = "998319",
                Quantity = 1,
                Rate = 7600
            });
            model.Terms.Add("E. & O.E.");
            model.Terms.Add("Goods once sold will not be taken back.");
            model.Terms.Add("Subject to 'Uttar Pradesh' jurisdiction only.");
            return model;
        }

        // /Bill/HardoiBill  -> depot-wise bill (green design)
        private BillViewModel BuildHardoi()
        {
            var model = new BillViewModel
            {
                Title = "Bill (April - 2025)",
                CompanyAddress = "Plot No 1355, Ram Vihar Colony, Osho Nagar, P.O: Manak Nagar, Pincode-226011, Lucknow (U.P.)",
                CompanyPhone = "+91-9455520949, 9369350321",
                CompanyEmail = "info@mectoi.in",
                BillNo = "2526/HDI/2",
                BillDate = new DateTime(2025, 5, 3),
                OrderNo = "राके0मु0अ0/15-01के0मु0अ0/15",
                OrderDate = new DateTime(2015, 2, 2),
                BilledToName = "The Regional Manager",
                BilledToAddress = "Hardoi Region U.P.S.R.T.C",
                AmountInWords = "Four thousand seven hundred twenty only",
                SignatoryName = "Manish (Authorized Signatory)"
            };
            string[] depots = { "HARDOI DEPOT", "KANNAUJ DEPOT", "LAKHIMPUR DEPOT", "RM OFFICE HARDOI",
                                "SHAHJAHANPUR DEPOT", "SITAPUR DEPOT", "SM OFFICE HARDOI", "GOLA DEPOT" };
            foreach (var d in depots)
                model.Items.Add(new BillItem { Description = "On line Payroll Management System Software", Depot = d, Quantity = 1, Rate = 500 });
            model.Terms.Add("100% payment has to be made by RTGS/NEFT to representative of Mectoi Technologies, Lucknow.");
            return model;
        }
    }
}
