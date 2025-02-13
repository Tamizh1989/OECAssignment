using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;

namespace RL.Backend.UnitTests
{
    [TestClass]
    public class AssignUserToPlanTests
    {
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AssignUserToPlan_InvalidPlanId_ReturnsBadRequest(int planId)
        {
            // Arrange
            var context = new Mock<RLContext>();
            var sut = new UpdateUsersInPlanProcedureCommandHandler(context.Object);
            var request = new UpdateUsersInPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = 1,
            };

            // Act
            var result = await sut.Handle(request, new CancellationToken());

            // Assert
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();

        }
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AssignUserToPlanProcedure_InvalidProcedureId_ReturnsBadRequest(int procedureId)
        {
            // Arrange
            var context = new Mock<RLContext>();
            var sut = new UpdateUsersInPlanProcedureCommandHandler(context.Object);
            var request = new UpdateUsersInPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = procedureId,
            };

           //Act
            var result = await sut.Handle(request, new CancellationToken());

            // result
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AssignUserToPlanProcedure_InvalidUserId_ReturnsBadRequest(int userId)
        {
            // Arrange
            var context = new Mock<RLContext>();
            var sut = new UpdateUsersInPlanProcedureCommandHandler(context.Object);
            var request = new UpdateUsersInPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = 1
            };

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            // result
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AssignUserToPlanProcedure_PlanNotFound_ReturnsNotFound(int planId)
        {
            // Arrange
            var context = DbContextHelper.CreateContext();
            var sut = new UpdateUsersInPlanProcedureCommandHandler(context);
            var request = new UpdateUsersInPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = 1,

            };

            context.Plans.Add(new Data.DataModels.Plan { PlanId = planId + 1 });
            await context.SaveChangesAsync();

            // Act
            var result = await sut.Handle(request, new CancellationToken());

            // Assert
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AssignUserToPlanProcedure_ProcedureNotFound_ReturnsNotFound(int procedureId)
        {
            // Arrange
            var context = DbContextHelper.CreateContext();
            var sut = new UpdateUsersInPlanProcedureCommandHandler(context);
            var request = new UpdateUsersInPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = procedureId,

            };

            context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
            await context.SaveChangesAsync();

            // Act
            var result = await sut.Handle(request, new CancellationToken());

            // Assert
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AssignUserToPlanProcedure_UserNotFound_ReturnsNotFound(int userId)
        {
            // Arrange
            var context = DbContextHelper.CreateContext();
            var sut = new UpdateUsersInPlanProcedureCommandHandler(context);
            var request = new UpdateUsersInPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = 1

            };

            context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
            context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "Test Procedure" });
            await context.SaveChangesAsync();

            // Act
            var result = await sut.Handle(request, new CancellationToken());

            // Assert
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 5)]
        [DataRow(35, 69, 9)]
        public async Task AssignUserToPlanProcedure_AlreadyAssigned_ReturnsSuccess(int planId, int procedureId, int userId)
        {
            // Arrange
            var context = DbContextHelper.CreateContext();
            var sut = new UpdateUsersInPlanProcedureCommandHandler(context);
            var request = new UpdateUsersInPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId

            };

            context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
            context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test Procedure" });
            context.Users.Add(new Data.DataModels.User { UserId = userId, Name = "Test User" });
            context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { ProcedureId = procedureId, PlanId = planId });
            await context.SaveChangesAsync();

            // Act
            var result = await sut.Handle(request, new CancellationToken());

            // Assert
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }
        
    }

}

