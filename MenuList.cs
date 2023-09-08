using System.Text;

namespace Brag
{
    public class MenuList
    {
        private MenuListOptionCollection menuOptions = new MenuListOptionCollection();
        public bool AutoLabel { get => menuOptions.AutoLabel; set => menuOptions.AutoLabel = value; }
        //public void Add(string text) => menuOptions.Add(text);
        public void Add(string text, string label = "", string returnValue = "") => menuOptions.Add(text, label, returnValue);
        public MenuListOption GetByText(string text) => menuOptions.GetByText(text);
        public MenuListOption GetByLabel(string label) => menuOptions.GetByLabel(label);
        public void Clear() => menuOptions.Clear();

        const int pageSize = 20;

        public string Pick()
        {
            MenuListOption selection = null;

            int numPages = menuOptions.Count / pageSize;
            if (numPages * pageSize < menuOptions.Count)
            {
                numPages++;
            }
            int pageNumber = 1;

            bool exit = false;
            do
            {
                List<MenuListOption> pageOptions = new();

                int counter = 0;
                int startAt = (pageNumber - 1) * pageSize;
                int endAt = pageNumber * pageSize;
                foreach (MenuListOption item in menuOptions)
                {
                    if (counter >= startAt && counter < endAt)
                    {
                        pageOptions.Add(item);
                    }

                    counter++;
                }

                Show(pageOptions);
                //string value = Console.ReadLine();
                string value = Console2.ReadLine();

                switch (value)
                {
                    case "PG_DN":
                        if (pageNumber < numPages)
                            pageNumber++;
                        break;
                    case "PG_UP":
                        if (pageNumber > 1)
                            pageNumber--;
                        break;
                    case Console2.Escape:
                        exit = true;
                        break;
                    default:
                        // this allows ANY item to be selected
                        //selection = menuOptions.GetByLabel(value);

                        // this requires that the option be on the current page
                        selection = GetByLabelOnPage(value, pageOptions);
                        if (selection == null)
                            Console.Beep();
                        break;
                }

            } while (!exit && selection == null);

            return selection == null ? "quit" : selection.ReturnValue;
        }

        public MenuListOption GetByLabelOnPage(string label, List<MenuListOption> pageOptions)
        {
            MenuListOption rtnVal = null;

            IEnumerable<MenuListOption> query = pageOptions.Where(MLOption => MLOption.Label.Equals(label, StringComparison.OrdinalIgnoreCase));
            if (query.Count() > 0)
            {
                //rtnVal = query.FirstOrDefault();
                rtnVal = query.First();
            }

            return rtnVal;
        }


        public void Show(List<MenuListOption> pageOptions)
        {
            Console.Clear();

            List<string> strings = new List<string>();

            StringBuilder sb = new StringBuilder();

            foreach (MenuListOption item in pageOptions)
            {
                strings.Add($"{item.Label}. {item.Text}");
            }

            // for centering
            int widestString = 0;
            foreach (var item in strings)
            {
                if (item.Length > widestString)
                    widestString = item.Length;
            }

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            int linesAboveMiddle = strings.Count / 2;
            int middle = height / 2;
            int topLines = middle - linesAboveMiddle;

            int charsLeftOfMiddle = widestString / 2;
            middle = width / 2;
            int leftChars = middle - charsLeftOfMiddle;

            string leftPadding = new string(' ', leftChars);

            for (int i = 0; i < topLines; i++)
            {
                sb.AppendLine();
            }

            foreach (var item in strings)
            {
                sb.AppendLine($"{leftPadding}{item}");
            }
            string underline = new string('-', widestString);

            //sb.AppendLine($"{leftPadding}{underline}");
            //sb.AppendLine($"{leftPadding}99. Quit");

            sb.AppendLine();
            //sb.AppendLine("Type Quit to exit");
            //sb.AppendLine(CenteredString("Type quit to exit", Console.WindowWidth));
            sb.Append(CenteredString("Enter choice: ", Console.WindowWidth));

            Console.Write(sb.ToString());

            var v = Console.GetCursorPosition();
            int left = v.Left;
            int top = v.Top;

            Console.WriteLine();
            Console.WriteLine(CenteredString("Esc = Exit, Up/Down = Prev/Next Page", Console.WindowWidth));
            Console.SetCursorPosition(v.Left, v.Top);
        }

        private string CenteredString(string text, int width)
        {
            int charsLeftOfMiddle = text.Length / 2;
            int middle = width / 2;
            int leftChars = middle - charsLeftOfMiddle;

            string leftPadding = new string(' ', leftChars);

            return $"{leftPadding}{text}";
        }
    }

    public class MenuListOption
    {
        // All three are required.  Any capability to auto-set any of these belongs on the collection
        public MenuListOption(string label, string text, string returnValue)
        {
            Label = label;
            Text = text;
            ReturnValue = returnValue;
        }

        public string Label { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string ReturnValue { get; set; } = string.Empty;
    }

    public class MenuListOptionCollection : List<MenuListOption>
    {
        public bool AutoLabel = true;
        // set the label by going through list checking for lowest available option

        public void Add(string text, string label, string returnValue)
        {
            string realLabel = label;
            string realReturnValue = returnValue;
            if (string.IsNullOrEmpty(label))
            {
                if (!AutoLabel)
                    throw new Exception("Label must be included when AutoLabel is false");

                realLabel = GetLabel();
            }
            else
            {
                MenuListOption mlo = GetByLabel(label);
                if (mlo != null)
                    throw new Exception("Label must be unique");
            }

            if (string.IsNullOrEmpty(returnValue))
            {
                realReturnValue = realLabel;
            }

            Add(new MenuListOption(realLabel, text, realReturnValue));
        }

        public MenuListOption GetByText(string text)
        {
            MenuListOption rtnVal = null;

            IEnumerable<MenuListOption> query = this.Where(MLOption => MLOption.Text.Equals(text, StringComparison.OrdinalIgnoreCase));
            if (query.Count() > 0)
            {
                //rtnVal = query.FirstOrDefault();
                rtnVal = query.First();
            }

            return rtnVal;
        }

        public MenuListOption GetByLabel(string label)
        {
            MenuListOption rtnVal = null;

            IEnumerable<MenuListOption> query = this.Where(MLOption => MLOption.Label.Equals(label, StringComparison.OrdinalIgnoreCase));
            if (query.Count() > 0)
            {
                //rtnVal = query.FirstOrDefault();
                rtnVal = query.First();
            }

            return rtnVal;
        }

        private string GetLabel()
        {
            // start with 1
            int counter = 1;
            while (GetByLabel(counter.ToString()) != null)
            {
                counter++;
            }

            return counter.ToString();
        }

    }

}
