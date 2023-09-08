namespace Brag
{
    public class Scroller
    {
        public static void Display(string textToDisplay)
        {
            int lines = Console.WindowHeight - 1;
            int firstLine = 0;

            string localValue = textToDisplay + "\r\n" + "--End of File--";

            string[] textLines = localValue.Split('\n');

            bool exit = false;
            do
            {
                Console.Clear();
                for (int i = firstLine; i < firstLine + lines && i < textLines.Length; i++)
                {
                    Console.WriteLine(textLines[i]);
                }

                ConsoleKeyInfo ki = Console.ReadKey(true);

                switch (ki.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (firstLine > 0)
                            firstLine--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (firstLine < textLines.Length - lines)
                            firstLine++;
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    default:
                        break;
                }

            } while (!exit);
        }
    }
}
