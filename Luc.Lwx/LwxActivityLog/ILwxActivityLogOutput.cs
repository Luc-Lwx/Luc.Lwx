namespace Luc.Lwx.LwxActivityLog;

/// <summary>
/// Represents the output interface for observability.
/// </summary>
public interface ILwxActivityLogOutput 
{
    void Publish(LwxRecord record);
}


