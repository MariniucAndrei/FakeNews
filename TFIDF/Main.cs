using System;

namespace FakeNews.TFIDF
{
    public class Main
    {
        public static string Tfidf(string[] documents)
        {
            var tfidfResult = AlgorithmForTfidf.Transform(documents);
            tfidfResult = AlgorithmForTfidf.Normalize(tfidfResult);
            var message = string.Empty;
            var newsCredibility = string.Empty;

            for (var index = 0; index < tfidfResult.Length; index++)
            {
                Console.WriteLine(documents[index]);

                foreach (var value in tfidfResult[index])
                {
                    Console.Write(value + ", ");

                    newsCredibility = value < 0 ? "Real" : "Fake";
                }

                message = $"This news is" + newsCredibility;

                Console.WriteLine("\n");
            }

            return message;
        }
    }
}
