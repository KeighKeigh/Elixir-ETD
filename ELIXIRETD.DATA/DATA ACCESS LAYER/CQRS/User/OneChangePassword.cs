using ELIXIRETD.DATA.DATA_ACCESS_LAYER.ErrorSetup.User;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.CQRS.User
{
    public class OneChangePassword
    {
        public class OneChangePasswordResult
        {
            public int Id { get; set; }
            //public bool? Is_PasswordChanged { get; set; }
        }

        public class OneChangePasswordCommand : IRequest<Result>
        {
            public string EmpId { get; set; }

            [Required]
            public string old_password { get; set; }
            [Required]
            public string password { get; set; }

        }

        public class Handler : IRequestHandler<OneChangePasswordCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {
                _context = context;
            }
            public async Task<Result> Handle(OneChangePasswordCommand command, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == command.EmpId, cancellationToken);

                if (user == null)
                {
                    return Result.Failure(UserError.UserNotExist());
                }


                if (user.Password != command.old_password)
                {
                    return Result.Failure(UserError.UserOldPasswordInCorrect());

                }

                if (command.password == user.UserName)
                {
                    return Result.Failure(UserError.InvalidDefaultPassword());
                }

                if (command.password == command.old_password)
                {
                    return Result.Failure(UserError.UserPasswordShouldChange());
                }

                user.Password = command.password;
                //user.IsPasswordChange = true;

                await _context.SaveChangesAsync(cancellationToken);

                var result = new OneChangePasswordResult
                {
                    Id = user.Id,
                    //Is_PasswordChanged = user.IsPasswordChange

                };

                return Result.Success(result);

            }
        }
    }
}
