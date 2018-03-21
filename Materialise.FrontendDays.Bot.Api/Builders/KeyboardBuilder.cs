using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Materialise.FrontendDays.Bot.Api.Builders
{
    public class KeyboardBuilder : IKeyboardBuilder
    {
        private readonly IList<string[]> _buttons;

        public KeyboardBuilder()
        {
            _buttons = new List<string[]>();
        }

        public IKeyboardBuilder AddButtonsRow(params string[] buttons)
        {
            _buttons.Add(buttons);

            return this;
        }

        public ReplyKeyboardMarkup Build()
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = _buttons
                    .Select(bs => bs
                        .Select(x => new KeyboardButton(x))
                        .ToArray())
                    .ToArray()
            };

            return keyboard;
        }
    }
}
