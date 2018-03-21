﻿using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;

namespace Materialise.FrontendDays.Bot.Api.Repositories
{
    public interface IUserAnswerRepository : IDbRepository<UserAnswer>
    {
        Task<UserAnswer[]> GetCorrectAnswers();
    }
}