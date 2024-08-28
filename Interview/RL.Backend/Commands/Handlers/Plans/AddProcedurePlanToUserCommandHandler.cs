using Castle.Core.Resource;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans
{
    public class AddProcedurePlanToUserCommandHandler : IRequestHandler<AddProcedurePlanToUserCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AddProcedurePlanToUserCommandHandler(RLContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<Unit>> Handle(AddProcedurePlanToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Validate request
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
                if (request.UserId < 0)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.PlanId == request.PlanId);
                var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);
                var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == request.UserId);

                if (plan is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
                if (procedure is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));
                if (user is null && request.UserId > 0)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));
                var exist = await _context.UsersPlanProcedures.FirstOrDefaultAsync(x => x.PlanId == request.PlanId && x.ProcedureId == request.ProcedureId && x.UserId == request.UserId);
                if (request.UserId == 0)
                {

                    var deleteuser = _context.UsersPlanProcedures
                             .Where(x => x.PlanId == request.PlanId && x.ProcedureId == request.ProcedureId)
                             .ToList();

                    foreach (var planuser in deleteuser)
                    {
                        planuser.Deleted = true;
                    }
                }
                else if (exist is null && request.UserId != 0)
                {
                    _context.UsersPlanProcedures.Add(new UsersPlanProcedures()
                    {
                        PlanId = request.PlanId,
                        ProcedureId = request.ProcedureId,
                        UserId = request.UserId,
                        Deleted = request.Deleted,
                    });
                }
                else if(exist!=null)
                {
                    exist.Deleted = request.Deleted;
                    _context.UsersPlanProcedures.Update(exist);
                }

                await _context.SaveChangesAsync();

                return ApiResponse<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
            finally
            {
                
            }
        }
    }
}
