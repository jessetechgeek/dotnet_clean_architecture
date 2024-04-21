using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation
{
    public class CreateLeaveAllocationCommandHandler : IRequestHandler<CreateLeaveAllocationCommand, Unit>
    {
        private readonly IMapper mapper;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IUserService _userService;

        public CreateLeaveAllocationCommandHandler(IMapper mapper, ILeaveAllocationRepository leaveAllocationRepository, ILeaveTypeRepository leaveTypeRepository, IUserService userService)
        {
            this.mapper = mapper;
            this._leaveAllocationRepository = leaveAllocationRepository;
            this._leaveTypeRepository = leaveTypeRepository;
            this._userService = userService;
        }

        public async Task<Unit> Handle(CreateLeaveAllocationCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateLeaveAllocationCommandValidator(_leaveTypeRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid Leave allocation Request", validationResult);
            }

            var leaveType = await _leaveTypeRepository.GetByIdAsync(request.LeaveTypeId);

            var employees = await _userService.GetEmployees();

            var period = DateTime.Now.Year;

            var allocations = new List<Domain.LeaveAllocation>();

            foreach (var employee in employees)
            {
                var allocationExists = await _leaveAllocationRepository.AllocationExists(employee.Id, request.LeaveTypeId, period);
                if (allocationExists == false)
                {
                    allocations.Add(new Domain.LeaveAllocation()
                    {
                        EmployeeId = employee.Id,
                        LeaveTypeId = request.LeaveTypeId,
                        NumberOfDays = leaveType.DefaultDays,
                        Period = period
                    });
                }
            }
            if (allocations.Any())
                await _leaveAllocationRepository.AddAllocations(allocations);

            return Unit.Value;

        }
    }
}
