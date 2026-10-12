using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class LineInput
    {
        public int? ServiceId { get; set; }
        public int? DepotId { get; set; }
        public string Description { get; set; }
        public string HsnCode { get; set; }
        public decimal Quantity { get; set; } = 1;
        public decimal Rate { get; set; }
    }

    public class BillCreateViewModel
    {
        public string BillType { get; set; } = "GST";          // GST | DEPOT
        public string Title { get; set; }
        public string CopyType { get; set; }
        [Required(ErrorMessage = "Bill number is required")]
        public string BillNo { get; set; }
        [Required] public DateTime BillDate { get; set; } = DateTime.Today;
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a company")]
        public int CompanyId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Select a client")]
        public int ClientId { get; set; }
        public int? ShipToClientId { get; set; }

        public decimal CgstRate { get; set; } = 9;
        public decimal SgstRate { get; set; } = 9;
        public string SignatoryName { get; set; } = "Authorised Signatory";

        public List<LineInput> Lines { get; set; } = new List<LineInput>();
        public List<int> SelectedTermIds { get; set; } = new List<int>();

        // dropdown data (filled by controller, not posted)
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<Service> ServiceMaster { get; set; } = new List<Service>();
        public List<Term> TermMaster { get; set; } = new List<Term>();
    }
}
