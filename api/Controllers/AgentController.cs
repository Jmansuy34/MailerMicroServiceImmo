using Mailer.DTOs;
using Mailer.Service;
using Microsoft.AspNetCore.Mvc;

namespace Mailer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly AgentService _agentService;

        public AgentController(AgentService agentService)
        {
            _agentService = agentService;
        }

        // Reçoit la requête POST avec les informations de l'agent et le token dans le body
        [HttpPost("registerAgent")]
        public async Task<IActionResult> RegisterAgent([FromBody] AdminAgentDto agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Appeler le service pour enregistrer l'agent et envoyer un email
                await _agentService.RegisterAgent(agent, agent.AuthToken); 
                return Ok(new { Message = "L'email d'inscription a été envoyé à l'agent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur est survenue.", Error = ex.Message });
            }
        }
    }
}
