using Auth.Core.Data;
using Auth.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands.ChangeActive;
using UserService.Application.Commands.ChangeEmail;
using UserService.Application.Commands.ChangePassword;
using UserService.Application.Commands.ChangeProfile;
using UserService.Application.Commands.ConfirmNewEmailCode;
using UserService.Application.Commands.ConfirmNewEmailLink;
using UserService.Application.Commands.ConfirmNewPassword;
using UserService.Application.Commands.ConfirmRegisterCode;
using UserService.Application.Commands.ConfirmRegisterLink;
using UserService.Application.Commands.CreateUser;
using UserService.Application.Commands.Delete;
using UserService.Application.Commands.DeleteById;
using UserService.Application.Commands.Login;
using UserService.Application.Commands.ResetPassword;
using UserService.Application.Queries.GetAllUsers;
using UserService.Application.Queries.GetCurrentUser;
using UserService.Application.Queries.GetSellers;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Accepted(); 
        }

        [HttpPost("confirm-registration-code")]
        public async Task<IActionResult> ConfirmRegistrationByCode([FromBody] ConfirmRegisterCodeCommand command, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(command, cancellationToken);
            Response.Cookies.Append("myCookie", token);
            return Ok(token);
        }

        [HttpGet("confirm-registration-link")]
        public async Task<IActionResult> ConfirmRegistrationByLink([FromQuery] string token, CancellationToken cancellationToken)
        {
            var jwtToken = await _mediator.Send(new ConfirmRegisterLinkCommand(token), cancellationToken);
            Response.Cookies.Append("myCookie", jwtToken);
            return Ok(jwtToken);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(command, cancellationToken);
            Response.Cookies.Append("myCookie", token);
            return Ok(token);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost("confirm-newPassword")]
        public async Task<IActionResult> ConfirmNewPassword([FromBody] ConfirmNewPasswordCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpPost("confirm-newEmail-code")]
        public async Task<IActionResult> ConfirmNewEmailByCode([FromBody] ConfirmNewEmailCodeCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpGet("confirm-newEmail-link")]
        public async Task<IActionResult> ConfirmNewEmailByLink([FromQuery] string token, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ConfirmNewEmailLinkCommand(token), cancellationToken   );
            return Ok("Email successfully confirmed");
        }


        [HasPermission(Permission.Update)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpPost("change-profile")]
        public async Task<IActionResult> ChangeProfile([FromBody] ChangeProfileCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpPost("change-active")]
        public async Task<IActionResult> ChangeActive([FromBody] ChangeActiveCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteCommand(), cancellationToken);
            Response.Cookies.Delete("myCookie");
            return Ok();
        }

        [HasPermission(Permission.Delete)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteById(Guid userId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteByIdCommand { Id = userId }, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Read)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) 
        {
            var users = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
            return Ok(users);
        }

        [HasPermission(Permission.Read)]
        [HttpGet("active-sellers")]
        public async Task<IActionResult> GetActiveSellers([FromQuery] GetSellersQuery query, CancellationToken cancellationToken)
        {
            var users = await _mediator.Send(query, cancellationToken);
            return Ok(users);
        }

        [HasPermission(Permission.Read)]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            return Ok(user);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("myCookie");
            return Ok();
        }

    }
}
