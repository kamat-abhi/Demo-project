using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gstin { get; set; }
        public string Pan { get; set; }
        public string BankName { get; set; }
        public string BankAccountNo { get; set; }
        public string Ifsc { get; set; }
        public string Branch { get; set; }
        public string StampUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Client
    {
        public int ClientId { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Address { get; set; }
        public string? Gstin { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Depot> Depots { get; set; } = new();
    }

    public class Depot
    {
        public int DepotId { get; set; }
        public int ClientId { get; set; }
        [Required] public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public Client Client { get; set; }
    }

    public class Service
    {
        public int ServiceId { get; set; }
        [Required] public string Description { get; set; }
        public string HsnCode { get; set; }
        public decimal DefaultRate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Term
    {
        public int TermId { get; set; }
        [Required] public string Text { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Bill
    {
        public int BillId { get; set; }
        public string? BillNo { get; set; }
        public string? BillType { get; set; } = "GST";       // GST | DEPOT
        public string? Title { get; set; } = "Invoice";
        public string? CopyType { get; set; }
        public DateTime BillDate { get; set; } = DateTime.Today;
        public string? OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? CompanyId { get; set; }
        public int? ClientId { get; set; }
        public int? ShipToClientId { get; set; }
        public decimal? CgstRate { get; set; } = 9;
        public decimal? SgstRate { get; set; } = 9;
        public decimal? SubTotal { get; set; }
        public decimal? Cgst { get; set; }
        public decimal? Sgst { get; set; }
        public decimal? GrandTotal { get; set; }
        public decimal? RoundedTotal { get; set; }
        public string? AmountInWords { get; set; }
        public string? SignatoryName { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public Company Company { get; set; }
        public Client Client { get; set; }
        [ForeignKey(nameof(ShipToClientId))] public Client ShipToClient { get; set; }
        public List<BillItemEntity> Items { get; set; } = new();
        public List<BillTerm> Terms { get; set; } = new();
    }

    // Named BillItemEntity so it does not clash with your existing view-model class BillItem
    [Table("BillItems")]
    public class BillItemEntity
    {
        [Key] public int BillItemId { get; set; }
        public int BillId { get; set; }
        public int? ServiceId { get; set; }
        public int? DepotId { get; set; }
        public string? Description { get; set; }
        public string? HsnCode { get; set; }
        public decimal Quantity { get; set; } = 1;
        public decimal Rate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Amount { get; set; }

        public Depot Depot { get; set; }
    }

    public class BillTerm
    {
        public int BillTermId { get; set; }
        public int BillId { get; set; }
        [Required] public string Text { get; set; }
    }
}
