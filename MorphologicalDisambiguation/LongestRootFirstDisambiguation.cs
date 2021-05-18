using System.Collections.Generic;
using System.IO;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation
{
    public class LongestRootFirstDisambiguation : MorphologicalDisambiguator
    {

        private Dictionary<string, string> rootList;

        public LongestRootFirstDisambiguation()
        {
            ReadFromFile("rootlist.txt");
        }

        public LongestRootFirstDisambiguation(string fileName)
        {
            ReadFromFile(fileName);
        }

        private void ReadFromFile(string fileName)
        {
            rootList = new Dictionary<string, string>();
            var assembly = typeof(LongestRootFirstDisambiguation).Assembly;
            var stream = assembly.GetManifestResourceStream("MorphologicalDisambiguation." + fileName);
            var streamReader = new StreamReader(stream);
            var line = streamReader.ReadLine();
            while (line != null)
            {
                var items = line.Split();
                rootList[items[0]] = items[1];
                line = streamReader.ReadLine();
            }
        }
        /**
         * <summary> Train method implements method in {@link MorphologicalDisambiguator}.</summary>
         *
         * <param name="corpus">{@link DisambiguationCorpus} to train.</param>
         */
        public void Train(DisambiguationCorpus corpus)
        {
        }

        /**
         * <summary> The disambiguate method gets an array of fsmParses. Then loops through that parses and finds the longest root
         * word. At the end, gets the parse with longest word among the fsmParses and adds it to the correctFsmParses
         * {@link ArrayList}.</summary>
         *
         * <param name="fsmParses">{@link FsmParseList} to disambiguate.</param>
         * <returns>correctFsmParses {@link ArrayList} which holds the parses with longest root words.</returns>
         */
        public List<FsmParse> Disambiguate(FsmParseList[] fsmParses)
        {
            FsmParse bestParse;
            var correctFsmParses = new List<FsmParse>();
            var i = 0;
            var bestRoot = "";
            foreach (var fsmParseList in fsmParses)
            {
                var surfaceForm = fsmParseList.GetFsmParse(0).GetSurfaceForm();
                var rootFound = false;
                if (rootList.ContainsKey(surfaceForm))
                {
                    bestRoot = rootList[surfaceForm];
                    for (var j = 0; j < fsmParseList.Size(); j++) {
                        if (fsmParseList.GetFsmParse(j).GetWord().GetName() == bestRoot) {
                            rootFound = true;
                            break;
                        }
                    }
                }

                if (bestRoot == "" || !rootFound)
                {
                    bestParse = fsmParseList.GetParseWithLongestRootWord();
                    fsmParseList.ReduceToParsesWithSameRootAndPos(bestParse.GetWordWithPos());
                }
                else
                {
                    fsmParseList.ReduceToParsesWithSameRoot(bestRoot);
                }
                var newBestParse = AutoDisambiguation.CaseDisambiguator(i, fsmParses, correctFsmParses);
                if (newBestParse != null)
                {
                    bestParse = newBestParse;
                }
                else
                {
                    bestParse = fsmParseList.GetFsmParse(0);
                }
                correctFsmParses.Add(bestParse);
                i++;
            }
            return correctFsmParses;
        }

        public void SaveModel()
        {
        }

        public void LoadModel()
        {
        }
    }
}