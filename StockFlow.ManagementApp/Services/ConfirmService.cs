namespace StockFlow.ManagementApp.Services
{
    public record ConfirmRequest(string Title, string Message, string ConfirmText, bool Danger, IReadOnlyList<string> Details);

    public class ConfirmService
    {
        private TaskCompletionSource<bool>? pending;

        public event Action? Changed;

        public ConfirmRequest? Current { get; private set; }

        public Task<bool> AskAsync(string title, string message, string confirmText = "Confirm", bool danger = false, IReadOnlyList<string>? details = null)
        {
            pending?.TrySetResult(false);

            pending = new TaskCompletionSource<bool>();
            Current = new ConfirmRequest(title, message, confirmText, danger, details ?? []);
            Changed?.Invoke();

            return pending.Task;
        }

        public void Answer(bool confirmed)
        {
            var answer = pending;
            pending = null;
            Current = null;
            Changed?.Invoke();

            answer?.TrySetResult(confirmed);
        }
    }
}
