using System.Collections.Concurrent;
using Luc.Lwx.LwxActivityLog;

namespace Luc.Lwx.Interface;

/// <summary>
/// This attribute is set the endpoint activity log generation options
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class LwxActivityLogAttribute : Attribute
{
    /// <summary>
    /// Inform the importance of this step for archiving and monitoring purposes
    /// </summary>
    /// <remarks>
    /// When the API modifies userdata, changes authorization, make purchases, the importance should be high.
    /// </remarks>
    public required LwxActivityImportance Imporance { get; set; }

    /// <summary>
    /// Inform the step for archiving and monitoring purposes
    /// </summary>
    public required LwxActionStep Step { get; set; }


    /// <summary>
    /// A short description of what this operation does
    /// </summary>
    public required string? ShortDescription { get; set; } = null;

    /// <summary>
    /// Inform how the request body should be handled (default is to be captured)
    /// </summary>
    public bool CaptureRequestBody { get; set; } = true;

    /// <summary>
    /// Inform how the response body should be handled (default is to be captured)
    /// </summary>
    public bool CaptureResponseBody { get; set; } = true;

}