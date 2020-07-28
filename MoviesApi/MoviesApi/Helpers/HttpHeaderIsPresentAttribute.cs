using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace MoviesApi.Helpers
{
    public class HttpHeaderIsPresentAttribute : Attribute, IActionConstraint
    {
        public int Order => 0;
        private readonly string _header;
        private readonly string _value;

        public HttpHeaderIsPresentAttribute(string header, string value)
        {
            _header = header;
            _value = value;
        }

        public bool Accept(ActionConstraintContext context)
        {
            var headers = context.RouteContext.HttpContext.Request.Headers;

            if (!headers.ContainsKey(_header))
            {
                return false;
            }

            return string.Equals(headers[_header], _value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
