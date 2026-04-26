using ELIXIRETD.DATA.DATA_ACCESS_LAYER.ErrorSetup.User;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.CQRS.User
{
    public class OneResetPassword
    {
        public class OneResetPasswordResult
        {
            public int Id { get; set; }
            //public bool? IsPasswordChange { get; set; }
        }

        public class OneResetPasswordCommand : IRequest<Result>
        {

            public string EmpId { get; set; } = string.Empty;

        }

        public class Handler : IRequestHandler<OneResetPasswordCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(OneResetPasswordCommand command, CancellationToken cancellationToken)
            {

                var User = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == command.EmpId, cancellationToken);

                if (User == null)
                {
                    return Result.Failure(UserError.UserNotExist());

                }

                User.Password = User.UserName;
                //User.IsPasswordChange = null;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new OneResetPasswordResult
                {
                    Id = User.Id,
                    //IsPasswordChange = User.IsPasswordChange
                };

                return Result.Success(results);

            }




        }
    }
}
