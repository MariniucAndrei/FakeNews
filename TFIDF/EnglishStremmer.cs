using System;
using System.Collections.Generic;
using System.Text;

namespace FakeNews.TFIDF
{
    public class EnglishWord
    {
        internal static EnglishWord CreateWithR1R2(string text)
        {
            var result = CreateForTest(text);
            result._r1 = CalculeazaRegiune(result.Stem, 0);
            result._r2 = CalculeazaRegiune(result.Stem, result._r1.Start);
            return result;
        }

        internal static EnglishWord CreateForTest(string text)
        {
            var word = new EnglishWord();
            word.Create(text);
            return word;
        }

        internal EnglishWord()
        { }

        public EnglishWord(string input)
        {
            Create(input);
            GenerateStem();
        }

        private void Create(string input)
        {
            Stem = input.ToLower();
            Original = input;
        }

        private static readonly char[] Vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
        private static readonly char[] NotShortSyllableNonVowels = { 'a', 'e', 'i', 'o', 'u', 'y', 'w', 'x', 'Y' };
        private static readonly string[] DoubleChars = { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };
        private const string ValidLiEnding = "cdeghkmnrt";

        private string _stem = string.Empty;
        private WordRegion _r1;
        private WordRegion _r2;

        private bool _cachedUpToDate;

        public string Stem
        {
            get => _stem;
            set
            {
                if (_stem == value) return;
                _stem = value;
                _cachedUpToDate = false;
            }
        }

        internal string GetR1()
        {
            CheckCache();
            return _r1.Text;
        }

        internal string GetR2()
        {
            CheckCache();
            return _r2.Text;
        }

        private void CheckCache()
        {
            if (_cachedUpToDate) return;
            _r1.GenerateRegion(_stem);
            _r2.GenerateRegion(_stem);
            _cachedUpToDate = true;
        }

        public string Original { get; private set; } = string.Empty;

        internal void GenerateStem()
        {
            if (IsException1())
                return;

            if (Stem.Length < 3)
                return;

            StandardiseApostrophesAndStripLeading();

            MarkYs();

            if (Stem.StartsWith("gener")
                    || Stem.StartsWith("arsen"))
            {
                _r1 = CalculeazaRegiune(Stem, 2);
            }
            else if (Stem.StartsWith("commun"))
            {
                _r1 = CalculeazaRegiune(Stem, 3);
            }
            else
            {
                _r1 = CalculeazaRegiune(Stem, 0);
            }
            _r2 = CalculeazaRegiune(Stem, _r1.Start);

            StripTrailingApostrophe();

            StripSuffixStep1A();

            if (IsException2())
                return;

            StripSuffixStep1B();
            
            ReplaceSuffixStep1C();
            
            ReplaceEndingStep2();
            
            ReplaceEndingStep3();
            
            StripSuffixStep4();
            
            StripSuffixStep5();
            
            Finally();
        }

        private bool IsException2()
        {
            return Stem switch
            {
                "inning" => true,
                "outing" => true,
                "canning" => true,
                "herring" => true,
                "earring" => true,
                "proceed" => true,
                "exceed" => true,
                "succeed" => true,
                _ => false
            };
        }

        private bool IsException1()
        {
            switch (Stem)
            {
                case "skis":
                    Stem = "ski";
                    return true;
                case "skies":
                    Stem = "sky";
                    return true;
                case "dying":
                    Stem = "die";
                    return true;
                case "lying":
                    Stem = "lie";
                    return true;
                case "tying":
                    Stem = "tie";
                    return true;

                case "idly":
                    Stem = "idl";
                    return true;
                case "gently":
                    Stem = "gentl";
                    return true;
                case "ugly":
                    Stem = "ugli";
                    return true;
                case "early":
                    Stem = "earli";
                    return true;
                case "only":
                    Stem = "onli";
                    return true;
                case "singly":
                    Stem = "singl";
                    return true;

                case "sky":
                    return true;
                case "news":
                    return true;
                case "howe":
                    return true;

                case "atlas":
                    return true;
                case "cosmos":
                    return true;
                case "bias":
                    return true;
                case "andes":
                    return true;
            }
            return false;
        }

        internal void Finally()
        {
            Stem = Stem.Replace("Y", "y");
        }

        internal void StripSuffixStep5()
        {
            if (EndsWithAndInR2("e")
                    || (EndsWithAndInR1("e") && IsShortSyllable(Stem.Length - 3) == false))
            {
                Stem = Stem.Remove(Stem.Length - 1);
                return;
            }
            if (EndsWithAndInR2("l")
                    && Stem.EndsWith("ll"))
            {
                Stem = Stem.Remove(Stem.Length - 1);
            }
        }

        internal void StripSuffixStep4()
        {
            if (EndsWithAndInR2("ement"))
            {
                Stem = Stem.Remove(Stem.Length - 5);
                return;
            }
            if (EndsWithAndInR2("ance")
                    || EndsWithAndInR2("ence")
                    || EndsWithAndInR2("able")
                    || EndsWithAndInR2("ible")
                    || EndsWithAndInR2("ment"))
            {
                Stem = Stem.Remove(Stem.Length - 4);
                return;
            }

            if (EndsWithAndInR2("ion")
                    && (Stem.EndsWith("tion") || Stem.EndsWith("sion")))
            {
                Stem = Stem.Remove(Stem.Length - 3);
                return;
            }

            if (Stem.EndsWith("ment"))
                return;

            if (EndsWithAndInR2("ant")
                    || EndsWithAndInR2("ent")
                    || EndsWithAndInR2("ism")
                    || EndsWithAndInR2("ate")
                    || EndsWithAndInR2("iti")
                    || EndsWithAndInR2("ous")
                    || EndsWithAndInR2("ize")
                    || EndsWithAndInR2("ive"))
            {
                Stem = Stem.Remove(Stem.Length - 3);
                return;
            }
            if (EndsWithAndInR2("al")
                    || EndsWithAndInR2("er")
                    || EndsWithAndInR2("ic")
                    )
            {
                Stem = Stem.Remove(Stem.Length - 2);
            }
        }

        internal void ReplaceEndingStep3()
        {
            if (EndsWithAndInR1("ational"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "e";
                return;
            }

            if (EndsWithAndInR1("tional"))
            {
                Stem = Stem.Substring(0, Stem.Length - 2);
                return;
            }

            if (EndsWithAndInR1("alize"))
            {
                Stem = Stem.Substring(0, Stem.Length - 3);
                return;
            }

            if (EndsWithAndInR2("ative"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5);
                return;
            }

            if (EndsWithAndInR1("icate") || EndsWithAndInR1("iciti"))
            {
                Stem = Stem.Substring(0, Stem.Length - 3);
                return;
            }

            if (EndsWithAndInR1("ical"))
            {
                Stem = Stem.Substring(0, Stem.Length - 2);
                return;
            }

            if (EndsWithAndInR1("ness"))
            {
                Stem = Stem.Substring(0, Stem.Length - 4);
                return;
            }

            if (EndsWithAndInR1("ful"))
            {
                Stem = Stem.Substring(0, Stem.Length - 3);
            }
        }

        internal void ReplaceEndingStep2()
        {
            if (EndsWithAndInR1("ational"))
            {
                Stem = Stem.Substring(0, Stem.Length - 7) + "ate";
                return;
            }
            if (EndsWithAndInR1("fulness"))
            {
                Stem = Stem.Substring(0, Stem.Length - 7) + "ful";
                return;
            }
            if (EndsWithAndInR1("iveness"))
            {
                Stem = Stem.Substring(0, Stem.Length - 7) + "ive";
                return;
            }
            if (EndsWithAndInR1("ization"))
            {
                Stem = Stem.Substring(0, Stem.Length - 7) + "ize";
                return;
            }
            if (EndsWithAndInR1("ousness"))
            {
                Stem = Stem.Substring(0, Stem.Length - 7) + "ous";
                return;
            }
            if (EndsWithAndInR1("biliti"))
            {
                Stem = Stem.Substring(0, Stem.Length - 6) + "ble";
                return;
            }
            if (EndsWithAndInR1("lessli"))
            {
                Stem = Stem.Substring(0, Stem.Length - 6) + "less";
                return;
            }
            if (EndsWithAndInR1("tional"))
            {
                Stem = Stem.Substring(0, Stem.Length - 6) + "tion";
                return;
            }
            if (EndsWithAndInR1("alism"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "al";
                return;
            }
            if (EndsWithAndInR1("aliti"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "al";
                return;
            }
            if (EndsWithAndInR1("ation"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "ate";
                return;
            }
            if (EndsWithAndInR1("entli"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "ent";
                return;
            }
            if (EndsWithAndInR1("fulli"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "ful";
                return;
            }
            if (EndsWithAndInR1("iviti"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "ive";
                return;
            }
            if (EndsWithAndInR1("ousli"))
            {
                Stem = Stem.Substring(0, Stem.Length - 5) + "ous";
                return;
            }
            if (EndsWithAndInR1("abli"))
            {
                Stem = Stem.Substring(0, Stem.Length - 4) + "able";
                return;
            }
            if (EndsWithAndInR1("alli"))
            {
                Stem = Stem.Substring(0, Stem.Length - 4) + "al";
                return;
            }
            if (EndsWithAndInR1("anci"))
            {
                Stem = Stem.Substring(0, Stem.Length - 4) + "ance";
                return;
            }
            if (EndsWithAndInR1("ator"))
            {
                Stem = Stem.Substring(0, Stem.Length - 4) + "ate";
                return;
            }
            if (EndsWithAndInR1("enci"))
            {
                Stem = Stem.Substring(0, Stem.Length - 4) + "ence";
                return;
            }
            if (EndsWithAndInR1("izer"))
            {
                Stem = Stem.Substring(0, Stem.Length - 4) + "ize";
                return;
            }
            if (EndsWithAndInR1("bli"))
            {
                Stem = Stem.Substring(0, Stem.Length - 3) + "ble";
                return;
            }
            if (EndsWithAndInR1("ogi"))
            {
                Stem = Stem.Substring(0, Stem.Length - 3) + "og";
                return;
            }
            if (EndsWithAndInR1("li")
                    && IsValidLiEnding())
            {
                Stem = Stem.Substring(0, Stem.Length - 2);
            }
        }

        private bool IsValidLiEnding()
        {
            if (Stem.Length <= 2) return false;
            var preLi = Stem.Substring(Stem.Length - 3, 1);
            return ValidLiEnding.Contains(preLi);
        }

        private bool EndsWithAndInR1(string suffix)
        {
            return (Stem.EndsWith(suffix)
                    && GetR1().Contains(suffix));
        }

        private bool EndsWithAndInR2(string suffix)
        {
            return (Stem.EndsWith(suffix)
                    && GetR2().Contains(suffix));
        }

        internal void ReplaceSuffixStep1C()
        {
            if (Stem.EndsWith("y", StringComparison.OrdinalIgnoreCase)
                    && (Stem.Length > 2)
                    && (Stem.IndexOfAny(Vowels, Stem.Length - 2) != Stem.Length - 2))
            {
                Stem = Stem.Substring(0, Stem.Length - 1) + "i";
            }
        }

        internal void StandardiseApostrophesAndStripLeading()
        {
            Stem = Stem.Replace('�', '\'').Replace('`', '\'').Replace('"', '\'');

            if (Stem[0] == '\'')
                Stem = Stem.Remove(0, 1);
        }

        internal static WordRegion CalculeazaRegiune(string word, int offset)
        {
            if (offset >= word.Length)
                return new WordRegion(word.Length, word.Length);

            var firstVowel = word.IndexOfAny(Vowels, offset);
            var firstNonVowel = IndexOfNone(word, Vowels, firstVowel);
            var nextVowel = firstNonVowel + 1;

            var result = new WordRegion();
            if (nextVowel > 0
                    && nextVowel < word.Length)
                result.Start = nextVowel;
            else
                result.Start = word.Length;
            result.End = word.Length;
            return result;
        }

        internal static int IndexOfNone(string word, char[] ignoredChars, int first)
        {
            if (first < 0)
                return -1;

            var firstNone = first;
            do
            {
                firstNone++;
            }
            while (firstNone < word.Length
                            && word.Substring(firstNone, 1).IndexOfAny(ignoredChars) > -1);
            return firstNone;
        }

        internal void StripSuffixStep1B()
        {
            if (Stem.EndsWith("eed")
                    || Stem.EndsWith("eedly"))
            {
                if (!EndsWithAndInR1("eed") && !EndsWithAndInR1("eedly")) return;
                if (_r1.Contains(Stem.Length))
                {
                    Stem = Stem.Substring(0, Stem.LastIndexOf("eed", StringComparison.Ordinal)) + "ee";
                }
                return;
            }

            if ((Stem.EndsWith("ed") && Stem.IndexOfAny(Vowels, 0, Stem.Length - 2) != -1)
                    || (Stem.EndsWith("edly") && Stem.IndexOfAny(Vowels, 0, Stem.Length - 4) != -1)
                    || (Stem.EndsWith("ing") && Stem.IndexOfAny(Vowels, 0, Stem.Length - 3) != -1)
                    || (Stem.EndsWith("ingly") && Stem.IndexOfAny(Vowels, 0, Stem.Length - 5) != -1))
            {
                StripEnding(new[] { "ed", "edly", "ing", "ingly" });

                if (Stem.EndsWith("at")
                        || Stem.EndsWith("bl")
                        || Stem.EndsWith("iz"))
                {
                    Stem += "e";
                    return;
                }
                
                var end2Chars = Stem.Substring(Stem.Length - 2, 2);
                var doubleEndings = new List<string>(DoubleChars);
                if (doubleEndings.Contains(end2Chars))
                {
                    Stem = Stem.Remove(Stem.Length - 1);
                    return;
                }

                if (IsShortWord())
                {
                    Stem += "e";
                }

            }
        }

        internal bool IsShortWord()
        {
            CheckCache();

            var lastVowelIndex = Stem.LastIndexOfAny(Vowels);
            return (lastVowelIndex > -1
                    && IsShortSyllable(lastVowelIndex)
                    && string.IsNullOrEmpty(GetR1()));
        }

        internal bool IsShortSyllable(int index)
        {
            var vowelIndex = Stem.IndexOfAny(Vowels, index);
            if (vowelIndex < 0)
                return false;

            if (vowelIndex > 0)
            {
                var expectedShortEnd = vowelIndex + 2;
                var nextVowelIndex = Stem.IndexOfAny(Vowels, vowelIndex + 1);
                if (nextVowelIndex == expectedShortEnd
                        || Stem.Length == expectedShortEnd)
                {
                    var nonVowelIndex = Stem.IndexOfAny(NotShortSyllableNonVowels, vowelIndex + 1);
                    var earlyVowelIndex = Stem.IndexOfAny(Vowels, vowelIndex - 1);
                    return (nonVowelIndex != vowelIndex + 1
                                            && earlyVowelIndex == vowelIndex);
                }

                return false;
            }

            return (Stem.IndexOfAny(Vowels) == 0
                    && Stem.Length > 1
                    && Stem.IndexOfAny(Vowels, 1) != 1);
        }

        private bool StripEnding(string[] endings)
        {
            foreach (var ending in endings)
            {
                if (!Stem.EndsWith(ending)) continue;
                Stem = Stem.Remove(Stem.Length - ending.Length);
                return true;
            }
            return false;
        }

        internal void StripSuffixStep1A()
        {
            if (Stem.EndsWith("sses"))
            {
                Stem = Stem.Substring(0, Stem.Length - 2);
                return;
            }

            if (Stem.EndsWith("ies")
                    || Stem.EndsWith("ied"))
            {
                Stem = Stem.Length > 4 ? Stem.Substring(0, Stem.Length - 2) : Stem.Substring(0, Stem.Length - 1);
                return;
            }

            if (Stem.EndsWith("us")
                    || Stem.EndsWith("ss"))
            {
                return;
            }

            if (!Stem.EndsWith("s") || Stem.Length <= 2 ||
                Stem.Substring(0, Stem.Length - 2).IndexOfAny(Vowels) <= -1) return;
            Stem = Stem.Substring(0, Stem.Length - 1);
        }

        internal void StripTrailingApostrophe()
        {
            if (Stem.EndsWith("'s'"))
            {
                Stem = Stem.Substring(0, Stem.Length - 3);
                return;
            }

            if (Stem.EndsWith("'s"))
            {
                Stem = Stem.Substring(0, Stem.Length - 2);
                return;
            }

            if (Stem.EndsWith("'"))
            {
                Stem = Stem.Substring(0, Stem.Length - 1);
            }
        }

        internal void MarkYs()
        {
            var vowelsSearch = new List<char>(Vowels);
            var previousWasVowel = true;
            var result = new StringBuilder();
            foreach (var c in Stem)
            {
                if (c == 'y'
                        && previousWasVowel)
                    result.Append('Y');
                else
                    result.Append(c);

                previousWasVowel = vowelsSearch.Contains(c);
            }
            Stem = result.ToString();
        }
    }
}