using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.DTOs.Scheduling
{
    public class ShiftTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public WeekDaysDto Days { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public EmployeeRoleDto Role { get; set; }
        public int RequiredStaff { get; set; }
        public decimal Hours { get; set; }
    }

    public class SaveShiftTemplateRequest
    {
        [MaxLength(FieldLimits.Name)]
        public string Name { get; set; } = string.Empty;
        public WeekDaysDto Days { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public EmployeeRoleDto Role { get; set; }
        public int RequiredStaff { get; set; } = 1;
    }

    public class AvailabilityDto
    {
        public DayOfWeek Day { get; set; }
        public TimeOnly AvailableFrom { get; set; }
        public TimeOnly AvailableUntil { get; set; }
    }

    public class StaffMemberDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public EmployeeRoleDto Role { get; set; }
        public int ContractHours { get; set; }
        public List<AvailabilityDto> Availability { get; set; } = [];
    }

    public class SaveStaffRequest
    {
        public int ContractHours { get; set; }
        public List<AvailabilityDto> Availability { get; set; } = [];
    }

    public class ScheduledShiftDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public EmployeeRoleDto Role { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public decimal Hours { get; set; }
    }

    public class SaveShiftRequest
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public EmployeeRoleDto Role { get; set; }
        [MaxLength(FieldLimits.EmployeeId)]
        public string? EmployeeId { get; set; }
    }

    public class ColleagueDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public EmployeeRoleDto Role { get; set; }
        public List<ScheduledShiftDto> UpcomingShifts { get; set; } = [];
    }

    public class StaffHoursDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public EmployeeRoleDto Role { get; set; }
        public int ContractHours { get; set; }
        public decimal PlannedHours { get; set; }
        public int Shifts { get; set; }
    }

    public class ScheduleWeekDto
    {
        public DateOnly WeekStart { get; set; }
        public bool Exists { get; set; }
        public ScheduleWeekStatusDto Status { get; set; }
        public bool ShiftsHidden { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmedByName { get; set; }
        public int OpenShifts { get; set; }
        public List<ScheduledShiftDto> Shifts { get; set; } = [];
        public List<StaffHoursDto> Staff { get; set; } = [];
    }

    public class ScheduleWeekSummaryDto
    {
        public DateOnly WeekStart { get; set; }
        public bool Exists { get; set; }
        public ScheduleWeekStatusDto Status { get; set; }
        public int Shifts { get; set; }
        public int OpenShifts { get; set; }
    }

    public class ScheduleRequestDto
    {
        public Guid Id { get; set; }
        public int RequestNumber { get; set; }
        public ScheduleRequestTypeDto Type { get; set; }
        public ScheduleRequestStatusDto Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public ScheduledShiftDto? Shift { get; set; }
        public string? TargetEmployeeId { get; set; }
        public string? TargetEmployeeName { get; set; }
        public ScheduledShiftDto? TargetShift { get; set; }
        public DateOnly? Date { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ReviewedByName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNote { get; set; }
    }

    public class CreateScheduleRequest
    {
        public ScheduleRequestTypeDto Type { get; set; }
        public Guid? ShiftId { get; set; }
        [MaxLength(FieldLimits.EmployeeId)]
        public string? TargetEmployeeId { get; set; }
        public Guid? TargetShiftId { get; set; }
        public DateOnly? Date { get; set; }
        [MaxLength(FieldLimits.Reason)]
        public string Reason { get; set; } = string.Empty;
    }

    public class ReviewScheduleRequest
    {
        [MaxLength(FieldLimits.Reason)]
        public string? Note { get; set; }
    }
}
