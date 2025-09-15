using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using IIdentity model;
namespace Abcmoney_Transfer.Controllers
{
    public static class ContextResolver
    {
        private static IHttpContextAccessor _contextAccessor;
        public static HttpContext Context => _contextAccessor?.HttpContext; // Added null check
        internal static void Configure(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
    }
    public class ApplicationClaim
    {
        public static readonly string CustomerId = "_customerId";
        public static readonly string SessionCode = "_sessionCode";
        public static readonly string Anonymous = "__anonymous";
        public static readonly string OnlineId = "_onlineId";
    }
    public static class IdentityExtensions
    {
       // REMOVE this line: private static object JwtClaimTypes;
        public static string GetSessionId(this IIdentity identity)
        {
            try
            {
                var _context = ContextResolver.Context;
                if (_context == null) return string.Empty;

                if (_context.User.Identity != null && _context.User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = identity as ClaimsIdentity;
                    if (claimsIdentity == null) return string.Empty;

                    var sessionClaim = claimsIdentity.Claims
                        .FirstOrDefault(c => c.Type == ApplicationClaim.SessionCode);

                    return sessionClaim?.Value ?? string.Empty;
                }
                else
                {
                    if (_context.Request.Cookies.TryGetValue(ApplicationClaim.SessionCode, out var cookie))
                    {
                        return cookie;
                    }
                    else
                    {
                        var guid = Guid.NewGuid().ToString();
                        CookieOptions option = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddDays(1)
                        };
                        _context.Response.Cookies.Append(ApplicationClaim.SessionCode, guid, option);
                        return guid;
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider logging instead of throwing
                return string.Empty;
            }
        }
        public static int GetIdentityUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst("IdUid");
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }
        public static string GetUserName(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            // Use the actual JwtClaimTypes.Name from IdentityModel namespace
            var claim = claimsIdentity?.FindFirst(IIdentity.JwtClaimTypes.Name);
            return (claim != null) ? claim.Value : "";
        }
        public static IEnumerable<string> GetRoles(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null) return Enumerable.Empty<string>();

            // Use the actual JwtClaimTypes.Role from IdentityModel namespace
            var roles = claimsIdentity.Claims
                .Where(c => c.Type == .JwtClaimTypes.Role || c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return roles;
        }
        public static string GetRoleIds(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst("RoleIds");
            return (claim != null) ? claim.Value : "-1";
        }
        public static string GetFullName(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var fn = claimsIdentity?.FindFirst("fn");
            return (fn != null) ? fn.Value : "";
        }
        public static string GetPicture(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var picture = claimsIdentity?.FindFirst("picture");
            return picture == null ? "" : picture.Value;
        }
        public static long GetAppUserUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst("appuserid");
            return (claim != null) ? Convert.ToInt64(claim.Value) : 0;
        }
    }
    public class ResponseModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public ResponseModel()
        {
        }
        public ResponseModel(int code, string msg)
        {
            Code = code;
            Message = msg;
        }
        public ResponseModel(int code, string msg, object data)
        {
            Code = code;
            Message = msg;
            Data = data;
        }
    }
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseApiController : ControllerBase
    {
        private string _sessionCode;
        protected string SessionCode
        {
            get { return User?.Identity?.GetSessionId() ?? string.Empty; } // Added null check
            set { _sessionCode = value; }
        }
        protected BaseApiController()
        {
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object HttpResponse(int statusCode, string msg, object data)
        {
            return new
            {
                Code = statusCode,
                Message = msg,
                Data = data
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object HttpResponse(int statusCode, string msg)
        {
            return new
            {
                Code = statusCode,
                Message = msg,
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object ValidationResponse(List<string> errors)
        {
            return new
            {
                Code = 600,
                Message = "Validation Error",
                Errors = errors ?? new List<string>()
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object NotAuthorizedResponse()
        {
            return new
            {
                Code = 401,
                Message = "Unauthorized Request"
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object ErrorResponse(int statusCode, string msg)
        {
            return new
            {
                Code = statusCode,
                Message = msg,
                Errors = new string[] { msg }
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object ErrorResponse(string[] msgs)
        {
            return new
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Error",
                Errors = msgs
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object HttpResponse(int statusCode, string msg, object data, int currentPage = 1)
        {
            return new
            {
                Code = statusCode,
                Message = msg,
                Data = data,
                CurrentPage = currentPage,
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object SuccessResponse(string msg, object data)
        {
            return new
            {
                Code = 200,
                Message = msg,
                Data = data
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected object ErrorResponse(ModelStateDictionary modelState, int code, object data)
        {
            return new
            {
                Code = code,
                Message = string.Join("; ", modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)),
                Data = data
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected ResponseModel ExceptionResponse(Exception ex, object data)
        {
            return new ResponseModel(500, ex.Message, data);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected string GetModelErrors(ModelStateDictionary modelState)
        {
           return string.Join("; ", modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }
    }
}