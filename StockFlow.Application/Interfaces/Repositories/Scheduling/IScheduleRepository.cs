using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Interfaces.Repositories.Scheduling
{
    public interface IScheduleRepository
    {
        Task<List<ShiftTemplate>> GetTemplatesAsync();
        Task<ShiftTemplate?> GetTemplateAsync(Guid id);
        Task AddTemplateAsync(ShiftTemplate template);
        Task RemoveTemplateAsync(ShiftTemplate template);

        Task<List<Employee>> GetStaffAsync();
        Task<Employee?> GetStaffMemberAsync(string employeeNumber);
        void ReplaceAvailability(Employee employee, List<EmployeeAvailability> availability);

        Task<ScheduleWeek?> GetWeekAsync(DateOnly weekStart);
        Task<List<ScheduleWeek>> GetWeeksAsync(DateOnly from, DateOnly until);
        Task AddWeekAsync(ScheduleWeek week);
        void AddShifts(IEnumerable<ScheduledShift> shifts);
        void RemoveShifts(IEnumerable<ScheduledShift> shifts);
        Task<ScheduledShift?> GetShiftAsync(Guid id);
        Task<List<ScheduledShift>> GetConfirmedShiftsAsync(Guid employeeId, DateOnly from, DateOnly until);

        Task<List<ScheduleRequest>> GetRequestsAsync(ScheduleRequestStatus? status, Guid? employeeId);
        Task<ScheduleRequest?> GetRequestAsync(Guid id);
        Task<int> GetNextRequestNumberAsync();
        Task AddRequestAsync(ScheduleRequest request);
        Task<bool> HasPendingRequestAsync(Guid employeeId, Guid? shiftId, DateOnly? date);
        Task<HashSet<(Guid EmployeeId, DateOnly Date)>> GetApprovedDaysOffAsync(DateOnly from, DateOnly until);

        Task SaveChangesAsync();
    }
}
