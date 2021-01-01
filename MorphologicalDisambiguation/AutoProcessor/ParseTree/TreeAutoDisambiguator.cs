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

        protected TreeAutoDisambiguator(FsmMorphologicalAnalyzer morphologicalAnalyzer,
            RootWordStatistics rootWordStatistics)
        {
            this.morphologicalAnalyzer = morphologicalAnalyzer;
            this.rootWordStatistics = rootWordStatistics;
        }

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