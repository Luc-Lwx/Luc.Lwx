namespace Luc.Lwx.LwxActivityLog;

public class LwxActivityLogConfig
{
    /// <summary>
    /// Indicates whether to ignore endpoints that do not have the [LwxEndpoint] attribute.
    /// Default is true.
    /// </summary>
    public bool IgnoreEndpointsWithoutAttribute { get; set; } = true;    
    
    /// <summary>
    /// Indicates whether to fix the IP address and port using the X-Forwarded-For header.
    /// Default is true.
    /// </summary>
    public bool FixIpAddr { get; set; } = true;
    
    /// <summary>
    /// Indicates whether to handle exceptions and generate a standardized error response.
    /// Default is true.
    /// </summary>
    public bool ErrorHandler { get; set; } = true;
}


