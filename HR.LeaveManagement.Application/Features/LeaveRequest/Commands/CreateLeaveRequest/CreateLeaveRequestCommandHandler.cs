using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Models.Email;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest
{
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAppLogger<CreateLeaveRequestCommandHandler> _appLogger;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly IUserService _userService;

        public CreateLeaveRequestCommandHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository, ILeaveRequestRepository leaveRequestRepository, IEmailSender emailSender, IAppLogger<CreateLeaveRequestCommandHandler> appLogger, ILeaveAllocationRepository leaveAllocationRepository, IUserService userService)
        {
            this._mapper = mapper;
            this._leaveTypeRepository = leaveTypeRepository;
            this._leaveRequestRepository = leaveRequestRepository;
            this._emailSender = emailSender;
            this._appLogger = appLogger;
            this._leaveAllocationRepository = leaveAllocationRepository;
            this._userService = userService;
        }

        public async Task<Unit> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateLeaveRequestValidator(_leaveTypeRepository);

            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid Leave Request", validationResult);
            }

            var employeeId = _userService.UserId;

            var allocation = await _leaveAllocationRepository.GetUserAllocations(employeeId, request.LeaveTypeId);

            if (allocation is null)
            {
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.LeaveTypeId),
                    "You do not have any allocations for this leave type."));
                throw new BadRequestException("Invalid Leave Request", validationResult);
            }

            int daysRequested = (int)(request.EndDate - request.StartDate).TotalDays;
            if (daysRequested > allocation.NumberOfDays)
            {
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                    nameof(request.EndDate), "You do not have enough days for this request"));
                throw new BadRequestException("Invalid Leave Request", validationResult);
            }

            var leaveRequest = _mapper.Map<Domain.LeaveRequest>(request);
            leaveRequest.RequestingEmployeeId = employeeId;
            leaveRequest.DateRequested = DateTime.Now;
            await _leaveRequestRepository.AddAsync(leaveRequest);

            var email = new EmailMessage
            {
                To = "admin@localhost",
                Subject = "Leave Request",
                Body = $"A new leave request was submitted."
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
