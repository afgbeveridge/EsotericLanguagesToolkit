using System;
using System.Collections.Generic;

namespace Interpreter.Abstractions {
        public static class ConsoleHighlighter {
                private static readonly Stack<ConsoleColor> StackedColors = new Stack<ConsoleColor>();

                public static void SetColor(ConsoleColor textColor, ConsoleColor? backColor = default(ConsoleColor)) {
                        StackedColors.Push(Console.ForegroundColor);
                        StackedColors.Push(Console.BackgroundColor);
                        Console.ForegroundColor = textColor;
                        Console.BackgroundColor = backColor ?? Console.BackgroundColor;
                }

                public static void RestorePreviousColor() {
                        Console.BackgroundColor = StackedColors.Pop();
                        Console.ForegroundColor = StackedColors.Pop();
                }

                public static void Display(string message, ConsoleColor textColor = ConsoleColor.Red,
                        ConsoleColor backColor = ConsoleColor.Black, bool includeNewLine = true) {
                        SetColor(textColor, backColor);
                        if (includeNewLine)
                                Console.WriteLine(message);
                        else
                                Console.Write(message);

                        RestorePreviousColor();
                }
        }
}