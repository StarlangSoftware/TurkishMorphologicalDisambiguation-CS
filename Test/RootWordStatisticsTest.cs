using MorphologicalAnalysis;
using MorphologicalDisambiguation;
using NUnit.Framework;

namespace Test
{
    public class RootWordStatisticsTest
    {
        [Test]
        public void TestRootWordStatistics()
        {
            var fsm = new FsmMorphologicalAnalyzer();
            var rootWordStatistics = new RootWordStatistics("penntreebank_statistics.txt");
            Assert.True(rootWordStatistics.ContainsKey("yasasını"));
            Assert.True(rootWordStatistics.ContainsKey("yapılandırıyorlar"));
            Assert.True(rootWordStatistics.ContainsKey("çöküşten"));
            Assert.AreEqual("yasa", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("yasasını"), 0.0));
            Assert.AreEqual("karşılaş", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("karşılaşabilir"), 0.0));
            Assert.Null(rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("karşılaşabilir"), 0.7));
            Assert.AreEqual("anlat", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("anlattı"), 0.0));
            Assert.Null(rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("anlattı"), 0.9));
            Assert.AreEqual("ver", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("vermesini"), 0.0));
            Assert.Null(rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("vermesini"), 0.9));
        }
    }
}