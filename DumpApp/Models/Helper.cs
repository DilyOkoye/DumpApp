using System.Web;
using System.Web.Mvc;

namespace DumpApp.Models
{
    public class Helper
    {
        public class SessionExpireAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                // check  sessions here
                if (HttpContext.Current.Session["guidNo"] == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Authentication",
                        action = "Login"
                    }));
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}