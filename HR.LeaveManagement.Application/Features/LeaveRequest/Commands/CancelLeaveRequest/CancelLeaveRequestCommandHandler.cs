using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Models.Email;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CancelLeaveRequest
{
    public class CancelLeaveRequestCommandHandler : IRequestHandler<CancelLeaveRequestCommand, Unit>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAppLogger<CancelLeaveRequestCommandHandler> _appLogger;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;

        public CancelLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, IEmailSender emailSender, IAppLogger<CancelLeaveRequestCommandHandler> appLogger, ILeaveAllocationRepository leaveAllocationRepository)
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._emailSender = emailSender;
            this._appLogger = appLogger;
            this._leaveAllocationRepository = leaveAllocationRepository;
        }

        public async Task<Unit> Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);

            if (leaveRequest == null)
            {
                throw new NotFoundException(nameof(leaveRequest), request.Id);
            }

            leaveRequest.Cancelled = true;

            if (leaveRequest.Approved == true)
            {
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                var allocation = await _leaveAllocationRepository.GetUserAllocations(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                allocation.NumberOfDays += daysRequested;

                await _leaveAllocationRepository.UpdateAsync(allocation);
            }

            var email = new EmailMessage
            {
                To = "manager@localhost",
                Subject = $"Leave Request Cancelled: {leaveRequest.RequestingEmployeeId}",
                Body = $"Leave Request for employee {leaveRequest.RequestingEmployeeId} has been cancelled."
            };

            try
            {
                await _emailSender.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.Message);
            }

            return Unit.Value;
        }
    }
}
