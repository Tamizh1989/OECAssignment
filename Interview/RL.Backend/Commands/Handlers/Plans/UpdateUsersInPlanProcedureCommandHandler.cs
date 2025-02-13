using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans
{
    public class UpdateUsersInPlanProcedureCommandHandler : IRequestHandler<UpdateUsersInPlanProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public UpdateUsersInPlanProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<Unit>> Handle(UpdateUsersInPlanProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate request
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
     
                var planProcedureExists = await _context.PlanProcedures
                    .AnyAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);
                
                if (!planProcedureExists)
                    return ApiResponse<Unit>.Fail(new NotFoundException("PlanProcedure combination not found"));
               
                foreach (var userId in request.UsersToAdd)
                {
                    bool exists = await _context.AssignedUsers.AnyAsync(a =>
                        a.PlanId == request.PlanId &&
                        a.ProcedureId == request.ProcedureId &&
                        a.UserId == userId, cancellationToken);

                    if (!exists)
                    {
                        var assignedUser = new AssignedUser
                        {
                            PlanId = request.PlanId,
                            ProcedureId = request.ProcedureId,
                            UserId = userId,
                            AssignedDate = DateTime.UtcNow,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow
                        };
                        _context.AssignedUsers.Add(assignedUser);
                    }
                }

                // Remove users
                var usersToRemove = await _context.AssignedUsers
                    .Where(a => a.PlanId == request.PlanId && a.ProcedureId == request.ProcedureId && request.UsersToRemove.Contains(a.UserId))
                    .ToListAsync(cancellationToken);

                _context.AssignedUsers.RemoveRange(usersToRemove);

                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }

        } 
    }
    
}
