using AnnotatedTree;
using MorphologicalAnalysis;

namespace MorphologicalDisambiguation.AutoProcessor.ParseTree
{
    public abstract class TreeAutoDisambiguator : MorphologicalDisambiguation.AutoDisambiguation
    {
        protected abstract bool AutoFillSingleAnalysis(ParseTreeDrawable parseTree);
        protected abstract bool AutoDisambiguateWithRules(ParseTreeDrawable parseTree);
        protected abstract bool AutoDisambiguateSingleRootWords(ParseTreeDrawable parseTree);
        protected abstract bool AutoDisambiguateMultipleRootWords(ParseTreeDrawable parseTree);

        /// <summary>
        /// Constructor for the TreeAutoDisambiguator. Sets the morphological analyzer.
        /// </summary>
        /// <param name="morphologicalAnalyzer">Morphological analyzer used in disambiguation.</param>
        protected TreeAutoDisambiguator(FsmMorphologicalAnalyzer morphologicalAnalyzer)
        {
            this.morphologicalAnalyzer = morphologicalAnalyzer;
        }

        /// <summary>
        /// The method disambiguates the given parse tree. There are three main steps: In the first step, it eliminates
        /// the words having single morphological analysis. In the third step, depending on the pos tag of the parent node
        /// of a leaf node, the method tries to use different rule based disambiguators. In the third step, for the remaining
        /// words, longest root word disambiguation algorithm is used.
        /// </summary>
        /// <param name="parseTree">Parse tree to disambiguate.</param>
        public void AutoDisambiguate(ParseTreeDrawable parseTree)
        {
            bool modified;
            modified = AutoFillSingleAnalysis(parseTree);
            modified = modified || AutoDisambiguateWithRules(parseTree);
            modified = modified || AutoDisambiguateSingleRootWords(parseTree);
            modified = modified || AutoDisambiguateMultipleRootWords(parseTree);
            if (modified)
            {
                parseTree.Save();
            }
        }
    }
}