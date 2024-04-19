using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;

namespace HR.LeaveManagement.Persistence.IntegrationTests
{
    public class HrDatabaseContextTests
    {
        private HrDatabaseContext _hrDatabaseContext;

        public HrDatabaseContextTests()
        {
            var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _hrDatabaseContext = new HrDatabaseContext(dbOptions);
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
