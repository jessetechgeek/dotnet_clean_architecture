using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTypes;
using HR.LeaveManagement.Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList
{
    public class LeaveRequestListDTO
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public LeaveTypeDTO LeaveType { get; set; }

        public DateTime DateRequested { get; set; }

        public bool? Approved { get; set; }

        public string RequestingEmployeeId { get; set; }

        public int Id { get; set; }

        public Employee Employee { get; set; }

        public bool? Cancelled { get; set; }
    }
}
