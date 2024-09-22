using Mapster;
using Microsoft.AspNetCore.Mvc;
using PoC.VirtualManager.Company.Api.Dtos;
using PoC.VirtualManager.Company.Client.Models;
using PoC.VirtualManager.Company.Infrastructure;

namespace PoC.VirtualManager.Company.Api.Controllers
{
    [ApiController]
    [Route("teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsRepository _teamRepository;

        public TeamsController(ITeamsRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamDto teamDto, CancellationToken cancellationToken)
        {
            if (teamDto == null)
            {
                return BadRequest("Team cannot be null.");
            }

            var existentTeam = await _teamRepository.GetByNameAsync(teamDto.Name, cancellationToken);
            if(existentTeam != null)
            {
                return Conflict(new
                {
                    Error = $"{nameof(TeamDto.Name)} must be unique.",
                    Team = existentTeam
                });
            }

            var team = teamDto.Adapt<Team>();

            team = await _teamRepository.InsertAsync(team, cancellationToken);

            return CreatedAtAction(nameof(CreateTeam), new { team.Id }, team.Adapt<TeamWithIdDto>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(
            string id, 
            [FromBody] TeamWithIdDto teamDto,
            CancellationToken cancellationToken)
        {
            if (teamDto == null)
            {
                return BadRequest("Invalid team data.");
            }

            var existingTeam = await _teamRepository.GetByIdAsync(id, cancellationToken);

            if (existingTeam == null)
            {
                return NotFound($"Team with Id = {id} not found.");
            }

            teamDto.Adapt(existingTeam);

            await _teamRepository.UpdateAsync(existingTeam, cancellationToken);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(string id, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(id, cancellationToken);

            if (team == null)
            {
                return NotFound();
            }

            var teamDto = team.Adapt<TeamWithIdAndMembersDto>();

            return Ok(teamDto);
        }
    }
}
