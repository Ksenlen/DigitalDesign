using System.Reflection;
using System.Text;
using TextAnalysisLibrary;


class DigitalDesign
{
    static void Main()
    {
        // Получение метода через рефлексию
        Type type = typeof(TextAnalysisLibrary.TextAnalysis);
        var methodInfo = type.GetMethod("GetTextAnalysis", BindingFlags.NonPublic | BindingFlags.Instance);


        // Чтение текстового файла
        Console.Write("Input file path: ");
        string? inputFilename = Console.ReadLine();
        if (!File.Exists(inputFilename))
        {
            Console.WriteLine("File doesn't exist");
            return;
        }
        string text = File.ReadAllText(inputFilename, Encoding.UTF8);

        if (!String.IsNullOrEmpty(text))
        {
            // Вызов приватного метода из dll, получение результата и сортировка полученного результата
            var wordCounts = methodInfo!.Invoke(new TextAnalysis(), new object[] { text }) as Dictionary<string, int>;
            var sortedDict = wordCounts!.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);


            // Запись результата в файл
            string outputFilePath = Path.GetFileNameWithoutExtension(inputFilename) + "_wordcount.txt";// Путь к выходному файлу
           
            using StreamWriter writer = new(outputFilePath);
            foreach (var entry in sortedDict)
            {
                writer.WriteLine($"{entry.Key}: {entry.Value}");
            }
            Console.WriteLine("Text processing completed. The result is written");
        }
        else
        {
            Console.WriteLine("File is empty");
        }
        
        Console.ReadLine();
    }
}
