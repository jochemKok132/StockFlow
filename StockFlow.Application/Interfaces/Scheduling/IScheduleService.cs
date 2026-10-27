using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.Application.Interfaces.Scheduling
{
    public interface IScheduleService
    {
        Task<List<ShiftTemplateDto>> GetTemplatesAsync();
        Task<ShiftTemplateDto> CreateTemplateAsync(SaveShiftTemplateRequest request);
        Task<ShiftTemplateDto> UpdateTemplateAsync(Guid id, SaveShiftTemplateRequest request);
        Task DeleteTemplateAsync(Guid id);

        Task<List<StaffMemberDto>> GetStaffAsync();
        Task<StaffMemberDto> SaveStaffAsync(string employeeNumber, SaveStaffRequest request);

        Task<List<ScheduleWeekSummaryDto>> GetWeeksAsync(DateOnly today, bool canSeeDrafts);
        Task<ScheduleWeekDto> GetWeekAsync(DateOnly weekStart, bool canSeeDrafts);
        Task<ScheduleWeekDto> GenerateWeekAsync(DateOnly weekStart, DateOnly today);
        Task<int> GenerateUpcomingWeeksAsync(DateOnly today);
        Task<ScheduleWeekDto> ConfirmWeekAsync(DateOnly weekStart, Guid employeeId, DateOnly today);
        Task<ScheduleWeekDto> AddShiftAsync(SaveShiftRequest request, bool canEditConfirmed, DateOnly today);
        Task<ScheduleWeekDto> UpdateShiftAsync(Guid shiftId, SaveShiftRequest request, bool canEditConfirmed, DateOnly today);
        Task<ScheduleWeekDto> DeleteShiftAsync(Guid shiftId, bool canEditConfirmed, DateOnly today);
        Task<List<ScheduledShiftDto>> GetMyShiftsAsync(Guid employeeId, DateOnly today);
        Task<List<ColleagueDto>> GetColleaguesAsync(Guid employeeId, DateOnly today);

        Task<List<ScheduleRequestDto>> GetRequestsAsync(ScheduleRequestStatusDto? status, Guid employeeId, bool canReview);
        Task<ScheduleRequestDto> CreateRequestAsync(CreateScheduleRequest request, Guid employeeId, DateOnly today);
        Task<ScheduleRequestDto> CancelRequestAsync(Guid id, Guid employeeId);
        Task<ScheduleRequestDto> ApproveRequestAsync(Guid id, ReviewScheduleRequest review, Guid reviewerId, DateOnly today);
        Task<ScheduleRequestDto> RejectRequestAsync(Guid id, ReviewScheduleRequest review, Guid reviewerId);
    }
}
