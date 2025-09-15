using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;


namespace Abcmoney_Transfer.Controllers
{
    public static class ContextResolver
    {
        private static IHttpContextAccessor _contextAccessor;

        public static Microsoft.AspNetCore.Http.HttpContext Context => _contextAccessor.HttpContext;

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


        public static string GetSessionId(this IIdentity identity)
        {
            try
            {
                var _context = ContextResolver.Context;
                if (_context.User.Identity.IsAuthenticated)
                {

                    IEnumerable<Claim> claims = ((ClaimsIdentity)identity).Claims;
                    foreach (var claim in claims)
                    {
                        if (claim.Type == ApplicationClaim.SessionCode)
                            return claim.Value;
                    }

                }
                else
                {
                    var cookie = "";
                    _context.Request.Cookies.TryGetValue(ApplicationClaim.SessionCode, out cookie);
                    if (cookie != null)
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
                return "";
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static int GetIdentityUserId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IdUid");
            // Test for null to avoid issues during local testing
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }
        public static string GetUserName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(JwtClaimTypes.Name);
            return (claim != null) ? claim.Value : "";
        }
        public static IEnumerable<string> GetRoles(this IIdentity identity)
        {
            var roles = ((ClaimsIdentity)identity).Claims
                .Where(c => c.Type == JwtClaimTypes.Role || c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return roles;
        }
        public static string GetRoleIds(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("RoleIds");
            return (claim != null) ? claim.Value : "-1";
        }
        public static string GetFullName(this IIdentity identity)
        {
            var fn = ((ClaimsIdentity)identity).FindFirst("fn");

            return (fn != null) ? fn.Value : "";
        }
        public static string GetPicture(this IIdentity identity)
        {
            var picture = ((ClaimsIdentity)identity).FindFirst("picture");

            return picture == null ? "" : picture.Value;
        }

        public static long GetAppUserUserId(this IIdentity identity)
        {
            //TODO: AppUser ID
            var claim = ((ClaimsIdentity)identity).FindFirst("appuserid");
            // Test for null to avoid issues during local testing
            return (claim != null) ? Convert.ToInt64(claim.Value) : 0;
        }

    }

    public class ResponseModel
    {
        private int v;
        private string error;
        public ResponseModel()
        {
        }
        public ResponseModel(int v, string error)
        {
            this.v = v;
            this.error = error;
        }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public ResponseModel(int code, string msg, object data)
        {
            Code = code;
            Message = msg;
            Data = data;
        }

    }


    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiAuthorize]
    public class BaseApiController : ControllerBase
    {
        private string _sessionCode;
        protected string SessionCode
        {
            get { return User.Identity.GetSessionId(); }
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
        protected object SuccessResponse(string msg, object data)
        {
            return new
            {
                Code = 200,
                Message = msg,
                Data = data
            };
        }
        protected object ErrorResponse(ModelStateDictionary modelState, int code, object data)
        {
            return new
            {
                Code = code,
                Message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)),
                Data = data
            };
        }
        protected ResponseModel ExceptionResponse(Exception ex, object data)
        {
            return new ResponseModel(500, ex.Message, data);

        }
        protected string GetModelErrors(ModelStateDictionary modelState)
        {
            return string.Join("; ", modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }
    }
}
