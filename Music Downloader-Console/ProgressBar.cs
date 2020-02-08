using System;

namespace Music_Downloader_Console
{
    public class CustomeProgressBar
    {
        private int Left;
        private int Top;
        private int Length;
        private int Value;

        public CustomeProgressBar()
        {
            Left = Console.CursorLeft;
            Top = Console.CursorTop;
            Length = 50;
            DrawProgressBar();
        }

        public CustomeProgressBar(int left, int top, int length = 50)
        {
            Left = left;
            Top = top;
            Length = length;
            DrawProgressBar();
        }

        public void DrawProgressBar()
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write("[");
            for (int i = 0; i < Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write("]");
        }

        public void Display(int value)
        {
            if (Value != value && value <= Length)
            {
                Value = value;
                Console.SetCursorPosition(Left + 1, Top);
                for (int i = 0; i < Value; i++)
                {
                    Console.Write("=");
                }
            }
        }
    }
}
