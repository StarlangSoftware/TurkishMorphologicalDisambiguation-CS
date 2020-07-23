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
            Assert.True(rootWordStatistics.ContainsKey("it$iti$itici"));
            Assert.True(rootWordStatistics.ContainsKey("yas$yasa$yasama"));
            Assert.True(rootWordStatistics.ContainsKey("tutuk$tutukla"));
            Assert.AreEqual("çık", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("çıkma"), 0.0));
            Assert.AreEqual("danışman", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("danışman"), 0.0));
            Assert.Null(rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("danışman"), 0.7));
            Assert.AreEqual("görüşme", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("görüşme"), 0.0));
            Assert.Null(rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("görüşme"), 0.7));
            Assert.AreEqual("anlaş", rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("anlaşma"), 0.0));
            Assert.Null(rootWordStatistics.BestRootWord(fsm.MorphologicalAnalysis("anlaşma"), 0.7));
        }
    }
}