using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public partial class BillController : Controller
    {
        private readonly BillingDbContext _db;
        public BillController(BillingDbContext db) { _db = db; }

        // /Bill  -> list of all bills from database
        public async Task<IActionResult> Index()
        {

            var bills = await _db.Bills
                .Include(b => b.Client)
                .Include(b => b.Items)
                .OrderByDescending(b => b.BillDate).ThenByDescending(b => b.BillId)
                .ToListAsync();

            var items = bills.Select(b => new BillListItem
            {

                Id = b.BillId,
                Kind = b.BillType == "GST" ? "GST Tax Invoice" : "Monthly Bill",
                Title = b.Title,
                BillNo = b.BillNo,
                Date = b.BillDate,
                Client = b.Client.Name + ", " + b.Client.Address,
                Total = b.BillType == "GST" ? (b.RoundedTotal ?? 0m) : (b.GrandTotal ?? 0m),
                Lines = b.Items.Count,
                Accent = b.BillType == "GST" ? "blue" : "green"
            }).ToList();

            return View(items);
        }

        // /Bill/Details/5 -> picks the right layout by BillType
        public async Task<IActionResult> Details(int id)
        {
            var bill = await _db.Bills
                .Include(b => b.Company)
                .Include(b => b.Client)
                .Include(b => b.ShipToClient)
                .Include(b => b.Items).ThenInclude(i => i.Depot)
                .Include(b => b.Terms)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null) return NotFound();

            var vm = MapToViewModel(bill);
            return View(bill.BillType == "GST" ? "SslInvoice" : "HardoiBill", vm);
        }

        // Entity -> the same BillViewModel your views already use
        private static BillViewModel MapToViewModel(Bill b)
        {
            var vm = new BillViewModel
            {
                CompanyName = b.Company.Name,
                CompanyAddress = b.Company.Address,
                CompanyPhone = b.Company.Phone,
                CompanyEmail = b.Company.Email,
                Gstin = b.Company.Gstin,
                Pan = b.Company.Pan,
                BankName = b.Company.BankName,
                BankAccountNo = b.Company.BankAccountNo,
                Ifsc = b.Company.Ifsc,
                Branch = b.Company.Branch,
                StampUrl = b.Company.StampUrl,

                Title = b.Title,
                CopyType = b.CopyType,
                BillNo = b.BillNo,
                BillDate = b.BillDate,
                OrderNo = b.OrderNo,
                OrderDate = b.OrderDate,

                BilledToName = b.Client.Name,
                BilledToAddress = b.Client.Address,
                ShippedToName = b.ShipToClient?.Name,
                ShippedToAddress = b.ShipToClient?.Address,

                CgstRate = b.CgstRate ?? 0m,
                SgstRate = b.SgstRate ?? 0m,
                AmountInWords = b.AmountInWords,
                SignatoryName = b.SignatoryName
            };

            foreach (var i in b.Items)
                vm.Items.Add(new BillItem
                {
                    Description = i.Description,
                    HsnCode = i.HsnCode,
                    Depot = i.Depot?.Name,
                    Quantity = i.Quantity,
                    Rate = i.Rate
                });

            foreach (var t in b.Terms) vm.Terms.Add(t.Text);
            return vm;
        }
    }
}
