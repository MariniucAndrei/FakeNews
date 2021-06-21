using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace FakeNews.TFIDF
{
    public static class AlgorithmForTfidf
    {
        public static Dictionary<string,double> VocabularyIdf = new Dictionary<string, double>();

        public static double[][] Transform(string [] documents, int vocabularyThreshold = 2)
        {
            List<List<string>> stemmedDocs;
            List<string> vocabulary;

            vocabulary = GetVocabulary(documents, out stemmedDocs, vocabularyThreshold);

            if (VocabularyIdf.Count != 0) return TransformToTfidfVectors(stemmedDocs, VocabularyIdf);

            foreach (var term in vocabulary)
            {
                double numberOfDocsContainingTerm = stemmedDocs.Count(d => d.Contains(term));
                VocabularyIdf[term] =
                    Math.Log(stemmedDocs.Count / (1 + numberOfDocsContainingTerm));
            }

            return TransformToTfidfVectors(stemmedDocs, VocabularyIdf);
        }

        private static double[][] TransformToTfidfVectors(List<List<string>> stemmedDocs,
            Dictionary<string, double> vocabularyIdf)
        {
            var vectors = new List<List<double>>();
            foreach (var doc in stemmedDocs)
            {
                var vector = new List<double>();

                foreach (var vocab in vocabularyIdf)
                {
                    double tf = doc.Count(d => d == vocab.Key);
                    var tfidf = tf * vocab.Value;

                    vector.Add(tfidf);
                }

                vectors.Add(vector);
            }

            return vectors.Select(v => v.ToArray()).ToArray();
        }

        public static double[][] Normalize(double[][] vectors)
        {
            return vectors.Select(vector => Normalize(vector)).ToArray();
        }

        public static double[] Normalize(double[] vector)
        {
            var sumSquared = vector.Sum(value => value * value);

            var sqrtSumSquared = Math.Sqrt(sumSquared);

            return vector.Select(value => value / sqrtSumSquared).ToArray();
        }

        private static List<string> GetVocabulary(string[] docs, out List<List<string>> stemmedDocs,
            int vocabularyThreshold)
        {
            var wordCountList = new Dictionary<string, int>();
            stemmedDocs = new List<List<string>>();

            var docIndex = 0;

            foreach (var doc in docs)
            {
                var stemmedDoc = new List<string>();

                docIndex++;

                if (docIndex % 100 == 0)
                {
                    Console.WriteLine("Processing " + docIndex + "/" + docs.Length);
                }

                var parts2 = WordsProcessing(doc);

                var words = new List<string>();

                foreach (var part in parts2)
                {
                    var stripped = Regex.Replace(part, "[^a-zA-Z0-9]", "");

                    if (StopWords.StopWordsList.Contains(stripped.ToLower())) continue;
                    try
                    {
                        var english = new EnglishWord(stripped);
                        var stem = english.Stem;
                        words.Add(stem);

                        if (stem.Length > 0)
                        {
                            if (wordCountList.ContainsKey(stem))
                            {
                                wordCountList[stem]++;
                            }
                            else
                            {
                                wordCountList.Add(stem, 0);
                            }

                            stemmedDoc.Add(stem);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                stemmedDocs.Add(stemmedDoc);
            }

            var vocabList = wordCountList.Where(w => w.Value >= vocabularyThreshold);

            return vocabList.Select(item => item.Key).ToList();
        }

        private static string[] WordsProcessing(string text)
        {
            // Strip all HTML.
            text = Regex.Replace(text, "<[^<>]+>", "");

            // Strip numbers.
            text = Regex.Replace(text, "[0-9]+", "number");

            // Strip urls.
            text = Regex.Replace(text, @"(http|https)://[^\s]*", "httpaddr");

            // Strip email addresses.
            text = Regex.Replace(text, @"[^\s]+@[^\s]+", "emailaddr");

            // Strip dollar sign.
            text = Regex.Replace(text, "[$]+", "dollar");

            // Strip usernames.
            text = Regex.Replace(text, @"@[^\s]+", "username");

            // WordsProcessing and also get rid of any punctuation
            return text.Split(" @$/#.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray());
        }

        public static void Save(string filePath = "tfidf.dat")
        {
            using var fs = new FileStream(filePath, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, VocabularyIdf);
        }

        public static void Load(string filePath = "tfidf.dat")
        {
            using var fs = new FileStream(filePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            VocabularyIdf = (Dictionary<string, double>)formatter.Deserialize(fs);
        }
    }
}
