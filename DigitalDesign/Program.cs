using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using TextAnalysisLibrary;
using static System.Runtime.InteropServices.JavaScript.JSType;


class DigitalDesign
{
    public static HttpClient client = new();

    public static async Task Main()
    {
        Stopwatch stopwatch = new();
        List<long> timeTests = new(); //Список для измерения времени работы метода
        int tests = 20; //Количество тестов для измерения времени

        // Получение метода через рефлексию
        Type type = typeof(TextAnalysisLibrary.TextAnalysis);
        var methodInfo = type.GetMethod("GetTextAnalysis", BindingFlags.NonPublic | BindingFlags.Instance);

        // Чтение текстового файла
        Console.Write("Input file path: ");
        //string? inputFilename = Console.ReadLine();
        string inputFilename = @"C:\Users\Ленок\Desktop\project\DigitalDesign\Voina-mir.txt";

        if (!File.Exists(inputFilename))
        {
            Console.WriteLine("File doesn't exist");
            return;
        }
        string text = File.ReadAllText(inputFilename, Encoding.UTF8);


        if (text != null)
        {
            Dictionary<string, int>? wordCounts = new();
            var adress = Path.GetFileNameWithoutExtension(inputFilename);
            // Вызов приватного однопоточного метода из dll, получение результата и сортировка полученного результата
            string outputFilePath = adress + "_wordcount.txt";// Путь к выходному файлу однопоточного
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
            string outputFilePathParallelCD = adress  + "_wordcountParallelCD.txt";// Путь к выходному файлу для параллельной обработки

            for (int i = 0; i < tests; i++)
            {
                stopwatch.Restart();
                wordCounts = TextAnalysis.GetTextAnalysisParallelCD(text);
                stopwatch.Stop();
                timeTests.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"Время работы многопоточного алгоритма с использованием CurrentDictionary: {timeTests.Average()}");
            var sortedDictParallelCD = wordCounts!.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
            WritingToFile(sortedDictParallelCD, outputFilePathParallelCD);
            timeTests.Clear();

            // Вызов параллельного метода с разделением на части 
            string outputFilePathParallel = adress + "_wordcountParallel.txt";// Путь к выходному файлу для параллельной обработки

            for (int i = 0; i < tests; i++)
            {
                stopwatch.Restart();
                wordCounts = TextAnalysis.GetTextAnalysisParallel(text);
                stopwatch.Stop();
                timeTests.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"Время работы многопоточного алгоритма с разделением на части без сервиса : {timeTests.Average()}");
            timeTests.Clear();

            // Вызов параллельного метода с разделением на части с сервиса
            for (int i = 0; i < tests; i++)
            {
                stopwatch.Restart();
                var response = await client.PostAsJsonAsync("https://localhost:7268/api/TextAnalysis", text); // Вызов пост функции на сервисе
                wordCounts = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
                stopwatch.Stop();
                timeTests.Add(stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"Время работы многопоточного алгоритма с разделением на части с вызовом с сервиса: {timeTests.Average()}");
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
