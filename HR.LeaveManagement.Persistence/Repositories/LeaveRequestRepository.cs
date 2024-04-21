using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Persistence.Repositories;

public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(HrDatabaseContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsWithDetails()
    {
        var leaveRequests = await _context.LeaveRequests
            .Where(q => !string.IsNullOrEmpty(q.RequestingEmployeeId))
            .Include(x => x.LeaveType)
            .ToListAsync();

        return leaveRequests;
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsWithDetails(string userId)
    {
        var leaveRequests = await _context.LeaveRequests
            .Where(q => q.RequestingEmployeeId == userId)
            .Include(x => x.LeaveType)
            .ToListAsync();

        return leaveRequests;
    }

    public async Task<LeaveRequest> GetLeaveRequestWithDetails(int id)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(x => x.LeaveType)
            .FirstOrDefaultAsync(x => x.Id == id);

        return leaveRequest;
    }
}
