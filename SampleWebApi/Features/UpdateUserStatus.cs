using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SampleWebApi.Common;
using SampleWebApi.Data;
using System.Reflection.Metadata.Ecma335;

namespace SampleWebApi.Features
{
    [Route("api/User"), ApiController]
    public class UpdateUserStatus : ControllerBase
    {
        private readonly IMediator _mediator;

        public UpdateUserStatus(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPatch("UpdateUserStatus/{id}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id)
        {
            try
            {
                var command = new UpdateUserStatusCommand();
                command.UserId = id;

                var result = await _mediator.Send(command);

                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class UpdateUserStatusCommand : IRequest<Result<UpdateUserStatusResult>>
        {
            public int UserId { get; set; }
        }

        public class UpdateUserStatusResult
        {
            public int Id { get; set;}
            public string FullName { get; set; }
            public string Username { get; set; }

            public string Password { get; set; }
        }

        public class Handler : IRequestHandler<UpdateUserStatusCommand, Result<UpdateUserStatusResult>>
        {
            private readonly SampleWebApiDbContext _context;

            public Handler(SampleWebApiDbContext context)
            {
                _context = context;
            }

            public async Task<Result<UpdateUserStatusResult>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

                if (user is null)
                {
                    return Result<UpdateUserStatusResult>.Failure(UserErrors.NotFound());
                }
                 user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateUserStatusResult
                {
                    Id = user.Id,
                    FullName = user.Fullname,
                    Username = user.Username,
                    Password = user.Password,
                };

                return Result<UpdateUserStatusResult>.Success(result, "Usr status updated successfully");
            }
        }
    }
}
