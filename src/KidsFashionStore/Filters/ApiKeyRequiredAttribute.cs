using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace KidsFashionStore.Filters
{
    /// <summary>
    /// Bảo vệ các API ghi dữ liệu (POST/PUT/DELETE). Web API không dùng Session của MVC nên dùng header X-Api-Key.
    /// - Web.config KHÔNG có appSettings "ApiWriteKey" (hoặc rỗng): chế độ demo, cho phép tất cả (test Postman nhanh).
    /// - Có cấu hình: request phải gửi header "X-Api-Key", sai/thiếu -> 401.
    /// </summary>
    public class ApiKeyRequiredAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var configured = ConfigurationManager.AppSettings["ApiWriteKey"];
            if (string.IsNullOrEmpty(configured)) return;

            IEnumerable<string> values;
            if (actionContext.Request.Headers.TryGetValues("X-Api-Key", out values)
                && values.FirstOrDefault() == configured)
            {
                return;
            }

            actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, "Thiếu hoặc sai header X-Api-Key.");
        }
    }
}
