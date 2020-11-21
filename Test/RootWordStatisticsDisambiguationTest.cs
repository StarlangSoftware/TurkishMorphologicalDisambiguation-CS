using System.Globalization;
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
                    if (fsmParses[j].TransitionList().ToLower(new CultureInfo("tr-TR")).Equals(word.GetParse().ToString().ToLower(new CultureInfo("tr-TR"))))
                    {
                        correctParse++;
                        correctRoot++;
                    }
                    else
                    {
                        if (fsmParses[j].GetWord().Equals(word.GetParse().GetWord()))
                        {
                            correctRoot++;
                        }
                    }

                }
            }

            Assert.AreEqual(0.9676, (correctRoot + 0.0) / corpus.NumberOfWords(), 0.0001);
            Assert.AreEqual(0.8035, (correctParse + 0.0) / corpus.NumberOfWords(), 0.0001);
        }
    }
}