namespace PoC.VirtualManager.Company.Api.Dtos
{
    public class TeamWithIdDto : TeamDto
    {
        public string Id { get; set; }
    }

    public class TeamWithIdAndMembersDto : TeamWithIdDto
    {
        public List<int> TeamMembersIds { get; set; } = new List<int>();
    }
}
