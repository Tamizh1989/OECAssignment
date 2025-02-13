using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

public class UpdateProcedureInPlanCommandHandler : IRequestHandler<UpdateProcedureInPlanCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public UpdateProcedureInPlanCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(UpdateProcedureInPlanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            var plan = await _context.Plans
                .Include(p => p.PlanProcedures)
                .FirstOrDefaultAsync(p => p.PlanId == request.PlanId, cancellationToken);

            if (plan == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
            
            var procedure = await _context.Procedures
                .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId, cancellationToken);

            if (procedure == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

            var planProcedure = plan.PlanProcedures.FirstOrDefault(pp => pp.ProcedureId == request.ProcedureId);

            if (request.IsAdding)
            {
                if (planProcedure == null)
                {
                    plan.PlanProcedures.Add(new PlanProcedure { ProcedureId = request.ProcedureId });
                }
            }
            else
            {
                if (planProcedure != null)
                {
                    plan.PlanProcedures.Remove(planProcedure);

                    // Remove assigned users for this procedure
                    var assignmentsToRemove = await _context.AssignedUsers
                        .Where(a => a.PlanId == request.PlanId && a.ProcedureId == request.ProcedureId)
                        .ToListAsync(cancellationToken);

                    if (assignmentsToRemove.Any())
                        _context.AssignedUsers.RemoveRange(assignmentsToRemove);
                }
                else
                {
                    return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found in the Plan"));
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}