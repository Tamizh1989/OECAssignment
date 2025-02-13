using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands
{
    public class UpdateUsersInPlanProcedureCommand : IRequest<ApiResponse<Unit>>
    {
        public int PlanId { get; set; }
        public int ProcedureId { get; set; }
        public List<int> UsersToAdd { get; set; } = new();
        public List<int> UsersToRemove { get; set; } = new();
    }
}
