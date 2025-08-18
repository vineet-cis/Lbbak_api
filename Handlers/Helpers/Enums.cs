using System.ComponentModel;

namespace Handlers.Helpers
{
    public enum ResponseCode
    {
        [Description("SUCCESS")]
        Success,
        [Description("EMPTY_OUTPUT")]
        Empty,
        [Description("INVALID_INPUT")]
        InvalidInput,
        [Description("SERVICE_UNAVAILABLE")]
        ServiceUnavailable,
        [Description("UNAUTHORIZED")]
        Unauthorized,
        [Description("INTERNAL_SERVER_ERROR")]
        InternalServerError,
        [Description("NOT_FOUND")]
        NotFound,
        [Description("BAD_REQUEST")]
        BadRequest,
        [Description("Failed")]
        Failed,
    }
}
