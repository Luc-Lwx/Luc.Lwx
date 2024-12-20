
using System.Collections.Concurrent;

namespace Luc.Web.LwxActivityLog;

/// <summary>
/// This class is used to store the output of the observability system for testing purposes.
/// </summary>
public class LwxActivityLogTestOutput : ILwxActivityLogOutput
{
    private readonly ConcurrentDictionary<string, List<LwxRecord>> _records = new();
    
    public void Publish(LwxRecord record)
    {
        if (record.RequestPath == null)
        {
            throw new ArgumentException($"{nameof(record.RequestPath)} cannot be null");
        }

        _records.AddOrUpdate(
            record.RequestPath,
            [record],
            (key, existingList) =>
            {
                existingList.Add(record);
                return existingList;
            });
    }

    public IReadOnlyDictionary<string, List<LwxRecord>> GetRecords()
    {
        return _records;
    }
}
