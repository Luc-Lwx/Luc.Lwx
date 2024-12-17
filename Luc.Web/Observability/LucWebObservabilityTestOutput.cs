
using System.Collections.Concurrent;

namespace Luc.Web.Observability;

/// <summary>
/// This class is used to store the output of the observability system for testing purposes.
/// </summary>
public class LucWebObservabilityTestOutput : ILucWebObservabilityOutput
{
    private readonly ConcurrentDictionary<string, List<OperationRecord>> _records = new();
    
    public void Publish(OperationRecord record)
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

    public IReadOnlyDictionary<string, List<OperationRecord>> GetRecords()
    {
        return _records;
    }
}
