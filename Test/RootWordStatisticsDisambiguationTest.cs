using MorphologicalAnalysis;
using MorphologicalDisambiguation;
using NUnit.Framework;

namespace Test
{
    public class RootWordStatisticsDisambiguationTest
    {
        [Test]
        public void TestDisambiguation()
        {
            var fsm = new FsmMorphologicalAnalyzer();
            var corpus = new DisambiguationCorpus("../../../penntreebank.txt");
            var algorithm = new RootWordStatisticsDisambiguation();
            algorithm.Train(corpus);
            var correctParse = 0;
            var correctRoot = 0;
            for (var i = 0; i < corpus.SentenceCount(); i++)
            {
                var sentenceAnalyses = fsm.RobustMorphologicalAnalysis(corpus.GetSentence(i));
                var fsmParses = algorithm.Disambiguate(sentenceAnalyses);
                for (var j = 0; j < corpus.GetSentence(i).WordCount(); j++)
                {
                    var word = (DisambiguatedWord) corpus.GetSentence(i).GetWord(j);
                    if (fsmParses[j].TransitionList().Equals(word.GetParse().ToString()))
                    {
                        correctParse++;
                    }

                    if (fsmParses[j].GetWord().Equals(word.GetParse().GetWord()))
                    {
                        correctRoot++;
                    }
                }
            }

            Assert.AreEqual(0.960, (correctRoot + 0.0) / corpus.NumberOfWords(), 0.001);
            Assert.AreEqual(0.790, (correctParse + 0.0) / corpus.NumberOfWords(), 0.001);
        }
    }
}