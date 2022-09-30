using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Azure.Core;
using Microsoft.Extensions.Logging.Abstractions;
using TrialsSystem.UsersService.Domain.AggregatesModel.UserAggregate;

namespace TrialsSystem.UsersService.Infrastructure.Helpers
{
    public class IdentityHttpRequestHelper : IIdentityHelper
    {
        private const string HeaderRoles = "_roles";
        private const string RouteUserId = "userId";
        private const string HeaderClintId = "clientId";

        public IdentityHttpRequestHelper(IHttpContextAccessor accessor)
        {
            var context = accessor.HttpContext as HttpContext;

            var routeValues = context?.GetRouteData()?.Values;

            if (routeValues != null)
                _userId = routeValues.ContainsKey(RouteUserId) ?
                         routeValues[RouteUserId].ToString() :
                         null;

            var headers = context?.Request?.Headers;
            if (headers != null)
            {
                _roles = headers.ContainsKey(HeaderRoles) ?
                        headers[HeaderRoles].ToString().Split(",") :
                        Array.Empty<string>();
                _clientId = headers.ContainsKey(HeaderClintId) ?
                           headers[HeaderClintId].ToString() :
                           null;
            }

        }

        private readonly IEnumerable<string> _roles;
        private readonly string? _clientId;
        private readonly string? _userId;

        public IEnumerable<string> Roles
        {
            get => _roles;
        }

        public string? ClientId
        {
            get => _clientId;
        }

        public string? UserId
        {
            get => _userId;
        }
    }
}
