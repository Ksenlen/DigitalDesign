using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using TextAnalysisLibrary;


class DigitalDesign
{

    public static void Main()
    {
        Stopwatch stopwatch = new();
        List<long> timeTests = new(); //Список для измерения времени работы метода
        int tests = 20; //Количество тестов для измерения времени

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
            Dictionary<string, int>? wordCounts = new();

            // Вызов приватного однопоточного метода из dll, получение результата и сортировка полученного результата
            string outputFilePath = Path.GetFileNameWithoutExtension(inputFilename) + "_wordcount.txt";// Путь к выходному файлу однопоточного
            for (int i = 0; i < tests; i++)
            {
                stopwatch.Restart();
                wordCounts = methodInfo!.Invoke(new TextAnalysis(), new object[] { text }) as Dictionary<string, int>;
                stopwatch.Stop();
                timeTests.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"Время работы однопоточного алгоритма: {timeTests.Average()}");
            var sortedDict = wordCounts!.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
            WritingToFile(sortedDict, outputFilePath);
            timeTests.Clear();

          
            // Вызов параллельного метода с использованием CurrentDict
            string outputFilePathParallel3 = Path.GetFileNameWithoutExtension(inputFilename) + "_wordcountParallelCD.txt";// Путь к выходному файлу для параллельной обработки

            for (int i = 0; i < tests; i++)
            {
                stopwatch.Restart();
                wordCounts = TextAnalysis.GetTextAnalysisParallelCD(text).ToDictionary(x => x.Key, x => x.Value);
                stopwatch.Stop();
                timeTests.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"Время работы многопоточного алгоритма с использованием CurrentDictionary: {timeTests.Average()}");
            var sortedDictParallel3 = wordCounts!.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
            WritingToFile(sortedDictParallel3, outputFilePathParallel3);
            timeTests.Clear();

            // Вызов параллельного метода с разделением на части
            string outputFilePathParallel = Path.GetFileNameWithoutExtension(inputFilename) + "_wordcountParallel.txt";// Путь к выходному файлу для параллельной обработки
            for (int i = 0; i < tests; i++)
            {
                stopwatch.Restart();
                wordCounts = TextAnalysis.GetTextAnalysisParallel(text);
                stopwatch.Stop();
                timeTests.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"Время работы многопоточного алгоритма с разделением на части: {timeTests.Average()}");
            var sortedDictParallel = wordCounts!.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
            WritingToFile(sortedDictParallel, outputFilePathParallel);

        }
        else
            Console.WriteLine("File is empty");

        Console.ReadLine();
    }

    //Запись результата в файл
    public static void WritingToFile(Dictionary<string, int> sortedDict, string outputFilePath) 
    {
        using StreamWriter writer = new(outputFilePath);
        foreach (var entry in sortedDict)
        {
            writer.WriteLine($"{entry.Key}: {entry.Value}");
        }
        Console.WriteLine("Text processing completed. The result is written");
    }
}
