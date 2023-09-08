using System.Text;

namespace Brag
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            MenuList ml = new MenuList();
            ml.Add("Add", returnValue: "add");
            ml.Add("View", returnValue: "view2");
            //ml.Add("View File", returnValue: "view2");
            //            ml.Add("Delete", returnValue: "delete");

            bool quit = false;
            do
            {
                string selection = ml.Pick();
                switch (selection)
                {
                    case "add":
                        await AddNote();
                        break;
                    //case "view":
                    //    await ViewFile();
                    //    break;
                    case "view2":
                        await View2();
                        break;
                    //case "delete":
                    //    Console.WriteLine("Delete was chosen");
                    //    Console.Read();
                    //    break;
                    case "quit":
                        quit = true;
                        break;
                }
            } while (!quit);

            Console.Clear();
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReviewNotes");

            Console.WriteLine($"Files are stored in this folder {folder}");

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            return 0;
        }

        internal static async Task AddNote()
        {
            Console.Clear();
            Console.WriteLine("Type your comment.  End by putting a . by itself on the last line and hitting enter.");
            string lastLine = "";
            StringBuilder fullString = new();
            fullString.AppendLine(DateTime.Now.ToString("g"));
            fullString.AppendLine(new string('-', 80));
            do
            {
                lastLine = Console.ReadLine();
                if (lastLine != ".")
                {
                    fullString.AppendLine(lastLine);
                }
            } while (lastLine != ".");
            //            Console.WriteLine(fullString.ToString());

            string fileName = WriteFile(fullString.ToString());

            Console.WriteLine(fileName);
        }

        internal static async Task ViewFile()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReviewNotes");

            string[] fileNames = Directory.GetFiles(folder);

            Array.Sort(fileNames);

            MenuList ml = new MenuList();

            foreach (string fName in fileNames)
            {
                ml.Add(Path.GetFileNameWithoutExtension(fName), returnValue: fName);
            }

            bool quit = false;
            do
            {
                string selection = ml.Pick();
                {
                    switch (selection)
                    {
                        case "quit":
                            quit = true;
                            break;
                        default:
                            Console.Clear();
                            string noteText = File.ReadAllText(selection);
                            Console.WriteLine(noteText);
                            Console.WriteLine("Press any key to continue");
                            Console.Read();
                            break;
                    }
                }
            } while (!quit);
        }

        internal static async Task View2()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReviewNotes");

            string[] fileNames = Directory.GetFiles(folder);

            Array.Sort(fileNames);

            StringBuilder sb = new StringBuilder();

            foreach (string fName in fileNames)
            {
                if (sb.Length > 0)
                    sb.AppendLine(new string('*', 80));

                FileInfo fi = new FileInfo(fName);
                string fileTitle = $"{Path.GetFileNameWithoutExtension(fName)} - {fi.CreationTime.ToString()}";
                sb.AppendLine(fileTitle);

                sb.AppendLine(new string('-', fileTitle.Length));
                //Console.WriteLine(new string('-', Console.WindowWidth));

                string noteText = File.ReadAllText(fName);

                sb.AppendLine(noteText);
            }
            Scroller.Display(sb.ToString());
        }


        // this uses the date and a sequence at the end.  If I write more than 99 items in a single day, something is really weird
        private static string WriteFile(string fileText)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReviewNotes");
            Directory.CreateDirectory(folder);

            string fileName = Path.ChangeExtension(Path.Combine(folder, DateTime.Now.ToString("yyyyMMdd") + "_01"), "txt");

            // this will trigger the exception = useful for testing
            //File.WriteAllText(fileName, fileText);

            // https://stackoverflow.com/questions/12389599/exception-codes-or-detecting-a-file-already-exists-type-exception
            const int WARN_WIN32_FILE_EXISTS = unchecked((int)0x80070050);

            bool fileWritten = false;

            int counter = 2;
            do
            {
                try
                {
                    using (var stream = new FileStream(fileName, FileMode.CreateNew))   // this will trigger the exception, if a file by this name already exists
                    using (var writer = new StreamWriter(stream))
                    {
                        //write file
                        writer.WriteAsync(fileText);
                        fileWritten = true;
                    }
                }
                //            catch (IOException e)
                catch (IOException e) when (e.HResult == WARN_WIN32_FILE_EXISTS)
                {
                    //var exists = File.Exists(@"C:\Text.text"); // =)
                    // e.HResult == -2147024816

                    if (e.HResult == -2147024816 || e.HResult == -2147024713)
                    {
                        // File already exists.
                        // try a new name
                        fileName = Path.ChangeExtension(Path.Combine(folder, DateTime.Now.ToString("yyyyMMdd") + $"_{counter.ToString("00")}"), "txt");
                        counter++;
                    }
                    //                var exists = File.Exists(@"Test"); // =)
                }
            } while (!fileWritten);

            return fileName;
        }
    }
}