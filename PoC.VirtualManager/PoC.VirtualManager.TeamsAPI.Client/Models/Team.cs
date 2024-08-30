namespace PoC.VirtualManager.Teams.Client.Models
{
    public class Team
    {
        public int CompanyId { get; set; }
        public string TeamName { get; set; }
        public List<int> TeamMembersIds { get; set; } = new List<int>();
        public string ProjectName { get; set; }
        public string Department { get; set; }
        public string DomainExpertise { get; set; }
        public string TechnicalExpertise { get; set; }
    }
}
