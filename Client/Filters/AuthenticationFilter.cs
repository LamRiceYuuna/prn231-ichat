using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Client.Filters {
    public class AuthenticationFilter : IActionFilter {
        public void OnActionExecuting(ActionExecutingContext context) {
            var token = context.HttpContext.Request.Cookies["bearer_token"];
            var currentPath = context.HttpContext.Request.Path.ToString();

            if (string.IsNullOrEmpty(token) && currentPath.Contains("Chat", StringComparison.OrdinalIgnoreCase) 
                && !currentPath.Equals("/User/Login", StringComparison.OrdinalIgnoreCase)) {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) {
            
        }
    }
}
