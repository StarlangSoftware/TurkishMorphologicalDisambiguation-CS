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
            var correctFsmParses = new List<FsmParse>();
            foreach (var fsmParseList in fsmParses)
            {
                var bestRoot = rootWordStatistics.BestRootWord(fsmParseList, 0.0);
                FsmParse bestParse;
                if (bestRoot != null)
                {
                    fsmParseList.ReduceToParsesWithSameRoot(bestRoot);
                    var newBestParse = fsmParseList.CaseDisambiguator();
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