using System.Collections.Generic;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation
{
    public class LongestRootFirstDisambiguation : MorphologicalDisambiguator
    {
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
            var correctFsmParses = new List<FsmParse>();
            foreach (var fsmParseList in fsmParses)
            {
                var bestParse = fsmParseList.GetParseWithLongestRootWord();
                if (bestParse != null)
                {
                    correctFsmParses.Add(bestParse);
                }
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