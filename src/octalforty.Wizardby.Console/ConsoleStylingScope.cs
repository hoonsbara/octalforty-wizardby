using System;

namespace octalforty.Wizardby.Console
{
    internal class ConsoleStylingScope : IDisposable
    {
        private ConsoleColor? oldForegroundColor;

        public ConsoleStylingScope(ConsoleColor? foregroundColor)
        {
            if(foregroundColor.HasValue)
            {
                oldForegroundColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = foregroundColor.Value;
            } // if
        }

        void IDisposable.Dispose()
        {
            if(oldForegroundColor.HasValue)
                System.Console.ForegroundColor = oldForegroundColor.Value;
        }
    }
}
