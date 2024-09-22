using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Company.Client.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string LegalName { get; set; }
        public string Industry { get; set; }
        public string Description { get; set; }
        public Contact ContactInfo { get; set; }
        public Address HeadquartersAddress { get; set; }
        public Financial FinancialInfo { get; set; }
        public KeyPersonnel Personnel { get; set; }
        public SocialMedia SocialMediaLinks { get; set; }
        public List<string> Subsidiaries { get; set; }
        public List<string> Competitors { get; set; }
        public List<string> Products { get; set; }
        public string MissionStatement { get; set; }
        public string VisionStatement { get; set; }
        public List<string> CoreValues { get; set; }
        public OrganizationalChart OrgChart { get; set; }
    }

    public class Contact
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string FaxNumber { get; set; }
    }

    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    public class Financial
    {
        public decimal Revenue { get; set; }
        public int NumberOfEmployees { get; set; }
        public DateTime FoundedDate { get; set; }
        public string StockSymbol { get; set; }
        public string StockExchange { get; set; }
    }

    public class KeyPersonnel
    {
        public string CEO { get; set; }
        public string CFO { get; set; }
        public string COO { get; set; }
        public string CTO { get; set; }
        public string CMO { get; set; }
        public string CHRO { get; set; }
        public string GeneralCounsel { get; set; }
        public string CIO { get; set; }
        public string VPOfSales { get; set; }
        public string VPOfMarketing { get; set; }
    }

    public class SocialMedia
    {
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
    }

    public class OrganizationalChart
    {
        public string CEO { get; set; }
        public List<string> SeniorVicePresidents { get; set; }
        public List<string> VicePresidents { get; set; }
        public List<string> Directors { get; set; }
        public List<string> Managers { get; set; }
        public List<string> Staff { get; set; }
    }
}
