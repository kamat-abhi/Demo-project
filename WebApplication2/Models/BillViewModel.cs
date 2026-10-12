using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Models
{
    public class BillViewModel
    {
        // Company (seller)
        public string CompanyName { get; set; } = "Mectoi Technologies";
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string Gstin { get; set; }

        // Bill header
        public string Title { get; set; } = "Invoice";
        public string CopyType { get; set; }    
        public string BillNo { get; set; }
        public DateTime BillDate { get; set; }
        public string OrderNo { get; set; }     
        public DateTime? OrderDate { get; set; }

        // Parties
        public string BilledToName { get; set; }
        public string BilledToAddress { get; set; }
        public string ShippedToName { get; set; }   
        public string ShippedToAddress { get; set; }

        // Lines
        public List<BillItem> Items { get; set; } = new List<BillItem>();
        public decimal CgstRate { get; set; } = 9;
        public decimal SgstRate { get; set; } = 9;

        // Footer
        public string AmountInWords { get; set; }
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string Ifsc { get; set; }
        public string Branch { get; set; }
        public string Pan { get; set; }
        public List<string> Terms { get; set; } = new List<string>();
        public string SignatoryName { get; set; }
        public string StampUrl { get; set; }     

        // Calculated
        public decimal SubTotal => Items.Sum(i => i.Amount);
        public decimal Cgst => Math.Round(SubTotal * CgstRate / 100, 2);
        public decimal Sgst => Math.Round(SubTotal * SgstRate / 100, 2);
        public decimal GrandTotal => SubTotal + Cgst + Sgst;
        public decimal RoundedTotal => Math.Round(GrandTotal, 0, MidpointRounding.AwayFromZero);
    }

    public class BillItem
    {
        public string Description { get; set; }
        public string Depot { get; set; }     
        public string HsnCode { get; set; }  
        public decimal Quantity { get; set; } = 1;
        public decimal Rate { get; set; }
        public decimal Amount => Quantity * Rate;
    }

    public class BillListItem
    {
        public string Action { get; set; }
        public string Kind { get; set; }
        public string Title { get; set; }
        public string BillNo { get; set; }
        public DateTime Date { get; set; }
        public string Client { get; set; }
        public decimal Total { get; set; }
        public int Lines { get; set; }
        public string Accent { get; set; }
    }
}
