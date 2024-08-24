namespace PoC.VirtualManager.Teams.Client.Models
{
    public class Team
    {
        public string TeamName { get; set; }
        public List<TeamMember> Members { get; set; } = new List<TeamMember>();
        public string ProjectName { get; set; }
        public string Department { get; set; }
        public string DomainExpertise { get; set; }
        public string TechnicalExpertise { get; set; }
    }
}
