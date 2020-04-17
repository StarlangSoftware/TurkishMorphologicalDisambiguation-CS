using System.Collections.Generic;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation
{
    public interface MorphologicalDisambiguator
    { 
        /**
        * <summary>Method to train the given {@link DisambiguationCorpus}.</summary>
        *
        * <param name="corpus">{@link DisambiguationCorpus} to train.</param>
        */
        void Train(DisambiguationCorpus corpus);

        /**
         * <summary> Method to disambiguate the given {@link FsmParseList}.</summary>
         *
         * <param name="fsmParses">{@link FsmParseList} to disambiguate.</param>
         * <returns>ArrayList of {@link FsmParse}.</returns>
         */
        List<FsmParse> Disambiguate(FsmParseList[] fsmParses);

        /**
         * <summary> Method to save a model.</summary>
         */
        void SaveModel();

        /**
         * <summary> Method to load a model.</summary>
         */
        void LoadModel();
        
    }
}