using System.Collections.Generic;
using System.IO;
using DataStructure;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation
{
    public class RootWordStatistics
    {
        private Dictionary<string, CounterHashMap<string>> statistics;

        /**
         * <summary> Constructor of {@link RootWordStatistics} class that generates a new {@link HashMap} for statistics.</summary>
         */
        public RootWordStatistics()
        {
            statistics = new Dictionary<string, CounterHashMap<string>>();
        }

        /**
         * <summary> Constructor of {@link RootWordStatistics} class which fetches the statistics from given input file.</summary>
         *
         * <param name="fileName">File to get statistics.</param>
         */
        public RootWordStatistics(string fileName)
        {
            var assembly = typeof(RootWordStatistics).Assembly;
            var stream = assembly.GetManifestResourceStream("MorphologicalDisambiguation." + fileName);
            ReadFromFile(stream);
        }

        /**
         * <summary> Constructor of {@link RootWordStatistics} class which fetches the statistics from given input file.</summary>
         *
         * <param name="inputStream">File to get statistics.</param>
         */
        public RootWordStatistics(Stream inputStream)
        {
            ReadFromFile(inputStream);
        }

        private void ReadFromFile(Stream inputStream)
        {
            statistics = new Dictionary<string, CounterHashMap<string>>();
            var streamReader = new StreamReader(inputStream);
            var size = int.Parse(streamReader.ReadLine());
            for (var i = 0; i < size; i++)
            {
                var line = streamReader.ReadLine();
                var items = line.Split(" ");
                var rootWord = items[0];
                var count = int.Parse(items[1]);
                var map = new CounterHashMap<string>();
                for (var j = 0; j < count; j++)
                {
                    line = streamReader.ReadLine();
                    items = line.Split(" ");
                    map.PutNTimes(items[0], int.Parse(items[1]));
                }

                statistics[rootWord] = map;
            }

            streamReader.Close();
        }

        /**
         * <summary> Method to check whether statistics contains the given string.</summary>
         *
         * <param name="key">string to look for.</param>
         * <returns>Returns true if this map contains a mapping for the specified key.</returns>
         */
        public bool ContainsKey(string key)
        {
            return statistics.ContainsKey(key);
        }

        /**
         * <summary> Method to get the value of the given string.</summary>
         *
         * <param name="key">string to look for.</param>
         * <returns>Returns the value to which the specified key is mapped, or {@code null} if this map contains no mapping for the key.</returns>
         */
        public CounterHashMap<string> Get(string key)
        {
            return statistics[key];
        }

        /**
         * <summary> Method to associates a string along with a {@link CounterHashMap} in the statistics.</summary>
         *
         * <param name="key">           Key with which the specified value is to be associated.</param>
         * <param name="wordStatistics">Value to be associated with the specified key.</param>
         */
        public void Put(string key, CounterHashMap<string> wordStatistics)
        {
            statistics[key] = wordStatistics;
        }

        /**
         * <summary> Method to save statistics into file specified with the input file name.</summary>
         *
         * <param name="fileName">File to save the statistics.</param>
         */
        public void SaveStatistics(string fileName)
        {
            var streamWriter = new StreamWriter(fileName);
            streamWriter.WriteLine(statistics.Count);
            foreach (var rootWord in statistics.Keys){
                var map = statistics[rootWord];
                streamWriter.WriteLine(rootWord + " " + ((Dictionary<string, int>)map).Count);
                streamWriter.Write(map.ToString());
            }
            streamWriter.Close();
        }

        /**
         * <summary> The bestRootWord method gets the root word of given {@link FsmParseList} and if statistics has a value for that word,
         * it returns the max value associated with that word.</summary>
         *
         * <param name="parseList">{@link FsmParseList} to check.</param>
         * <param name="threshold">A double value for limit.</param>
         * <returns>The max value for the root word.</returns>
         */
        public string BestRootWord(FsmParseList parseList, double threshold)
        {
            var rootWords = parseList.RootWords();
            if (statistics.ContainsKey(rootWords))
            {
                var rootWordStatistics = statistics[rootWords];
                return rootWordStatistics.Max(threshold);
            }

            return null;
        }
    }
}