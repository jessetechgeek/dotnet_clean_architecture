using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public class MockLeaveTypeRepository
    {
        public static Mock<ILeaveTypeRepository> GetLeaveTypeMockRepository()
        {
            var leaveTypes = new List<LeaveType>
            {
                new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 10 },
                new LeaveType { Id = 2, Name = "Sick Leave", DefaultDays = 10 },
                new LeaveType { Id = 3, Name = "Maternity Leave", DefaultDays = 10 }
            };

            var mockRepo = new Mock<ILeaveTypeRepository>();
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(leaveTypes);

            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<LeaveType>()))
                .Returns((LeaveType leaveType) =>
                {
                    leaveTypes.Add(leaveType);
                    return Task.CompletedTask;
                });

            return mockRepo;
        }
    }
}
