using System.Collections.Generic;
using Corpus;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using NGram;

namespace MorphologicalDisambiguation
{
    public class RootFirstDisambiguation : NaiveDisambiguation
    {
        protected NGram<Word> wordBiGramModel;
        protected NGram<Word> igBiGramModel;

        /**
         * <summary> The train method initially creates new NGrams; wordUniGramModel, wordBiGramModel, igUniGramModel, and igBiGramModel. It gets the
         * sentences from given corpus and gets each word as a DisambiguatedWord. Then, adds the word together with its part of speech
         * tags to the wordUniGramModel. It also gets the transition list of that word and adds it to the igUniGramModel.
         * <p/>
         * If there exists a next word in the sentence, it adds the current and next {@link DisambiguatedWord} to the wordBiGramModel with
         * their part of speech tags. It also adds them to the igBiGramModel with their transition lists.
         * <p/>
         * At the end, it calculates the NGram probabilities of both word and ig unigram models by using LaplaceSmoothing, and
         * both word and ig bigram models by using InterpolatedSmoothing.</summary>
         *
         * <param name="corpus">{@link DisambiguationCorpus} to train.</param>
         */
        public override void Train(DisambiguationCorpus corpus)
        {
            int i, j;
            Word[] words1 = new Word[1];
            Word[] igs1 = new Word[1];
            Word[] words2 = new Word[2];
            Word[] igs2 = new Word[2];
            wordUniGramModel = new NGram<Word>(1);
            wordBiGramModel = new NGram<Word>(2);
            igUniGramModel = new NGram<Word>(1);
            igBiGramModel = new NGram<Word>(2);
            for (i = 0; i < corpus.SentenceCount(); i++)
            {
                Sentence sentence = corpus.GetSentence(i);
                for (j = 0; j < sentence.WordCount(); j++)
                {
                    var word = (DisambiguatedWord) sentence.GetWord(j);
                    words1[0] = word.GetParse().GetWordWithPos();
                    wordUniGramModel.AddNGram(words1);
                    igs1[0] = new Word(word.GetParse().GetTransitionList());
                    igUniGramModel.AddNGram(igs1);
                    if (j + 1 < sentence.WordCount())
                    {
                        words2[0] = words1[0];
                        words2[1] = ((DisambiguatedWord) sentence.GetWord(j + 1)).GetParse().GetWordWithPos();
                        wordBiGramModel.AddNGram(words2);
                        igs2[0] = igs1[0];
                        igs2[1] = new Word(((DisambiguatedWord) sentence.GetWord(j + 1)).GetParse()
                            .GetTransitionList());
                        igBiGramModel.AddNGram(igs2);
                    }
                }
            }

            wordUniGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
            igUniGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
            wordBiGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
            igBiGramModel.CalculateNGramProbabilities(new LaplaceSmoothing<Word>());
        }

        /**
         * <summary> The getWordProbability method returns the probability of a word by using word bigram or unigram model.</summary>
         *
         * <param name="word">            Word to find the probability.</param>
         * <param name="correctFsmParses">FsmParse of given word which will be used for getting part of speech tags.</param>
         * <param name="index">           Index of FsmParse of which part of speech tag will be used to get the probability.</param>
         * <returns>The probability of the given word.</returns>
         */
        protected double GetWordProbability(Word word, List<FsmParse> correctFsmParses, int index)
        {
            if (index != 0 && correctFsmParses.Count == index)
            {
                return wordBiGramModel.GetProbability(correctFsmParses[index - 1].GetWordWithPos(), word);
            }

            return wordUniGramModel.GetProbability(word);
        }

        /**
         * <summary> The getIgProbability method returns the probability of a word by using ig bigram or unigram model.</summary>
         *
         * <param name="word">            Word to find the probability.</param>
         * <param name="correctFsmParses">FsmParse of given word which will be used for getting transition list.</param>
         * <param name="index">           Index of FsmParse of which transition list will be used to get the probability.</param>
         * <returns>The probability of the given word.</returns>
         */
        protected double GetIgProbability(Word word, List<FsmParse> correctFsmParses, int index)
        {
            if (index != 0 && correctFsmParses.Count == index)
            {
                return igBiGramModel.GetProbability(new Word(correctFsmParses[index - 1].GetTransitionList()),
                    word);
            }

            return igUniGramModel.GetProbability(word);
        }

        /**
         * <summary> The getBestRootWord method takes a {@link FsmParseList} as an input and loops through the list. It gets each word with its
         * part of speech tags as a new {@link Word} word and its transition list as a {@link Word} ig. Then, finds their corresponding
         * probabilities. At the end returns the word with the highest probability.</summary>
         *
         * <param name="fsmParseList">{@link FsmParseList} is used to get the part of speech tags and transition lists of words.</param>
         * <returns>The word with the highest probability.</returns>
         */
        protected Word GetBestRootWord(FsmParseList fsmParseList)
        {
            double bestProbability = int.MinValue;
            Word bestWord = null;
            for (var j = 0; j < fsmParseList.Size(); j++)
            {
                var word = fsmParseList.GetFsmParse(j).GetWordWithPos();
                var ig = new Word(fsmParseList.GetFsmParse(j).GetTransitionList());
                var wordProbability = wordUniGramModel.GetProbability(word);
                var igProbability = igUniGramModel.GetProbability(ig);
                var probability = wordProbability * igProbability;
                if (probability > bestProbability)
                {
                    bestWord = word;
                    bestProbability = probability;
                }
            }

            return bestWord;
        }

        /**
         * <summary> The getParseWithBestIgProbability gets each {@link FsmParse}'s transition list as a {@link Word} ig. Then, finds the corresponding
         * probability. At the end returns the parse with the highest ig probability.</summary>
         *
         * <param name="parseList">       {@link FsmParseList} is used to get the {@link FsmParse}.</param>
         * <param name="correctFsmParses">FsmParse is used to get the transition lists.</param>
         * <param name="index">           Index of FsmParse of which transition list will be used to get the probability.</param>
         * <returns>The parse with the highest probability.</returns>
         */
        protected FsmParse GetParseWithBestIgProbability(FsmParseList parseList, List<FsmParse> correctFsmParses,
            int index)
        {
            FsmParse bestParse = null;
            double bestProbability = int.MinValue;
            for (var j = 0; j < parseList.Size(); j++)
            {
                var ig = new Word(parseList.GetFsmParse(j).GetTransitionList());
                var probability = GetIgProbability(ig, correctFsmParses, index);
                if (probability > bestProbability)
                {
                    bestParse = parseList.GetFsmParse(j);
                    bestProbability = probability;
                }
            }

            return bestParse;
        }

        /**
         * <summary> The disambiguate method gets an array of fsmParses. Then loops through that parses and finds the most probable root
         * word and removes the other words which are identical to the most probable root word. At the end, gets the most probable parse
         * among the fsmParses and adds it to the correctFsmParses {@link ArrayList}.</summary>
         *
         * <param name="fsmParses">{@link FsmParseList} to disambiguate.</param>
         * <returns>correctFsmParses {@link ArrayList} which holds the most probable parses.</returns>
         */
        public override List<FsmParse> Disambiguate(FsmParseList[] fsmParses)
        {
            int i;
            var correctFsmParses = new List<FsmParse>();
            for (i = 0; i < fsmParses.Length; i++)
            {
                var bestWord = GetBestRootWord(fsmParses[i]);
                fsmParses[i].ReduceToParsesWithSameRootAndPos(bestWord);
                var bestParse = GetParseWithBestIgProbability(fsmParses[i], correctFsmParses, i);
                if (bestParse != null)
                {
                    correctFsmParses.Add(bestParse);
                }
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