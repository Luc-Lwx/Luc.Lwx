### Lwx Activity Log

This README provides an overview of the `LwxActivityLog` library, which is used for logging and observability in .NET applications. It includes setup instructions and examples of how to use the library.

#### Setup

1. **Configure the Activity Log Output**

   You can configure the output of the activity log by implementing the `ILwxActivityLogOutput` interface and registering it in the DI container:

   ```csharp
   public class CustomActivityLogOutput : ILwxActivityLogOutput
   {
       public void Publish(LwxActivityRecord record)
       {
           // Custom logic to handle the published record
       }
   }

   var builder = new LwxApiBuilder(args);

   builder.LwxConfigureActivityLogOutput(new CustomActivityLogOutput());

   // ...

   var app = await builder.Build();

   // ...

   await app.RunAsync();
   ```

3. **Configure Settings in `appsettings.json`**

   Add the `LwxActivityLog` section to your `appsettings.json` file:

   ```json
   {
       "LwxActivityLog": {
           "FixIpAddr": true,
           "IgnoreEndpointsWithoutAttribute": true,
           "ErrorHandler": true
       }
   }
   ```

#### Examples

1. **Logging a Request and Response**

   The middleware automatically logs requests and responses. You can access the activity record in your endpoints:

   ```csharp
   public static partial class ExampleEndpoint
   { 
       [LwxEndpoint(
         Path = "GET /example",
         AuthPolicy = typeof(AuthPolicyPublic),
         SwaggerFuncName = "Example",
         SwaggerFuncSummary = "Example endpoint",
         SwaggerFuncDescription = "This endpoint demonstrates logging a request and response.",
         SwaggerGroupTitle = "Example"
       )]
       [LwxActivityLog(
         Imporance = LwxActivityImportance.Medium,
         Step = LwxActionStep.Step,
         ShortDescription = "Logs request and response"
       )]
       public async static Task<IResult> Execute(HttpContext ctx)
       {
           var record = ctx.LwxGetActivityRecord();
           // Add custom context information
           record?.SetContextInfo("CustomKey", "CustomValue");

           return Results.Ok("Example response");
       }
   }
   ```

2. **Setting Request and Response Bodies**

   You can set the request and response bodies in the activity record:

   ```csharp
   public static partial class ExampleEndpoint
   { 
       [LwxEndpoint(
         Path = "POST /example",
         AuthPolicy = typeof(AuthPolicyPublic),
         SwaggerFuncName = "Example",
         SwaggerFuncSummary = "Example endpoint",
         SwaggerFuncDescription = "This endpoint demonstrates setting request and response bodies.",
         SwaggerGroupTitle = "Example"
       )]
       [LwxActivityLog(
         Imporance = LwxActivityImportance.Medium,
         Step = LwxActionStep.Step,
         ShortDescription = "Sets request and response bodies"
       )]
       public async static Task<IResult> Execute(HttpContext ctx, [FromBody] object requestBody)
       {
           ctx.LwxSetActivityRecordRequestBodyJson(requestBody);

           var responseBody = new { Message = "Example response" };
           ctx.LwxSetActivityRecordResponseBodyJson(responseBody);

           return Results.Ok(responseBody);
       }
   }
   ```

3. **Handling Exceptions**

   The middleware can handle exceptions and generate standardized error responses:

   ```csharp
   public static partial class ExampleEndpoint
   { 
       [LwxEndpoint(
         Path = "GET /error",
         AuthPolicy = typeof(AuthPolicyPublic),
         SwaggerFuncName = "Error",
         SwaggerFuncSummary = "Error endpoint",
         SwaggerFuncDescription = "This endpoint demonstrates handling exceptions.",
         SwaggerGroupTitle = "Example"
       )]
       [LwxActivityLog(
         Imporance = LwxActivityImportance.High,
         Step = LwxActionStep.Finish,
         ShortDescription = "Handles exceptions"
       )]
       public async static Task<IResult> Execute(HttpContext ctx)
       {
           throw new Exception("An error occurred");
       }
   }
   ```

   With `ErrorHandler` enabled in the configuration, the middleware will catch the exception and return a standardized error response.

#### Conclusion

The `LwxActivityLog` library provides a comprehensive solution for logging and observability in .NET applications. By following the setup instructions and using the provided examples, you can easily integrate it into your application and start capturing valuable insights into your application's behavior.