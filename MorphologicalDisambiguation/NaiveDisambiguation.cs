using System.Collections.Generic;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using NGram;

namespace MorphologicalDisambiguation
{
    public abstract class NaiveDisambiguation : MorphologicalDisambiguator
    {
        protected NGram<Word> wordUniGramModel;
        protected NGram<Word> igUniGramModel;

        public abstract void Train(DisambiguationCorpus corpus);
        public abstract List<FsmParse> Disambiguate(FsmParseList[] fsmParses);
        public abstract void SaveModel();
        public abstract void LoadModel();
    }
}