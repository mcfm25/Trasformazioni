using Hangfire.Dashboard;

namespace Trasformazioni.Authorization
{
    /// <summary>
    /// Filtro di autorizzazione per la dashboard Hangfire
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Permetti accesso solo agli utenti autenticati con ruolo Admin
            return httpContext.User.Identity?.IsAuthenticated == true &&
                   httpContext.User.IsInRole(RoleNames.Amministrazione);
        }
    }
}
