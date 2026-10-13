using StockFlow.Shared.DTOs.Releases;

namespace StockFlow.Infrastructure.Releases
{
    public class ReleaseCache
    {
        private readonly object _lock = new();
        private List<ReleaseDto>? _releases;
        private DateTime _expiresAt;

        public bool TryGet(out List<ReleaseDto> releases)
        {
            lock (_lock)
            {
                if (_releases is not null && DateTime.UtcNow < _expiresAt)
                {
                    releases = _releases;
                    return true;
                }

                releases = [];
                return false;
            }
        }

        public List<ReleaseDto>? Stale
        {
            get
            {
                lock (_lock)
                    return _releases;
            }
        }

        public void Set(List<ReleaseDto> releases, TimeSpan timeToLive)
        {
            lock (_lock)
            {
                _releases = releases;
                _expiresAt = DateTime.UtcNow.Add(timeToLive);
            }
        }

        public void Invalidate()
        {
            lock (_lock)
                _expiresAt = DateTime.MinValue;
        }
    }
}
