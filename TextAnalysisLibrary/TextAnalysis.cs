using System.Collections.Concurrent;

namespace TextAnalysisLibrary
{
    public class TextAnalysis
    {
        static char[] delimeters = Enumerable.Range(0, 256).Select(i => (char)i).Where(c => !char.IsLetter(c)).ToArray();
      
        //Однопоточный приватный метод
        private Dictionary<string, int> GetTextAnalysis(string text)
        {
            Dictionary<string, int> wordCount = new();
            string[] words = text.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                if (wordCount.ContainsKey(word))
                    wordCount[word]++;
                else
                    wordCount[word] = 1;
            }
            return wordCount;
        }

        //Многопоточные методы  
          
        public static ConcurrentDictionary<string, int> GetTextAnalysisParallelCD(string text)
        {
            ConcurrentDictionary<string, int> wordCount = new();
            string[] words = text.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
            Parallel.ForEach(words, word =>
            {
                wordCount.AddOrUpdate(word, 1, (_, count) => count + 1);
            });
            return wordCount;
        }


        public static Dictionary<string, int> GetTextAnalysisParallel(string text)
        {
            string[] words = text.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
            int processorCount = Environment.ProcessorCount;
            Dictionary<string, int>[] partialWordCounts = new Dictionary<string, int>[processorCount];

            
            Parallel.For(0, processorCount, i =>
            {
                partialWordCounts[i] = new Dictionary<string, int>();
                int startIndex = (words.Length / processorCount) * i;
                int endIndex = (i == processorCount - 1) ? words.Length : (words.Length / processorCount) * (i + 1);

                for (int j = startIndex; j < endIndex; j++)
                {
                    string word = words[j];
                    if (partialWordCounts[i].ContainsKey(word))
                        partialWordCounts[i][word]++;
                    else
                        partialWordCounts[i][word] = 1;
                }
            });

            Dictionary<string, int> wordCount = new ();
            foreach (var partialCount in partialWordCounts)
            {
                foreach (var kvp in partialCount)
                {
                    if (wordCount.ContainsKey(kvp.Key))
                        wordCount[kvp.Key] += kvp.Value;
                    else
                        wordCount[kvp.Key] = kvp.Value;
                }
            }

            return wordCount;
        }
    }
}