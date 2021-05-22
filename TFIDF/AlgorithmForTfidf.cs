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

        public static double[][] Transform(string [] documents, int vocabularyThreshold = 3)
        {
            List<List<string>> stemmedDocs;
            List<string> vocabulary;

            //Get the vocabulary and stem the documents at the same time
            vocabulary = GetVocabulary(documents, out stemmedDocs, vocabularyThreshold);

            if (VocabularyIdf.Count == 0)
            {
                //Calculate the IDF for each vocabulary term
                foreach (var term in vocabulary)
                {
                    double numberOfDocsContainingTerm = stemmedDocs.Count(d => d.Contains(term));
                    VocabularyIdf[term] =
                        Math.Log(stemmedDocs.Count / (1 + numberOfDocsContainingTerm));
                }
            }

            //Trasform each document into a vector of tfidf values
            return TransformToTFIDFVectors(stemmedDocs, VocabularyIdf);
        }

        private static double[][] TransformToTFIDFVectors(List<List<string>> stemmedDocs,
            Dictionary<string, double> vocabularyIDF)
        {
            //Transform each document into a vector of tfidf values
            var vectors = new List<List<double>>();
            foreach (var doc in stemmedDocs)
            {
                var vector = new List<double>();

                foreach (var vocab in vocabularyIDF)
                {
                    //Term frequency = count how many times the term appears in this document
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
            //Normalize the vectors using L2-Norm
            var normalizedVectors = new List<double[]>();
            foreach (var vector in vectors)
            {
                var normalized = Normalize(vector);
                normalizedVectors.Add(normalized);
            }

            return normalizedVectors.ToArray();
        }

        public static double[] Normalize(double[] vector)
        {
            var result = new List<double>();

            double sumSquared = 0;
            foreach (var value in vector)
            {
                sumSquared += value * value;
            }

            var sqrtSumSquared = Math.Sqrt(sumSquared);

            foreach (var value in vector)
            {
                //L2-norm: Xi = Xi /Sqrt(X0^2 + x1^2 + ... + Xn^2)
                result.Add(value / sqrtSumSquared);
            }

            return result.ToArray();
        }

        public static void Save(string filePath = "vocabulary.dat")
        {
            //Save result to disk
            using var fs = new FileStream(filePath, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, VocabularyIdf);
        }

        public static void Load(string filePath = "vocabulary.dat")
        {
            //Load from disk
            using var fs = new FileStream(filePath, FileMode.Open);
            var formatter = new BinaryFormatter();
            VocabularyIdf = (Dictionary<string, double>) formatter.Deserialize(fs);
        }

        private static List<string> GetVocabulary(string[] docs, out List<List<string>> stemmedDocs,
            int vocabularyThreshold)
        {
            var vocabulary = new List<string>();
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

                var parts2 = Tokenize(doc);

                var words = new List<string>();
                foreach (var part in parts2)
                {
                    //Strip non-alphanumeric characters
                    var stripped = Regex.Replace(part, "[^a-zA-Z0-9]", "");

                    if (!StopWords.StopWordsList.Contains(stripped.ToLower()))
                    {
                        try
                        {
                            var english = new EnglishWord(stripped);
                            var stem = english.Stem;
                            words.Add(stem);

                            if (stem.Length > 0)
                            {
                                //Build the word count list
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
                }

                stemmedDocs.Add(stemmedDoc);
            }

            //Get the top words
            var vocabList = wordCountList.Where(w => w.Value >= vocabularyThreshold);
            foreach (var item in vocabList)
            {
                vocabulary.Add(item.Key);
            }

            return vocabulary;
        }

        private static string[] Tokenize(string text)
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

            // Tokenize and also get rid of any punctuation
            return text.Split(" @$/#.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray());
        }
    }
}
