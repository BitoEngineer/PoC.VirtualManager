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
        private readonly ITeamsRepository _teamRepository;

        public TeamMembersController(ITeamsRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [HttpPost] //TODO import from factorial
        public async Task<IActionResult> CreateTeamMember([FromBody] TeamMemberDto teamMemberDto)
        {
            if (teamMemberDto == null)
            {
                return BadRequest("TeamMember cannot be null.");
            }

            var existentTeamMember = await _teamRepository.GetTeamMemberByNameAsync(teamMemberDto.Name);
            if (existentTeamMember != null)
            {
                return Conflict(new
                {
                    Error = $"{nameof(TeamDto.Name)} must be unique.",
                    Team = existentTeamMember
                });
            }

            var teamMember = teamMemberDto.Adapt<TeamMember>();

            teamMember = await _teamRepository.UpsertTeamMemberAsync(teamMember);

            return CreatedAtAction(nameof(CreateTeamMember), new { id = teamMember.Id }, teamMember);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamMember(string id, [FromBody] TeamMemberWithIdDto teamMemberDto)
        {
            if (teamMemberDto == null)
            {
                return BadRequest("Invalid team data.");
            }

            var existingTeamMember = await _teamRepository.GetTeamMemberByIdAsync(id);

            if (existingTeamMember == null)
            {
                return NotFound($"Team with Id = {id} not found.");
            }

            if (existingTeamMember.TeamId != null)
            {
                return BadRequest($"{nameof(TeamMemberDto.TeamId)} can not be changed.");
            }

            teamMemberDto.Adapt(existingTeamMember);

            await _teamRepository.UpsertTeamMemberAsync(existingTeamMember);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamMemberById(string id)
        {
            var team = await _teamRepository.GetTeamMemberByIdAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            var teamMemberDto = team.Adapt<TeamMemberWithIdDto>();

            return Ok(teamMemberDto);
        }
    }
}
