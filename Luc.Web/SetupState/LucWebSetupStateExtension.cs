namespace Luc.Web.SetupState;

public static class LucWebSetupStateExtension
{
  internal static LucWebSetupState GetLucSetupState(this WebApplicationBuilder builder)
  {
    return builder.Services.GetLucSetupState();
  }  

  internal static LucWebSetupState GetLucSetupState(this IServiceCollection services)
  {
      var singletonDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(LucWebSetupState));
      if (singletonDescriptor == null)
      {
          services.AddSingleton<LucWebSetupState>(new LucWebSetupState());
          singletonDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(LucWebSetupState))!;
      }

      var singletonInstance = singletonDescriptor.ImplementationInstance!;
      return (singletonInstance as LucWebSetupState)!;
  }

  internal static LucWebSetupState GetLucSetupState(this WebApplication app)
  {
      return app.Services.GetRequiredService<LucWebSetupState>();
  }
}