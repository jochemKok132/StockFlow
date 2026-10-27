using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces.Scheduling;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.API.Controllers.Scheduling
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController(IScheduleService scheduleService) : ControllerBase
    {
        private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
        private bool CanSeeDrafts => User.Allows(Permissions.ViewScheduleDrafts);
        private bool CanEditConfirmed => User.Allows(Permissions.ConfirmSchedule);

        [Authorize(Roles = Permissions.ViewSchedule)]
        [HttpGet("Weeks")]
        public Task<IActionResult> GetWeeksAsync() =>
            this.RunAsync(() => scheduleService.GetWeeksAsync(Today, CanSeeDrafts));

        [Authorize(Roles = Permissions.ViewSchedule)]
        [HttpGet("Week/{weekStart}")]
        public Task<IActionResult> GetWeekAsync(DateOnly weekStart) =>
            this.RunAsync(() => scheduleService.GetWeekAsync(weekStart, CanSeeDrafts));

        [Authorize(Roles = Permissions.ViewSchedule)]
        [HttpGet("Mine")]
        public Task<IActionResult> GetMineAsync() =>
            this.RunAsEmployeeAsync(userId => scheduleService.GetMyShiftsAsync(userId, Today));

        [Authorize(Roles = Permissions.ViewSchedule)]
        [HttpGet("Colleagues")]
        public Task<IActionResult> GetColleaguesAsync() =>
            this.RunAsEmployeeAsync(userId => scheduleService.GetColleaguesAsync(userId, Today));

        [Authorize(Roles = Permissions.EditSchedule)]
        [HttpPost("Week/{weekStart}/Generate")]
        public Task<IActionResult> GenerateAsync(DateOnly weekStart) =>
            this.RunAsync(() => scheduleService.GenerateWeekAsync(weekStart, Today), w => $"The week of {w.WeekStart:dd-MM-yyyy} was generated.");

        [Authorize(Roles = Permissions.ConfirmSchedule)]
        [HttpPost("Week/{weekStart}/Confirm")]
        public Task<IActionResult> ConfirmAsync(DateOnly weekStart) =>
            this.RunAsEmployeeAsync(userId => scheduleService.ConfirmWeekAsync(weekStart, userId, Today), w => $"The week of {w.WeekStart:dd-MM-yyyy} is confirmed.");

        [Authorize(Roles = Permissions.EditSchedule)]
        [HttpPost("Shift")]
        public Task<IActionResult> AddShiftAsync(SaveShiftRequest request) =>
            this.RunAsync(() => scheduleService.AddShiftAsync(request, CanEditConfirmed, Today), _ => "Shift added.");

        [Authorize(Roles = Permissions.EditSchedule)]
        [HttpPut("Shift/{id:guid}")]
        public Task<IActionResult> UpdateShiftAsync(Guid id, SaveShiftRequest request) =>
            this.RunAsync(() => scheduleService.UpdateShiftAsync(id, request, CanEditConfirmed, Today), _ => "Shift saved.");

        [Authorize(Roles = Permissions.EditSchedule)]
        [HttpDelete("Shift/{id:guid}")]
        public Task<IActionResult> DeleteShiftAsync(Guid id) =>
            this.RunAsync(() => scheduleService.DeleteShiftAsync(id, CanEditConfirmed, Today), _ => "Shift removed.");

        [Authorize(Roles = Permissions.ViewScheduleSettings)]
        [HttpGet("Templates")]
        public Task<IActionResult> GetTemplatesAsync() =>
            this.RunAsync(() => scheduleService.GetTemplatesAsync());

        [Authorize(Roles = Permissions.ManageScheduleSettings)]
        [HttpPost("Templates")]
        public Task<IActionResult> CreateTemplateAsync(SaveShiftTemplateRequest request) =>
            this.RunAsync(() => scheduleService.CreateTemplateAsync(request), t => $"Shift {t.Name} added.");

        [Authorize(Roles = Permissions.ManageScheduleSettings)]
        [HttpPut("Templates/{id:guid}")]
        public Task<IActionResult> UpdateTemplateAsync(Guid id, SaveShiftTemplateRequest request) =>
            this.RunAsync(() => scheduleService.UpdateTemplateAsync(id, request), t => $"Shift {t.Name} saved.");

        [Authorize(Roles = Permissions.ManageScheduleSettings)]
        [HttpDelete("Templates/{id:guid}")]
        public Task<IActionResult> DeleteTemplateAsync(Guid id) =>
            this.RunAsync(() => scheduleService.DeleteTemplateAsync(id), "Shift removed.");

        [Authorize(Roles = Permissions.ViewScheduleSettings)]
        [HttpGet("Staff")]
        public Task<IActionResult> GetStaffAsync() =>
            this.RunAsync(() => scheduleService.GetStaffAsync());

        [Authorize(Roles = Permissions.ManageScheduleSettings)]
        [HttpPut("Staff/{employeeId}")]
        public Task<IActionResult> SaveStaffAsync(string employeeId, SaveStaffRequest request) =>
            this.RunAsync(() => scheduleService.SaveStaffAsync(employeeId, request), s => $"Hours of {s.FullName} saved.");

        [Authorize(Roles = Permissions.ViewSchedule)]
        [HttpGet("Requests")]
        public Task<IActionResult> GetRequestsAsync([FromQuery] ScheduleRequestStatusDto? status) =>
            this.RunAsEmployeeAsync(userId => scheduleService.GetRequestsAsync(status, userId, User.Allows(Permissions.ReviewScheduleRequests) || User.IsInRole(AccessRules.ViewOnlyRole)));

        [Authorize(Roles = Permissions.RequestScheduleChanges)]
        [HttpPost("Requests")]
        public Task<IActionResult> CreateRequestAsync(CreateScheduleRequest request) =>
            this.RunAsEmployeeAsync(userId => scheduleService.CreateRequestAsync(request, userId, Today), r => $"Request {r.RequestNumber} sent.");

        [Authorize(Roles = Permissions.RequestScheduleChanges)]
        [HttpPost("Requests/{id:guid}/Cancel")]
        public Task<IActionResult> CancelRequestAsync(Guid id) =>
            this.RunAsEmployeeAsync(userId => scheduleService.CancelRequestAsync(id, userId), r => $"Request {r.RequestNumber} cancelled.");

        [Authorize(Roles = Permissions.ReviewScheduleRequests)]
        [HttpPost("Requests/{id:guid}/Approve")]
        public Task<IActionResult> ApproveRequestAsync(Guid id, ReviewScheduleRequest review) =>
            this.RunAsEmployeeAsync(userId => scheduleService.ApproveRequestAsync(id, review, userId, Today), r => $"Request {r.RequestNumber} approved.");

        [Authorize(Roles = Permissions.ReviewScheduleRequests)]
        [HttpPost("Requests/{id:guid}/Reject")]
        public Task<IActionResult> RejectRequestAsync(Guid id, ReviewScheduleRequest review) =>
            this.RunAsEmployeeAsync(userId => scheduleService.RejectRequestAsync(id, review, userId), r => $"Request {r.RequestNumber} rejected.");
    }
}
