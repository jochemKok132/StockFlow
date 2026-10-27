using StockFlow.Application.Mappings.Enums;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.Application.Mappings.Scheduling
{
    public static class ScheduleMappings
    {
        public static WeekDays ToDomain(this WeekDaysDto days) => (WeekDays)((int)days & 127);
        public static WeekDaysDto ToDto(this WeekDays days) => (WeekDaysDto)((int)days & 127);
        public static ScheduleWeekStatusDto ToDto(this ScheduleWeekStatus status) => (ScheduleWeekStatusDto)(int)status;
        public static ScheduleRequestTypeDto ToDto(this ScheduleRequestType type) => (ScheduleRequestTypeDto)(int)type;
        public static ScheduleRequestType ToDomain(this ScheduleRequestTypeDto type) => (ScheduleRequestType)(int)type;
        public static ScheduleRequestStatusDto ToDto(this ScheduleRequestStatus status) => (ScheduleRequestStatusDto)(int)status;
        public static ScheduleRequestStatus ToDomain(this ScheduleRequestStatusDto status) => (ScheduleRequestStatus)(int)status;

        public static ShiftTemplateDto ToDto(this ShiftTemplate template) => new()
        {
            Id = template.Id,
            Name = template.Name,
            Days = template.Days.ToDto(),
            StartTime = template.StartTime,
            EndTime = template.EndTime,
            Role = template.Role.ToDto(),
            RequiredStaff = template.RequiredStaff,
            Hours = ScheduleRules.Hours(template.StartTime, template.EndTime),
        };

        public static StaffMemberDto ToStaffMemberDto(this Employee employee) => new()
        {
            EmployeeId = employee.EmployeeId,
            FullName = employee.FullName,
            Role = employee.Role.ToDto(),
            ContractHours = employee.ContractHours,
            Availability = employee.Availability
                .OrderBy(a => ((int)a.Day + 6) % 7)
                .ThenBy(a => a.AvailableFrom)
                .Select(a => new AvailabilityDto { Day = a.Day, AvailableFrom = a.AvailableFrom, AvailableUntil = a.AvailableUntil })
                .ToList(),
        };

        public static ScheduledShiftDto ToDto(this ScheduledShift shift) => new()
        {
            Id = shift.Id,
            Date = shift.Date,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            Role = shift.Role.ToDto(),
            EmployeeId = shift.Employee?.EmployeeId,
            EmployeeName = shift.Employee?.FullName,
            Hours = ScheduleRules.Hours(shift.StartTime, shift.EndTime),
        };

        public static ScheduleRequestDto ToDto(this ScheduleRequest request) => new()
        {
            Id = request.Id,
            RequestNumber = request.RequestNumber,
            Type = request.Type.ToDto(),
            Status = request.Status.ToDto(),
            CreatedAt = request.CreatedAt,
            EmployeeId = request.Employee?.EmployeeId ?? "-",
            EmployeeName = request.Employee?.FullName ?? "-",
            Shift = request.Shift?.ToDto(),
            TargetEmployeeId = request.TargetEmployee?.EmployeeId,
            TargetEmployeeName = request.TargetEmployee?.FullName,
            TargetShift = request.TargetShift?.ToDto(),
            Date = request.Date,
            Reason = request.Reason,
            ReviewedByName = request.ReviewedBy?.FullName,
            ReviewedAt = request.ReviewedAt,
            ReviewNote = request.ReviewNote,
        };
    }
}
