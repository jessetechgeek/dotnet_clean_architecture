using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using System;

namespace HR.LeaveManagement.Persistence.IntegrationTests
{
    public class HrDatabaseContextTests
    {
        private HrDatabaseContext _hrDatabaseContext;
        private readonly string _userId;
        private readonly Mock<IUserService> _userServiceMock;

        public HrDatabaseContextTests()
        {
            var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _userId = "00000000-0000-0000-0000-000000000000";

            _userServiceMock = new Mock<IUserService>();
            _userServiceMock.Setup(m => m.UserId).Returns(_userId);


            _hrDatabaseContext = new HrDatabaseContext(dbOptions, _userServiceMock.Object);
        }

        [Fact]
        public async void Save_SetDateCreated()
        {
            // Arrange
            var leaveType = new LeaveType
            {
                Id = 1,
                Name = "Test Vacation",
                DefaultDays = 10
            };

            // Act
            await _hrDatabaseContext.LeaveTypes.AddAsync(leaveType);
            await _hrDatabaseContext.SaveChangesAsync();

            Assert.NotNull(leaveType.DateCreated);
        }

        [Fact]
        public async void Save_SetDateModified()
        {
            // Arrange
            var leaveType = new LeaveType
            {
                Id = 1,
                Name = "Test Vacation",
                DefaultDays = 10
            };

            // Act
            await _hrDatabaseContext.LeaveTypes.AddAsync(leaveType);
            await _hrDatabaseContext.SaveChangesAsync();

            Assert.NotNull(leaveType.DateModified);
        }

    }
}
