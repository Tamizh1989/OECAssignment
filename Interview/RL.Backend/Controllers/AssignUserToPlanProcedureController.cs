using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Backend.Commands;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignUserToPlanProcedureController : ControllerBase
    {
        private readonly ILogger<AssignUserToPlanProcedureController> _logger;
        private readonly RLContext _context;
        private readonly IMediator _mediator;
        public AssignUserToPlanProcedureController(ILogger<AssignUserToPlanProcedureController> logger, RLContext context, IMediator mediator)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [EnableQuery]
        public IEnumerable<AssignedUser> Get()
        {
            return _context.AssignedUsers;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserToPlanProcedure(UpdateUsersInPlanProcedureCommand command, CancellationToken token)
        {
            var response = await _mediator.Send(command, token);

            return response.ToActionResult();
        }

    }
    
}
