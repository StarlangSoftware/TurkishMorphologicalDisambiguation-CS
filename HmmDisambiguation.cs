using System.Collections.Generic;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using NGram;

namespace MorphologicalDisambiguation
{
    public class HmmDisambiguation : NaiveDisambiguation
    {
        protected NGram<Word> wordBiGramModel;
        protected NGram<Word> igBiGramModel;

        /**
         * <summary> The train method gets sentences from given {@link DisambiguationCorpus} and both word and the next word of that sentence at each iteration.
         * Then, adds these words together with their part of speech tags to word unigram and bigram models. It also adds the last inflectional group of
         * word to the ig unigram and bigram models.
         * <p/>
         * At the end, it calculates the NGram probabilities of both word and ig unigram models by using LaplaceSmoothing, and
         * both word and ig bigram models by using InterpolatedSmoothing.</summary>
         *
         * <param name="corpus">{@link DisambiguationCorpus} to train.</param>
         */
        public override void Train(DisambiguationCorpus corpus)
        {
            int i, j;
            var words1 = new Word[1];
            var igs1 = new Word[1];
            var words2 = new Word[2];
            var igs2 = new Word[2];
            wordUniGramModel = new NGram<Word>(1);
            igUniGramModel = new NGram<Word>(1);
            wordBiGramModel = new NGram<Word>(2);
            igBiGramModel = new NGram<Word>(2);
            for (i = 0; i < corpus.SentenceCount(); i++)
            {
                var sentence = corpus.GetSentence(i);
                for (j = 0; j < sentence.WordCount() - 1; j++)
                {
                    var word = (DisambiguatedWord) sentence.GetWord(j);
                    var nextWord = (DisambiguatedWord) sentence.GetWord(j + 1);
                    words2[0] = word.GetParse().GetWordWithPos();
                    words1[0] = words2[0];
                    words2[1] = nextWord.GetParse().GetWordWithPos();
                    wordUniGramModel.AddNGram(words1);
                    wordBiGramModel.AddNGram(words2);
                    int k;
                    for (k = 0; k < nextWord.GetParse().Size(); k++)
                    {
                        igs2[0] = new Word(word.GetParse().GetLastInflectionalGroup().ToString());
                        igs2[1] = new Word(nextWord.GetParse().GetInflectionalGroup(k).ToString());
                        igBiGramModel.AddNGram(igs2);
                        igs1[0] = igs2[1];
                        igUniGramModel.AddNGram(igs1);
                    }
                }
            }

            wordUniGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
            igUniGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
            wordBiGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
            igBiGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
        }

        /**
         * <summary> The disambiguate method takes {@link FsmParseList} as an input and gets one word with its part of speech tags, then gets its probability
         * from word unigram model. It also gets ig and its probability. Then, hold the logarithmic value of  the product of these probabilities in an array.
         * Also by taking into consideration the parses of these word it recalculates the probabilities and returns these parses.</summary>
         *
         * <param name="fsmParses">{@link FsmParseList} to disambiguate.</param>
         * <returns>ArrayList of FsmParses.</returns>
         */
        public override List<FsmParse> Disambiguate(FsmParseList[] fsmParses)
        {
            int i, j;
            int bestIndex;
            double probability, bestProbability;
            Word w1;
            Word ig1;
            if (fsmParses.Length == 0)
            {
                return null;
            }

            for (i = 0; i < fsmParses.Length; i++)
            {
                if (fsmParses[i].Size() == 0)
                {
                    return null;
                }
            }

            var correctFsmParses = new List<FsmParse>();
            var probabilities = new double[fsmParses.Length][];
            var best = new int[fsmParses.Length][];
            for (i = 0; i < fsmParses.Length; i++)
            {
                probabilities[i] = new double[fsmParses[i].Size()];
                best[i] = new int[fsmParses[i].Size()];
            }

            for (i = 0; i < fsmParses[0].Size(); i++)
            {
                var currentParse = fsmParses[0].GetFsmParse(i);
                w1 = currentParse.GetWordWithPos();
                probability = wordUniGramModel.GetProbability(w1);
                for (j = 0; j < currentParse.Size(); j++)
                {
                    ig1 = new Word(currentParse.GetInflectionalGroup(j).ToString());
                    probability *= igUniGramModel.GetProbability(ig1);
                }

                probabilities[0][i] = System.Math.Log(probability);
            }

            for (i = 1; i < fsmParses.Length; i++)
            {
                for (j = 0; j < fsmParses[i].Size(); j++)
                {
                    bestProbability = int.MinValue;
                    bestIndex = -1;
                    var currentParse = fsmParses[i].GetFsmParse(j);
                    int k;
                    for (k = 0; k < fsmParses[i - 1].Size(); k++)
                    {
                        var previousParse = fsmParses[i - 1].GetFsmParse(k);
                        w1 = previousParse.GetWordWithPos();
                        var w2 = currentParse.GetWordWithPos();
                        probability = probabilities[i - 1][k] + System.Math.Log(wordBiGramModel.GetProbability(w1, w2));
                        int t;
                        for (t = 0; t < fsmParses[i].GetFsmParse(j).Size(); t++)
                        {
                            ig1 = new Word(previousParse.LastInflectionalGroup().ToString());
                            var ig2 = new Word(currentParse.GetInflectionalGroup(t).ToString());
                            probability += System.Math.Log(igBiGramModel.GetProbability(ig1, ig2));
                        }

                        if (probability > bestProbability)
                        {
                            bestIndex = k;
                            bestProbability = probability;
                        }
                    }

                    probabilities[i][j] = bestProbability;
                    best[i][j] = bestIndex;
                }
            }

            bestProbability = int.MinValue;
            bestIndex = -1;
            for (i = 0; i < fsmParses[fsmParses.Length - 1].Size(); i++)
            {
                if (probabilities[fsmParses.Length - 1][i] > bestProbability)
                {
                    bestProbability = probabilities[fsmParses.Length - 1][i];
                    bestIndex = i;
                }
            }

            if (bestIndex == -1)
            {
                return null;
            }

            correctFsmParses.Add(fsmParses[fsmParses.Length - 1].GetFsmParse(bestIndex));
            for (i = fsmParses.Length - 2; i >= 0; i--)
            {
                bestIndex = best[i + 1][bestIndex];
                if (bestIndex == -1)
                {
                    return null;
                }

                correctFsmParses.Insert(0, fsmParses[i].GetFsmParse(bestIndex));
            }

            return correctFsmParses;
        }

        public override void SaveModel()
        {
        }

        public override void LoadModel()
        {
        }
    }
}