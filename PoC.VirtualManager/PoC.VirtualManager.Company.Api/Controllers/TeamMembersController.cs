using Mapster;
using Microsoft.AspNetCore.Mvc;
using PoC.VirtualManager.Company.Api.Dtos;
using PoC.VirtualManager.Company.Client.Models;
using PoC.VirtualManager.Company.Infrastructure;

namespace PoC.VirtualManager.Company.Api.Controllers
{
    [ApiController]
    [Route("teams/members")]
    public class TeamMembersController : ControllerBase
    {
        private readonly ITeamMembersRepository _teamMembersRepository;

        public TeamMembersController(ITeamMembersRepository teamMembersRepository)
        {
            ArgumentNullException.ThrowIfNull(teamMembersRepository, nameof(teamMembersRepository));
            _teamMembersRepository = teamMembersRepository;
        }

        [HttpPost] //TODO import from factorial
        public async Task<IActionResult> CreateTeamMember(
            [FromBody] TeamMemberDto teamMemberDto,
            CancellationToken cancellationToken)
        {
            if (teamMemberDto == null)
            {
                return BadRequest("TeamMember cannot be null.");
            }

            var existentTeamMember = await _teamMembersRepository.GetByNameAsync(teamMemberDto.Name, cancellationToken);
            if (existentTeamMember != null)
            {
                return Conflict(new
                {
                    Error = $"{nameof(TeamDto.Name)} must be unique.",
                    Team = existentTeamMember
                });
            }

            var teamMember = teamMemberDto.Adapt<TeamMember>();

            teamMember = await _teamMembersRepository.InsertAsync(teamMember, cancellationToken);

            return CreatedAtAction(nameof(CreateTeamMember), new { id = teamMember.Id }, teamMember);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamMember(
            string id, 
            [FromBody] TeamMemberWithIdDto teamMemberDto,
            CancellationToken cancellationToken)
        {
            if (teamMemberDto == null)
            {
                return BadRequest("Invalid team data.");
            }

            var existingTeamMember = await _teamMembersRepository.GetByIdAsync(id, cancellationToken);

            if (existingTeamMember == null)
            {
                return NotFound($"Team with Id = {id} not found.");
            }

            if (existingTeamMember.TeamId != null)
            {
                return BadRequest($"{nameof(TeamMemberDto.TeamId)} can not be changed.");
            }

            teamMemberDto.Adapt(existingTeamMember);

            await _teamMembersRepository.InsertAsync(existingTeamMember, cancellationToken);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamMemberById(string id, CancellationToken cancellationToken)
        {
            var team = await _teamMembersRepository.GetByIdAsync(id, cancellationToken);

            if (team == null)
            {
                return NotFound();
            }

            var teamMemberDto = team.Adapt<TeamMemberWithIdDto>();

            return Ok(teamMemberDto);
        }
    }
}
