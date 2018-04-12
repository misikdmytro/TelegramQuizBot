﻿using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Predicates.Contracts;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Commands.Predicates
{
    public class DenyStatsPredicate : ICommandPredicate
    {
        private readonly IDbRepository<User> _usersRepository;

        public DenyStatsPredicate(IDbRepository<User> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<bool> IsThisCommand(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .FirstOrDefault();

            return update.Message.Text.Equals("/stats") && user?.UserStatus != UserStatus.Answered;
        }
    }
}