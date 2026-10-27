using StockFlow.ManagementApp.Services;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.ManagementApp.Interfaces.Scheduling
{
    public interface IScheduleService
    {
        Task<ApiResult<List<ScheduleWeekSummaryDto>>> GetWeeksAsync();
        Task<ApiResult<ScheduleWeekDto>> GetWeekAsync(DateOnly weekStart);
        Task<ApiResult<List<ScheduledShiftDto>>> GetMyShiftsAsync();
        Task<ApiResult<List<ColleagueDto>>> GetColleaguesAsync();
        Task<ApiResult<ScheduleWeekDto>> GenerateAsync(DateOnly weekStart);
        Task<ApiResult<ScheduleWeekDto>> ConfirmAsync(DateOnly weekStart);
        Task<ApiResult<ScheduleWeekDto>> AddShiftAsync(SaveShiftRequest request);
        Task<ApiResult<ScheduleWeekDto>> UpdateShiftAsync(Guid id, SaveShiftRequest request);
        Task<ApiResult<ScheduleWeekDto>> DeleteShiftAsync(Guid id);
        Task<ApiResult<List<ShiftTemplateDto>>> GetTemplatesAsync();
        Task<ApiResult<ShiftTemplateDto>> CreateTemplateAsync(SaveShiftTemplateRequest request);
        Task<ApiResult<ShiftTemplateDto>> UpdateTemplateAsync(Guid id, SaveShiftTemplateRequest request);
        Task<ApiResult<object>> DeleteTemplateAsync(Guid id);
        Task<ApiResult<List<StaffMemberDto>>> GetStaffAsync();
        Task<ApiResult<StaffMemberDto>> SaveStaffAsync(string employeeId, SaveStaffRequest request);
        Task<ApiResult<List<ScheduleRequestDto>>> GetRequestsAsync(ScheduleRequestStatusDto? status);
        Task<ApiResult<ScheduleRequestDto>> CreateRequestAsync(CreateScheduleRequest request);
        Task<ApiResult<ScheduleRequestDto>> CancelRequestAsync(Guid id);
        Task<ApiResult<ScheduleRequestDto>> ApproveRequestAsync(Guid id, ReviewScheduleRequest review);
        Task<ApiResult<ScheduleRequestDto>> RejectRequestAsync(Guid id, ReviewScheduleRequest review);
    }
}
