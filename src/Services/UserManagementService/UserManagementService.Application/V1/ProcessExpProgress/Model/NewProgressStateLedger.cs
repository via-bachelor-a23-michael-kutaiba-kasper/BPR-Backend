using System.Collections.Concurrent;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model;

public class ExperienceGainedLedger
{
    private readonly IDictionary<string, IExpGeneratingEvent> _ledger = new ConcurrentDictionary<string, IExpGeneratingEvent>();

    public void RegisterExpGeneratingEvent(string userId, Func<IExpGeneratingEvent, IExpGeneratingEvent> eventFactory)
    {
        if (!_ledger.ContainsKey(userId))
        {
            _ledger.Add(userId, eventFactory.Invoke(null));
            return;
        }

        var currentDecorator = _ledger[userId];
        var updatedDecorator = eventFactory.Invoke(currentDecorator);
        
        _ledger[userId] = updatedDecorator;
    }
    public IEnumerable<string> GetUserIds()
    {
        return _ledger.Keys.ToList();
    }

    public long GetExperienceGained(string userId)
    {
        return !_ledger.ContainsKey(userId) ? 0 : _ledger[userId].GetExperienceGained();
    }

}