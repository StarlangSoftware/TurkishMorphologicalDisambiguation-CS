using System.Collections.Generic;
using AnnotatedSentence;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation.AutoProcessor.Sentence
{
    public class TurkishSentenceAutoDisambiguator : SentenceAutoDisambiguator
    {
        LongestRootFirstDisambiguation longestRootFirstDisambiguation;
        
        /**
         * <summary> Constructor for the class.</summary>
         * <param name="rootWordStatistics">The object contains information about the selected correct root words in a corpus for a set
         *                           of possible lemma. For example, the lemma
         *                           `günü': 2 possible root words `gün' and `günü'
         *                           `çağlar' : 2 possible root words `çağ' and `çağlar'</param>
         */
        public TurkishSentenceAutoDisambiguator() : base(new FsmMorphologicalAnalyzer())
        {
            longestRootFirstDisambiguation = new LongestRootFirstDisambiguation();
        }

        /**
         * <summary> Constructor for the class.</summary>
         * <param name="fsm">               Finite State Machine based morphological analyzer</param>
         * <param name="rootWordStatistics">The object contains information about the selected correct root words in a corpus for a set
         *                           of possible lemma. For example, the lemma
         *                           `günü': 2 possible root words `gün' and `günü'
         *                           `çağlar' : 2 possible root words `çağ' and `çağlar'</param>
         */
        public TurkishSentenceAutoDisambiguator(FsmMorphologicalAnalyzer fsm) : base(fsm)
        {
        }
        
        /**
         * <summary> If the words has only single root in its possible parses, the method disambiguates by looking special cases.
         * The cases are implemented in the caseDisambiguator method.</summary>
         * <param name="fsmParseList">Morphological parses of the word.</param>
         * <param name="word">Word to be disambiguated.</param>
         */
        private void SetParseAutomatically(FsmParse disambiguatedParse, AnnotatedWord word)
        {
            word.SetParse(disambiguatedParse.TransitionList());
            word.SetMetamorphicParse(disambiguatedParse.WithList());
        }

        /**
         * <summary> The method disambiguates words with multiple possible root words in its morphological parses. If the word
         * is already morphologically disambiguated, the method does not disambiguate that word. The method first check
         * for multiple root words by using rootWords method. If there are multiple root words, the method select the most
         * occurring root word (if its occurence wrt other root words occurence is above some threshold) for that word
         * using the bestRootWord method. If root word is selected, then the case for single root word is called.</summary>
         * <param name="sentence">The sentence to be disambiguated automatically.</param>
         */
        protected override void AutoDisambiguateMultipleRootWords(AnnotatedSentence.AnnotatedSentence sentence)
        {
            FsmParseList[] fsmParses = morphologicalAnalyzer.RobustMorphologicalAnalysis(sentence);
            List<FsmParse> correctParses = longestRootFirstDisambiguation.Disambiguate(fsmParses);
            for (int i = 0; i < sentence.WordCount(); i++){
                AnnotatedWord word = (AnnotatedWord) sentence.GetWord(i);
                if (word.GetParse() == null){
                    SetParseAutomatically(correctParses[i], word);
                }
            }
        }

    }
}