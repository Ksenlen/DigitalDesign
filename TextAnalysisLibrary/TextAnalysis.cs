namespace TextAnalysisLibrary
{
    public class TextAnalysis
    {
        private  Dictionary<string, int> GetTextAnalysis(string text)
        {
            Dictionary<string, int> wordCount = new ();
            var delimeters = Enumerable.Range(0, 256).Select(i => (char)i).Where(c => !char.IsLetter(c)).ToArray();

            //string[] words = text.Split(' ');
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
    }
}