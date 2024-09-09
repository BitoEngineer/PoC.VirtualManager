namespace PoC.VirtualManager.Teams.Api.Dtos
{
    public class TeamDto
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> TeamMembersIds { get; set; } = new List<int>();
        public string Description { get; set; }
        public string Department { get; set; }
        public string DomainExpertise { get; set; }
        public string TechnicalExpertise { get; set; }
    }

}
