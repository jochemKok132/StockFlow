using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Scheduling;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.ManagementApp.Services.Scheduling
{
    public class ScheduleService(IHttpService httpService) : IScheduleService
    {
        private static string Day(DateOnly date) => date.ToString("yyyy-MM-dd");

        public async Task<ApiResult<List<ScheduleWeekSummaryDto>>> GetWeeksAsync() =>
            ApiResult<List<ScheduleWeekSummaryDto>>.From(await httpService.GetAsync<List<ScheduleWeekSummaryDto>>("Schedule/Weeks"), "The weeks could not be loaded.");

        public async Task<ApiResult<ScheduleWeekDto>> GetWeekAsync(DateOnly weekStart) =>
            ApiResult<ScheduleWeekDto>.From(await httpService.GetAsync<ScheduleWeekDto>($"Schedule/Week/{Day(weekStart)}"), "The schedule could not be loaded.");

        public async Task<ApiResult<List<ScheduledShiftDto>>> GetMyShiftsAsync() =>
            ApiResult<List<ScheduledShiftDto>>.From(await httpService.GetAsync<List<ScheduledShiftDto>>("Schedule/Mine"), "Your shifts could not be loaded.");

        public async Task<ApiResult<List<ColleagueDto>>> GetColleaguesAsync() =>
            ApiResult<List<ColleagueDto>>.From(await httpService.GetAsync<List<ColleagueDto>>("Schedule/Colleagues"), "Your colleagues could not be loaded.");

        public async Task<ApiResult<ScheduleWeekDto>> GenerateAsync(DateOnly weekStart) =>
            ApiResult<ScheduleWeekDto>.From(await httpService.PostAsync<ScheduleWeekDto, object>($"Schedule/Week/{Day(weekStart)}/Generate", new { }), "The week could not be generated.");

        public async Task<ApiResult<ScheduleWeekDto>> ConfirmAsync(DateOnly weekStart) =>
            ApiResult<ScheduleWeekDto>.From(await httpService.PostAsync<ScheduleWeekDto, object>($"Schedule/Week/{Day(weekStart)}/Confirm", new { }), "The week could not be confirmed.");

        public async Task<ApiResult<ScheduleWeekDto>> AddShiftAsync(SaveShiftRequest request) =>
            ApiResult<ScheduleWeekDto>.From(await httpService.PostAsync<ScheduleWeekDto, SaveShiftRequest>("Schedule/Shift", request), "The shift could not be added.");

        public async Task<ApiResult<ScheduleWeekDto>> UpdateShiftAsync(Guid id, SaveShiftRequest request) =>
            ApiResult<ScheduleWeekDto>.From(await httpService.PutAsync<ScheduleWeekDto, SaveShiftRequest>($"Schedule/Shift/{id}", request), "The shift could not be saved.");

        public async Task<ApiResult<ScheduleWeekDto>> DeleteShiftAsync(Guid id) =>
            ApiResult<ScheduleWeekDto>.From(await httpService.DeleteAsync<ScheduleWeekDto>($"Schedule/Shift/{id}"), "The shift could not be removed.");

        public async Task<ApiResult<List<ShiftTemplateDto>>> GetTemplatesAsync() =>
            ApiResult<List<ShiftTemplateDto>>.From(await httpService.GetAsync<List<ShiftTemplateDto>>("Schedule/Templates"), "The shifts could not be loaded.");

        public async Task<ApiResult<ShiftTemplateDto>> CreateTemplateAsync(SaveShiftTemplateRequest request) =>
            ApiResult<ShiftTemplateDto>.From(await httpService.PostAsync<ShiftTemplateDto, SaveShiftTemplateRequest>("Schedule/Templates", request), "The shift could not be added.");

        public async Task<ApiResult<ShiftTemplateDto>> UpdateTemplateAsync(Guid id, SaveShiftTemplateRequest request) =>
            ApiResult<ShiftTemplateDto>.From(await httpService.PutAsync<ShiftTemplateDto, SaveShiftTemplateRequest>($"Schedule/Templates/{id}", request), "The shift could not be saved.");

        public async Task<ApiResult<object>> DeleteTemplateAsync(Guid id) =>
            ApiResult<object>.From(await httpService.DeleteAsync<object>($"Schedule/Templates/{id}"), "The shift could not be removed.");

        public async Task<ApiResult<List<StaffMemberDto>>> GetStaffAsync() =>
            ApiResult<List<StaffMemberDto>>.From(await httpService.GetAsync<List<StaffMemberDto>>("Schedule/Staff"), "The staff could not be loaded.");

        public async Task<ApiResult<StaffMemberDto>> SaveStaffAsync(string employeeId, SaveStaffRequest request) =>
            ApiResult<StaffMemberDto>.From(await httpService.PutAsync<StaffMemberDto, SaveStaffRequest>($"Schedule/Staff/{Uri.EscapeDataString(employeeId)}", request), "The hours could not be saved.");

        public async Task<ApiResult<List<ScheduleRequestDto>>> GetRequestsAsync(ScheduleRequestStatusDto? status) =>
            ApiResult<List<ScheduleRequestDto>>.From(await httpService.GetAsync<List<ScheduleRequestDto>>("Schedule/Requests" + (status is null ? "" : $"?status={status}")), "The requests could not be loaded.");

        public async Task<ApiResult<ScheduleRequestDto>> CreateRequestAsync(CreateScheduleRequest request) =>
            ApiResult<ScheduleRequestDto>.From(await httpService.PostAsync<ScheduleRequestDto, CreateScheduleRequest>("Schedule/Requests", request), "The request could not be sent.");

        public async Task<ApiResult<ScheduleRequestDto>> CancelRequestAsync(Guid id) =>
            ApiResult<ScheduleRequestDto>.From(await httpService.PostAsync<ScheduleRequestDto, object>($"Schedule/Requests/{id}/Cancel", new { }), "The request could not be cancelled.");

        public async Task<ApiResult<ScheduleRequestDto>> ApproveRequestAsync(Guid id, ReviewScheduleRequest review) =>
            ApiResult<ScheduleRequestDto>.From(await httpService.PostAsync<ScheduleRequestDto, ReviewScheduleRequest>($"Schedule/Requests/{id}/Approve", review), "The request could not be approved.");

        public async Task<ApiResult<ScheduleRequestDto>> RejectRequestAsync(Guid id, ReviewScheduleRequest review) =>
            ApiResult<ScheduleRequestDto>.From(await httpService.PostAsync<ScheduleRequestDto, ReviewScheduleRequest>($"Schedule/Requests/{id}/Reject", review), "The request could not be rejected.");
    }
}
