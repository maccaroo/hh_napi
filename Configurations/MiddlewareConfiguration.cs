using Serilog;
using hh_napi.Middleware;

namespace hh_napi.Configurations;

public static class MiddlewareConfiguration
{
    /// <summary>
    /// Configures the middleware pipeline in the correct order.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <remarks>
    /// The order of the middleware components is important. The order used here is:
    /// <list type="number">
    ///     <item>Exception handling</item>
    ///     <item>Logging</item>
    ///     <item>HTTPS redirection</item>
    ///     <item>Routing</item>
    ///     <item>Authentication</item>
    ///     <item>Authorization</item>
    ///     <item>OpenAPI (if in development)</item>
    /// </list>
    /// See the individual middleware components for more information.
    /// </remarks>
    public static void ConfigureMiddleware(this WebApplication app)
    {
        // Exception handing
        app.UseMiddleware<ExceptionHandlingMiddleware>();   // Catches exceptions before anything else runs

        // Logging
        app.UseSerilogRequestLogging(); // Logs requests at the start for tracking.

        // CORS
        app.UseCors("AllowAngularApp");

        // Redirect to HTTPS
        app.UseHttpsRedirection();  // Ensures all requests are redirected to HTTPS before anything else
        
        // Routing
        app.UseRouting();   // Must come before authentication and authorization for correct route matching

        // Authn and authz
        app.UseAuthentication();    // Ensures the request has a valid token before accessing secured routes
        app.UseAuthorization();     // Ensures the authenticated user has the right permissions

        // OpenAPI
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }
}