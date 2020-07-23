using System;
using System.Collections.Generic;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation
{
    public class DummyDisambiguation : MorphologicalDisambiguator
    {
        /**
         * <summary> Train method implements method in {@link MorphologicalDisambiguator}.</summary>
         *
         * <param name="corpus">{@link DisambiguationCorpus} to train.</param>
         */
        public void Train(DisambiguationCorpus corpus)
        {
            throw new System.NotImplementedException();
        }

        /**
         * <summary> Overridden disambiguate method takes an array of {@link FsmParseList} and loops through its items, if the current FsmParseList's
         * size is greater than 0, it adds a random parse of this list to the correctFsmParses {@link ArrayList}.</summary>
         *
         * <param name="fsmParses">{@link FsmParseList} to disambiguate.</param>
         * <returns>correctFsmParses {@link ArrayList}.</returns>
         */
        public List<FsmParse> Disambiguate(FsmParseList[] fsmParses)
        {
            var random = new Random();
            var correctFsmParses = new List<FsmParse>();
            foreach (var fsmParseList in fsmParses) {
                if (fsmParseList.Size() > 0)
                {
                    correctFsmParses.Add(fsmParseList.GetFsmParse(random.Next(fsmParseList.Size())));
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