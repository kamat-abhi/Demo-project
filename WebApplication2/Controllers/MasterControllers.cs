using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class CompaniesController : MasterController<Company>
    {
        public CompaniesController(BillingDbContext db) : base(db) { }
        protected override string Heading => "Company";
    }

    public class ClientsController : MasterController<Client>
    {
        public ClientsController(BillingDbContext db) : base(db) { }
        protected override string Heading => "Clients";
        protected override IQueryable<Client> Query() => Db.Clients.OrderBy(c => c.Name);
    }

    public class ServicesController : MasterController<Service>
    {
        public ServicesController(BillingDbContext db) : base(db) { }
        protected override string Heading => "Services / Items";
        protected override IQueryable<Service> Query() => Db.Services.OrderBy(s => s.Description);
    }

    public class TermsController : MasterController<Term>
    {
        public TermsController(BillingDbContext db) : base(db) { }
        protected override string Heading => "Terms & Conditions";
    }

    public class DepotsController : MasterController<Depot>
    {
        public DepotsController(BillingDbContext db) : base(db) { }
        protected override string Heading => "Depots";
        protected override IQueryable<Depot> Query() =>
            Db.Depots.Include(d => d.Client).OrderBy(d => d.Client.Name).ThenBy(d => d.Name);
        protected override async Task LoadLookups() =>
            ViewBag.Clients = new SelectList(await Db.Clients.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                                             "ClientId", "Name");
    }
}
