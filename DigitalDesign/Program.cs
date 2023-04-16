using System;
using System.Text;


Console.Write("Input file path: ");
string inputFilename = Console.ReadLine();
if (!File.Exists(inputFilename))
{
    Console.WriteLine( "File doesn't exist");
    return;
}

string outputFilename = Path.GetFileNameWithoutExtension(inputFilename) + "_wordcount.txt";
Dictionary<string, int> wordCounts = new Dictionary<string, int>();
var delimeters = Enumerable.Range(0, 256).Select(i => (char)i).Where(c => !char.IsLetter(c)).ToArray();

using (StreamReader reader = new StreamReader(inputFilename, Encoding.UTF8))
{
    string line;
    while ((line = reader.ReadLine()) != null)
    {
        string[] words = line.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            if (wordCounts.ContainsKey(word))
                wordCounts[word]++;
            else
                wordCounts[word] = 1;
        }
    }
}

List<KeyValuePair<string, int>> sortedWordCounts = wordCounts.ToList();
sortedWordCounts.Sort((x, y) => y.Value.CompareTo(x.Value));

using (StreamWriter writer = new StreamWriter(outputFilename))
{
    foreach (KeyValuePair<string, int> pair in sortedWordCounts)
    {
        writer.WriteLine("{0}\t{1}", pair.Key, pair.Value);
    }
}