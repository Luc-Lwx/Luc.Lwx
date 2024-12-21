namespace Luc.Lwx.LwxSetupState;

public static class LwxSetupStateExtension
{
  internal static LwxSetupState LwxGetSetupState(this WebApplicationBuilder builder)
  {
    return builder.Services.LwxGetSetupState();
  }  

  internal static LwxSetupState LwxGetSetupState(this IServiceCollection services)
  {
      var singletonDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(LwxSetupState));
      if (singletonDescriptor == null)
      {
          services.AddSingleton<LwxSetupState>(new LwxSetupState());
          singletonDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(LwxSetupState))!;
      }

      var singletonInstance = singletonDescriptor.ImplementationInstance!;
      return (singletonInstance as LwxSetupState)!;
  }

  internal static LwxSetupState LwxGetSetupState(this WebApplication app)
  {
      return app.Services.GetRequiredService<LwxSetupState>();
  }
}