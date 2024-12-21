namespace Luc.Lwx.LwxActivityLog;

using Luc.Lwx.Interface;
using System.Text;
using System.Text.Json;
using System.Globalization;

/// <summary>
/// Represents the output interface for observability.
/// </summary>
public interface ILwxActivityLogOutput 
{
    void Publish(LwxRecord record);
}

public class LwxActivityLogMiddleware 
(
    ILwxActivityLogOutput output,
    LwxActivityLogConfig config
) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    { 
        if( config.FixIpAddr )
        {
            UpdateRequestIpAndPort(context); 
        }
        
        var record = CreateActivityLogRecord(context);
        if( record == null )
        {
            await next(context);
            return;
        }

        context.SetLwxActivityRecord(record);

        // Set Host, RemoteIp, and RemotePort
        record.Host = context.Request.Host.Value;
        record.RemoteIp = context.Connection.RemoteIpAddress?.ToString();
        record.RemotePort = context.Connection.RemotePort;

        Stream originalBodyStream;
        if( record.ResponseBodyMode != LwxRecordBodyCaptureMode.Ignored )
        {
            originalBodyStream = context.Response.Body;
            context.Response.Body = new MemoryStream();
        }
        else
        {
            originalBodyStream = Stream.Null;
        }

        UpdateRequestHeaders(record, context);
      
        if( record.RequestBodyMode != LwxRecordBodyCaptureMode.Ignored )
        {
            await UpdateRequestBody(record, context);
        }

        // Extract and parse JWT token
        ExtractJwtTokenInfo(record, context);

        try
        {
            await next(context);
        }
        catch (LwxResponseException e)
        {
            await HandleResponseException(e, context);
        }
        catch (LwxOverrideResponseException e)
        {
            await HandleOverrideResponseException(e, context);
        }
        catch (Exception e)
        {
            if (config.ErrorHandler)
            {
                await HandleGeneralException(e, context);
            }
            else
            {
                throw;
            }
        }

        UpdateResponseHeaders(record, context);

        if( record.ResponseBodyMode != LwxRecordBodyCaptureMode.Ignored )
        {
            await UpdateResponseBody(record, context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
        }

        output.Publish(record);
    }
   
    /// <summary>
    /// Creates an OperationRecord based on the current HttpContext.
    /// </summary>
    private LwxRecord? CreateActivityLogRecord(HttpContext context) 
    {
        var endpoint = context.GetEndpoint();
        var lwxActivtyLogAttribute = endpoint?.Metadata.GetMetadata<LwxActivityLogAttribute>();
        if (lwxActivtyLogAttribute == null) 
        {            
            if( config.IgnoreEndpointsWithoutAttribute )
            {
                return null;
            }             
        }
        else if( lwxActivtyLogAttribute.Imporance == LwxActivityImportance.Ignore) 
        {
            return null;
        }
        
        var routePattern = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.RouteEndpoint>()?.RoutePattern.RawText;
        var routeValues = context.Request.RouteValues;
        var requestPathParams = routeValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? string.Empty);

        if (routePattern == null)
        {
            routePattern = "{path}";
            requestPathParams["path"] = context.Request.Path.ToString();
        }

        var result = new LwxRecord
        {
            RequestPath = routePattern, 
            RequestPathParams = requestPathParams,
            Importance = lwxActivtyLogAttribute?.Imporance,
            Step = lwxActivtyLogAttribute?.Step,               
            RequestBodyMode = lwxActivtyLogAttribute?.RequestBodyMode,
            ResponseBodyMode = lwxActivtyLogAttribute?.ResponseBodyMode
        };        

        // when the user declared that will provide a custom body, we need to set the default to ignored         
        if( result.RequestBodyMode == LwxRecordBodyCaptureMode.Custom )
        {
            result.RequestBodyMode = LwxRecordBodyCaptureMode.Ignored;
        }
        // when the user declared that will provide a custom body, we need to set the default to ignored 
        if( result.ResponseBodyMode == LwxRecordBodyCaptureMode.Custom )
        {
            result.ResponseBodyMode = LwxRecordBodyCaptureMode.Ignored;
        }        
        return result;
    }

    /// <summary>
    /// Extracts and parses the JWT token information from the HttpContext.
    /// </summary>
    private static void ExtractJwtTokenInfo(LwxRecord record, HttpContext context)
    {
        var user = context.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            foreach (var claim in user.Claims)
            {
                switch (claim.Type)
                {
                    case "sub":
                        // Subject identifier - unique ID of the user
                        // Example: "sub": "1234567890"
                        record.AuthUserId = claim.Value;
                        break;
                    case "preferred_username":
                        // Preferred username of the user
                        // Example: "preferred_username": "johndoe"
                        record.AuthUserName = claim.Value;
                        break;
                    case "email":
                        // Email address of the user
                        // Example: "email": "johndoe@example.com"
                        record.AuthEmail = claim.Value;
                        break;
                    case "name":
                        // Full name of the user
                        // Example: "name": "John Doe"
                        record.AuthFullName = claim.Value;
                        break;
                    case "given_name":
                        // Ignore given name since full name is captured
                        break;
                    case "family_name":
                        // Ignore family name since full name is captured
                        break;
                    case "azp":
                        // Authorized party - client ID of the application
                        // Example: "azp": "myclient"
                        record.AuthAuthorizedParty = claim.Value;
                        break;
                    case "aud":
                        // Audience - intended recipients of the token
                        // Example: "aud": "myclient"
                        record.AuthAudience = claim.Value;
                        break;
                    case "realm_access":
                        // Roles assigned to the user in the realm
                        // Example: "realm_access": "{\"roles\":[\"user\",\"admin\"]}"
                        record.AuthRoles = claim.Value;
                        break;
                    case "resource_access":
                        // Roles assigned to the user for specific resources
                        // Example: "resource_access": "{\"myclient\":{\"roles\":[\"custom-role\"]}}"
                        record.AuthResources = claim.Value;
                        break;
                    // ...existing code...
                    default:
                         // Capture extra claims
                        record.AuthExtra ??= [];
                        record.AuthExtra[claim.Type] = claim.Value;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Updates the IP address and port of the request using the X-Forwarded-For header.
    /// </summary>
    private static void UpdateRequestIpAndPort(HttpContext context)
    {
        // Examples generated by Copilot
        // X-Forwarded-For: 203.0.113.195
        // X-Forwarded-For: 203.0.113.195, 70.41.3.18, 150.172.238.178
        // X-Forwarded-For: 2001:0db8:85a3:0000:0000:8a2e:0370:7334
        // X-Forwarded-For: 2001:0db8:85a3:0000:0000:8a2e:0370:7334, 203.0.113.195
        // X-Forwarded-For: 203.0.113.195:8080
        // X-Forwarded-For: [2001:0db8:85a3:0000:0000:8a2e:0370:7334]:8080

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ipPort = forwardedFor.FirstOrDefault()?.Split(',');
            if (ipPort != null && ipPort.Length > 0)
            {
                var ip = ipPort[0].Trim();
                
                // Save the original IP and port for logging purposes
                context.Request.Headers["Original-Remote-Ip"] = ip;

                // Try parsing the IP address without port first
                if (System.Net.IPAddress.TryParse(ip, out var ipAddress))
                {
                    context.Connection.RemoteIpAddress = ipAddress;
                    context.Connection.RemotePort = 0;
                }
                else if (System.Net.IPEndPoint.TryParse(ip, out var endPoint))
                {
                    context.Connection.RemoteIpAddress = endPoint.Address;
                    context.Connection.RemotePort = endPoint.Port;
                }
            }
        }
    }

    /// <summary>
    /// Updates the request headers of the operation record.
    /// </summary>
    private static void UpdateRequestHeaders(LwxRecord record, HttpContext context)
    {
        record.RequestHeaders = context.Request.Headers
            .Where(kvp => !string.Equals(kvp.Key, "Host", StringComparison.OrdinalIgnoreCase))
            .GroupBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => string.Join("\r\n", g.SelectMany(v => v.Value))
            );
    }

    /// <summary>
    /// Updates the response headers of the operation record.
    /// </summary>
    private static void UpdateResponseHeaders(LwxRecord record, HttpContext context)
    {
        record.ResponseHeaders = context.Response.Headers
            .GroupBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => string.Join("\r\n", g.SelectMany(v => v.Value))
            );
    }

    /// <summary>
    /// Updates the request body of the operation record.
    /// </summary>
    private static async Task UpdateRequestBody(LwxRecord record, HttpContext context)
    {
        if (record.RequestBodyMode != LwxRecordBodyCaptureMode.Ignored)
        {
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            var contentType = context.Request.ContentType?.ToLower(CultureInfo.InvariantCulture) ?? throw new InvalidOperationException("Request content type is null.");
            var mediaType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
            var charset = mediaType.CharSet?.ToLower(CultureInfo.InvariantCulture);

            if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Application.Json && charset == "utf-8")
            {
                record.RequestBodyType = LwxRecordBodyType.Json;
                record.RequestBodyJson = requestBody;
            }
            else if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Text.Plain)
            {
                record.RequestBody = requestBody;
                record.RequestBodyType = LwxRecordBodyType.Text;
            }
            else
            {
                record.RequestBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestBody));
                record.RequestBodyType = LwxRecordBodyType.Base64;
            }
        }
    }
   
    private static async Task UpdateResponseBody(LwxRecord record, HttpContext context)
    {
        if (record.ResponseBodyMode != LwxRecordBodyCaptureMode.Ignored)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var contentType = (context.Response.ContentType?.ToLower(CultureInfo.InvariantCulture)) ?? throw new InvalidOperationException("Response content type is null.");
            var mediaType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
            var charset = mediaType.CharSet?.ToLower(CultureInfo.InvariantCulture);

            if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Application.Json && charset == "utf-8")
            {
                record.ResponseBodyJson = responseBody;
                record.ResponseBodyType = LwxRecordBodyType.Json;
            }
            else if (mediaType.MediaType == System.Net.Mime.MediaTypeNames.Text.Plain)
            {
                record.ResponseBody = responseBody;
                record.ResponseBodyType = LwxRecordBodyType.Text;
            }
            else
            {
                record.ResponseBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseBody));
                record.ResponseBodyType = LwxRecordBodyType.Base64;
            }
        }
    }
    
    private static async Task HandleResponseException(LwxResponseException e, HttpContext context)
    {
        var response = new LwxResponseDto
        {
            Ok = false,
            ErrorCode = e.StatusCode.ToString(),
            ErrorMessage = e.Message
        };
        var responseString = JsonSerializer.Serialize(response);

        context.Response.StatusCode = e.StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(responseString);
    }

    private static async Task HandleOverrideResponseException(LwxOverrideResponseException e, HttpContext context)
    {
        var responseString = e.Content?.ToString() ?? string.Empty;
        var contentType = e.ContentType;

        if (contentType == null)
        {
            if (e.Content is string)
            {
                contentType = "text/plain";
            }
            else
            {
                contentType = "application/json";
                responseString = JsonSerializer.Serialize(e.Content);
            }
        }

        context.Response.StatusCode = e.StatusCode;
        context.Response.ContentType = contentType;
        await context.Response.WriteAsync(responseString);
    }

    private static async Task HandleGeneralException(Exception e, HttpContext context)
    {
        var response = new LwxResponseDto
        {
            Ok = false,
            ErrorCode = "500",
            ErrorMessage = e.Message
        };
        var responseString = JsonSerializer.Serialize(response);

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(responseString);
    }
}


