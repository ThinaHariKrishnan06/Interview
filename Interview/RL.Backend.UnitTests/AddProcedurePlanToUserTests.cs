using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;

namespace RL.Backend.UnitTests
{
    [TestClass]
    public class AddProcedurePlanToUserTests
    {
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AddProcedurePlanToUserTests_InvalidPlanId_ReturnsBadRequest(int planId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddProcedurePlanToUserCommandHandler(context.Object);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = planId,
                ProcedureId = 1,
                UserId = 1,
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AddProcedurePlanToUserTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddProcedurePlanToUserCommandHandler(context.Object);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = 1,
                ProcedureId = procedureId,
                UserId = 1,
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task AddProcedurePlanToUserTests_InvalidUserId_ReturnsBadRequest(int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddProcedurePlanToUserCommandHandler(context);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = 1,
                ProcedureId = 1,
                UserId = userId,
            };
            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = 1
            });
            await context.SaveChangesAsync();
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AddProcedureToPlanTests_PlanIdNotFound_ReturnsNotFound(int planId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddProcedurePlanToUserCommandHandler(context);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = planId,
                ProcedureId = 1,
                UserId = 1
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId + 1
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AddProcedureToPlanTests_ProcedureIdNotFound_ReturnsNotFound(int procedureId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddProcedurePlanToUserCommandHandler(context);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = 1,
                ProcedureId = procedureId,
                UserId = 1
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = procedureId + 1
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = procedureId + 1,
                ProcedureTitle = "Test Procedure"
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AddProcedureToPlanTests_userIdNotFound_ReturnsNotFound(int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddProcedurePlanToUserCommandHandler(context);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = 1,
                ProcedureId = 1,
                UserId = userId
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = 1
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = 1,
                ProcedureTitle = "Test Procedure"
            });
            context.Users.Add(new Data.DataModels.User
            {
                UserId = userId + 1,
                Name = "Test User"
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 879)]
        [DataRow(35, 69, 78)]
        public async Task AddProcedureToPlanTests_AlreadyContainsProcedure_ReturnsSuccess(int planId, int procedureId, int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddProcedurePlanToUserCommandHandler(context);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId= userId

            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = procedureId,
                ProcedureTitle = "Test Procedure"
            });
            context.Users.Add(new Data.DataModels.User
            {
                UserId = userId,
                Name = "Test User"
            });
            context.UsersPlanProcedures.Add(new Data.DataModels.UsersPlanProcedures
            {
                ProcedureId = procedureId,
                PlanId = planId,
                UserId = userId,
                Deleted = false

            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 879)]
        [DataRow(35, 69, 78)]
        public async Task AddProcedureToPlanTests_DoesntContainsProcedure_ReturnsSuccess(int planId, int procedureId, int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddProcedurePlanToUserCommandHandler(context);
            var request = new AddProcedurePlanToUserCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId= userId
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = procedureId,
                ProcedureTitle = "Test Procedure"
            });
            context.Users.Add(new Data.DataModels.User
            {
                UserId = userId,
                Name = "Test"+ userId
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            var dbUsertoPlanProcedure = await context.UsersPlanProcedures.FirstOrDefaultAsync(pp => pp.PlanId == planId && pp.ProcedureId == procedureId && pp.UserId== userId);

            dbUsertoPlanProcedure.Should().NotBeNull();

            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }
    }
}
