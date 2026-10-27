using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.Application.Validations.Scheduling
{
    public static class ScheduleValidation
    {
        private static readonly EmployeeRoleDto[] ScheduledRoles =
            [EmployeeRoleDto.Manager, EmployeeRoleDto.ShiftLeader, EmployeeRoleDto.ServiceDesk, EmployeeRoleDto.Cashier, EmployeeRoleDto.FloorWorker, EmployeeRoleDto.Support];

        public static bool IsScheduledRole(EmployeeRoleDto role) => ScheduledRoles.Contains(role);

        public static void ValidateTimes(TimeOnly start, TimeOnly end)
        {
            if (end <= start)
                throw new ArgumentException("The end time must be after the start time.");
            if (ScheduleRules.Hours(start, end) > ScheduleRules.MaxShiftHours)
                throw new ArgumentException($"A shift can be at most {ScheduleRules.MaxShiftHours} hours.");
        }

        public static void ValidateRole(EmployeeRoleDto role)
        {
            if (!Enum.IsDefined(role) || !IsScheduledRole(role))
                throw new ArgumentException("Pick a role that works in the store (manager, shift leader, service desk, cashier, floor worker or support).");
        }

        public static void Validate(this SaveShiftTemplateRequest request)
        {
            Guard.NotNull(request, "Shift");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("A shift needs a name.");
            if (request.Name.Trim().Length > FieldLimits.Name)
                throw new ArgumentException(string.Format(FieldLimits.TooLongMessage, "The name", FieldLimits.Name));
            if (((int)request.Days & 127) == 0 || ((int)request.Days & ~127) != 0)
                throw new ArgumentException("Pick at least one day.");
            ValidateTimes(request.StartTime, request.EndTime);
            ValidateRole(request.Role);
            if (request.RequiredStaff < 1 || request.RequiredStaff > ScheduleRules.MaxRequiredStaff)
                throw new ArgumentException($"The number of employees must be between 1 and {ScheduleRules.MaxRequiredStaff}.");
        }

        public static void Validate(this SaveStaffRequest request)
        {
            Guard.NotNull(request, "Staff");
            if (request.ContractHours < 0 || request.ContractHours > ScheduleRules.MaxContractHours)
                throw new ArgumentException($"Contract hours must be between 0 and {ScheduleRules.MaxContractHours}.");
            foreach (var slot in request.Availability ?? [])
            {
                if (!Enum.IsDefined(slot.Day))
                    throw new ArgumentException("A day of the availability is invalid.");
                if (slot.AvailableUntil <= slot.AvailableFrom)
                    throw new ArgumentException($"On {slot.Day} the end of the availability must be after the start.");
            }
            if ((request.Availability ?? []).GroupBy(a => a.Day).Any(g => g.Count() > 1))
                throw new ArgumentException("Give one availability window per day.");
        }

        public static void Validate(this SaveShiftRequest request, DateOnly weekStart)
        {
            Guard.NotNull(request, "Shift");
            if (request.Date < weekStart || request.Date > weekStart.AddDays(6))
                throw new ArgumentException("The date must be inside the week.");
            ValidateTimes(request.StartTime, request.EndTime);
            ValidateRole(request.Role);
            if (!string.IsNullOrWhiteSpace(request.EmployeeId) && !IdentifierFormats.IsEmployeeId(request.EmployeeId.Trim()))
                throw new ArgumentException($"An employee id must be {FieldLimits.EmployeeId} digits.");
        }

        public static void Validate(this CreateScheduleRequest request, DateOnly today)
        {
            Guard.NotNull(request, "Request");
            if (!Enum.IsDefined(request.Type))
                throw new ArgumentException("The type of request is invalid.");
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ArgumentException("Give a reason.");
            if (request.Reason.Trim().Length > FieldLimits.Reason)
                throw new ArgumentException(string.Format(FieldLimits.TooLongMessage, "The reason", FieldLimits.Reason));

            if (request.Type == ScheduleRequestTypeDto.DayOff)
            {
                if (request.Date is not DateOnly date)
                    throw new ArgumentException("Pick the day you want off.");
                if (date <= today)
                    throw new ArgumentException("A day off can only be asked for a day after today.");
            }
            else
            {
                if (request.ShiftId is null)
                    throw new ArgumentException("Pick the shift you want to swap.");
                if (string.IsNullOrWhiteSpace(request.TargetEmployeeId) || !IdentifierFormats.IsEmployeeId(request.TargetEmployeeId.Trim()))
                    throw new ArgumentException("Pick the colleague who takes over the shift.");
            }
        }

        public static void Validate(this ReviewScheduleRequest review)
        {
            if (review?.Note is { } note && note.Trim().Length > FieldLimits.Reason)
                throw new ArgumentException(string.Format(FieldLimits.TooLongMessage, "The note", FieldLimits.Reason));
        }
    }
}
