﻿using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands.Predicates.Contracts
{
    public interface ICommandPredicate
    {
        bool IsDefault { get; }
        Task<bool> IsThisCommand(Update update);
    }
}