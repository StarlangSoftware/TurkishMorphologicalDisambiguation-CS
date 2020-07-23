using MorphologicalAnalysis;
using MorphologicalDisambiguation;
using NUnit.Framework;

namespace Test
{
    public class DummyDisambiguationTest
    {
        [Test]
        public void TestDisambiguation()
        {
            var fsm = new FsmMorphologicalAnalyzer();
            var corpus = new DisambiguationCorpus("../../../penntreebank.txt");
            var algorithm = new DummyDisambiguation();
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

            Assert.AreEqual(0.86, (correctRoot + 0.0) / corpus.NumberOfWords(), 0.01);
            Assert.AreEqual(0.70, (correctParse + 0.0) / corpus.NumberOfWords(), 0.01);
        }
    }
}