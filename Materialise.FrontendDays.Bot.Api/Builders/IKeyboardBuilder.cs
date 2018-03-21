using Telegram.Bot.Types.ReplyMarkups;

namespace Materialise.FrontendDays.Bot.Api.Builders
{
    public interface IKeyboardBuilder
    {
        IKeyboardBuilder AddButtonsRow(params string[] buttons);
        ReplyKeyboardMarkup Build();
    }
}