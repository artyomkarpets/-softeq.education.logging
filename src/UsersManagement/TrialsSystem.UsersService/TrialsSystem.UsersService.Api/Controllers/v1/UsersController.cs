using Microsoft.AspNetCore.Mvc;
using TrialsSystem.UsersService.Infrastructure.Models.UserDTOs;
using MediatR;
using TrialsSystem.UsersService.Api.Application.Commands;
using TrialsSystem.UsersService.Api.Application.Commands.UserCommands;
using TrialsSystem.UsersService.Api.Application.Queries.UserQueries;
using TrialsSystem.UsersService.Api.Filters;
using TrialSystem.Shared.UsersService.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using TrialsSystem.UsersService.Infrastructure.Helpers;

namespace TrialsSystem.UsersService.Api.Controllers.v1
{
    /// <summary>
    /// User management controller
    /// </summary>
    [Route("api/v1/{userId}/[controller]")]
    [ServiceFilter(typeof(UserExceptionFilter))]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUrlHelperFactory _factory;
        private readonly ILogger<UsersController> _logger;
        private readonly IIdentityHelper _identityHelper;

        public UsersController(IMediator mediator,
                               IUrlHelperFactory factory,
                               ILogger<UsersController> logger,
                               IIdentityHelper identityHelper)
        {
            _mediator = mediator;
            _factory = factory;
            _logger = logger;
            _identityHelper = identityHelper;

        }

        /// <summary>
        /// Get all users by setting parameters and filters
        /// </summary>
        /// <param name="userId">authorized user Id</param>
        /// <param name="skip">skip items (pagination parameters)</param>
        /// <param name="take">take items (pagination parameters)</param>
        /// <param name="email">part of sn (filter)</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetUsersResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetAsync(
            [FromRoute] string userId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] string? email = null,
            [FromQuery] string? name = null,
            [FromQuery] string? surname = null)
        {

            var roles = _identityHelper.Roles;

            _logger.LogInformation($"Get users by userId:{userId} with roles: {roles}");

            var response = await _mediator.Send(
                new UsersQuery(take, skip, email, name, surname));
            return Ok(response);
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="userId">authorized user Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync(
            [FromRoute] string userId,
            [FromRoute] string id)
        {
            var response = await _mediator.Send(new UserQuery(userId, id));
            return Ok(response);
        }

        /// <summary>
        /// CreateAsync new  user
        /// </summary>
        /// <param name="request">CreateAsync user request model</param>
        /// <returns></returns>
        [HttpPost("/api/v1/[controller]")]
        [ProducesResponseType(typeof(UpdateUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync(CreateUserRequest request)
        {
            var id = await _mediator.Send(new
                CreateUserCommand(
                request.Email,
                request.Name,
                request.Surname,
                request.CityId,
                request.BirthDate,
                request.GenderId,
                request.IdentityId));

            var helper = _factory.GetUrlHelper(ControllerContext);
            var uri = helper.Action("Get", new { userId = id, id });
            return Created(uri, null);

        }

        /// <summary>
        /// Update user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UpdateUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAsync(string id,
            [FromRoute] string userId, UpdateUserRequest request)
        {
            var response = await _mediator.Send(new
                UpdateUserCommand(id,
                request.Name,
                request.Surname,
                request.CityId,
                request.BirthDate,
                request.Weight,
                request.Height,
                request.GenderId,
                userId));

            return Ok(response);
        }

        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return Ok();
        }
    }
}
