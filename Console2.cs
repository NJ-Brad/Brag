namespace Brag
{
    public static class Console2
    {
        public const string Escape = "_ESCAPE_";

        public static string ReadLine()
        {
            string value = "";

            //Console.SetCursorPosition(int, int)
            var v = Console.GetCursorPosition();
            int left = v.Left;
            int top = v.Top;

            int cursorPosition = 0;

            bool exit = false;
            do
            {

                // There is something "not right" after hitting escape on a secondary menu, then re-showing the main menu.
                // Using the intercept option fixes the problem
                // https://stackoverflow.com/questions/20018716/c-sharp-clearing-key-buffer
                ConsoleKeyInfo ki = Console.ReadKey(true);

                switch (ki.Key)
                {
                    case ConsoleKey.Enter:
                        exit = true;
                        break;
                    case ConsoleKey.Backspace:
                        //int newLeft = Console.GetCursorPosition().Left;
                        if (value.Length > 0)
                        {
                            Console.Write(ki.KeyChar);
                            Console.Write(" ");
                            Console.Write("\b");
                            value = value.Substring(0, value.Length - 1);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        value = "PG_UP";
                        break;
                    case ConsoleKey.DownArrow:
                        value = "PG_DN";
                        exit = true;
                        break;
                    case ConsoleKey.Escape:
                        value = Escape;
                        exit = true;
                        break;
                    default:
                        value = value + ki.KeyChar;
                        Console.Write(ki.KeyChar);
                        break;
                }

            } while (!exit);

            //Console.Write(value);

            return value;
        }
    }
}
