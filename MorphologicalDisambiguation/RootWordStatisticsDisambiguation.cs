using System.Collections.Generic;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation
{
    public class RootWordStatisticsDisambiguation : MorphologicalDisambiguator
    {
        private RootWordStatistics rootWordStatistics;

        /**
         * <summary> Train method implements method in {@link MorphologicalDisambiguator}.</summary>
         *
         * <param name="corpus">{@link DisambiguationCorpus} to train.</param>
         */
        public void Train(DisambiguationCorpus corpus)
        {
            rootWordStatistics = new RootWordStatistics("penntreebank_statistics.txt");
        }

        public List<FsmParse> Disambiguate(FsmParseList[] fsmParses)
        {
            FsmParse bestParse;
            string bestRoot;
            var correctFsmParses = new List<FsmParse>();
            int i = 0;
            foreach (var fsmParseList in fsmParses)
            {
                string rootWords = fsmParseList.RootWords();
                if (rootWords.Contains("$"))
                {
                    bestRoot = rootWordStatistics.BestRootWord(fsmParseList, 0.0);
                    if (bestRoot == null)
                    {
                        bestRoot = fsmParseList.GetParseWithLongestRootWord().GetWord().GetName();
                    }
                }
                else
                {
                    bestRoot = rootWords;
                }

                if (bestRoot != null)
                {
                    fsmParseList.ReduceToParsesWithSameRoot(bestRoot);
                    FsmParse newBestParse = AutoDisambiguation.CaseDisambiguator(i, fsmParses, correctFsmParses);
                    if (newBestParse != null)
                    {
                        bestParse = newBestParse;
                    }
                    else
                    {
                        bestParse = fsmParseList.GetFsmParse(0);
                    }
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