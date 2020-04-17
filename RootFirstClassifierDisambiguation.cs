using System;
using System.Collections.Generic;
using Classification.Attribute;
using Classification.Classifier;
using Classification.DataSet;
using Classification.Instance;
using Classification.Model;
using Classification.Parameter;
using MorphologicalAnalysis;
using Attribute = Classification.Attribute.Attribute;

namespace MorphologicalDisambiguation
{
    public class RootFirstClassifierDisambiguation : RootFirstDisambiguation
    {
        private Dictionary<string, Model> _models;
        private readonly Classifier _classifier;
        private readonly Parameter _parameters;

        /**
         * <summary> Constructor for setting the {@link Classifier} and {@link Parameter}.</summary>
         *
         * <param name="classifier">Type of the {@link Classifier}.</param>
         * <param name="parameters">{@link Parameter}s of the classifier.</param>
         */
        public RootFirstClassifierDisambiguation(Classifier classifier, Parameter parameters)
        {
            this._classifier = classifier;
            this._parameters = parameters;
        }

        /**
         * <summary> The createDataDefinition method creates an {@link ArrayList} of {@link AttributeType}s and adds 2 times BINARY AttributeType
         * for each element of the {@link InflectionalGroup}.</summary>
         *
         * <returns>A new data definition with the attributeTypes.</returns>
         */
        private DataDefinition CreateDataDefinition()
        {
            var attributeTypes = new List<AttributeType>();
            for (var i = 0; i < 2 * 130; i++)
            {
                attributeTypes.Add(AttributeType.BINARY);
            }

            return new DataDefinition(attributeTypes);
        }

        /**
         * <summary> The addAttributes method takes an {@link InflectionalGroup} ig and an {@link ArrayList} of attributes. If the given
         * ig contains any of the morphological tags of InflectionalGroup, it adds a new {@link BinaryAttribute} with the value of
         * true to the attributes ArrayList, if not it adds a new {@link BinaryAttribute} with the value of false.</summary>
         *
         * <param name="ig">        InflectionalGroup to check the morphological tags.</param>
         * <param name="attributes">ArrayList of attributes.</param>
         */
        private void AddAttributes(InflectionalGroup ig, List<Attribute> attributes)
        {
            for (var k = 0; k < 130; k++)
            {
                if (ig.ContainsTag(InflectionalGroup.MorphoTags[k]))
                {
                    attributes.Add(new BinaryAttribute(true));
                }
                else
                {
                    attributes.Add(new BinaryAttribute(false));
                }
            }
        }

        /**
         * <summary> The classificationProblem method takes a {@link String} input and parses it. If the given {@link MorphologicalParse}
         * contains the parsed String, it directly returns that String, if not it returns null.</summary>
         *
         * <param name="disambiguationProblem">String input to be parsed.</param>
         * <param name="morphologicalParse">   MorphologicalParse input.</param>
         * <returns>If the given {MorphologicalParse contains the String, it directly returns that String, if not it returns null.</returns>
         */
        private string ClassificationProblem(string disambiguationProblem, MorphologicalParse morphologicalParse)
        {
            var parses = disambiguationProblem.Split("$");
            foreach (var parse in parses)
            {
                if (morphologicalParse.ToString().Contains(parse))
                {
                    return parse;
                }
            }

            return null;
        }

        /**
         * <summary> The train method gets sentences from given {@link DisambiguationCorpus}, then perform morphological analyses for each
         * word of a sentence and gets a {@link FsmParseList} at each time and removes the other words which are identical to the current word
         * and part of speech tags.
         * <p/>
         * If the size of the {@link FsmParseList} greater than 1,  it removes the prefixes and suffixes from the {@link FsmParseList} and
         * evaluates it as disambiguationProblem  String. If this String is already placed in Dataset, it gets its value, else
         * put it to the DataSet as a new key.
         * <p/>
         * Apart from that, it also gets two previous InflectionalGroups and finds out their class labels, and adds them to the Dataset
         * as a new {@link Instance}.</summary>
         *
         * <param name="corpus">{@link DisambiguationCorpus} to train.</param>
         */
        public override void Train(DisambiguationCorpus corpus)
        {
            base.Train(corpus);
            int i;
            var dataSets = new Dictionary<string, DataSet>();
            var dataDefinition = CreateDataDefinition();
            var fsm = new FsmMorphologicalAnalyzer();
            for (i = 0; i < corpus.SentenceCount(); i++)
            {
                var sentence = corpus.GetSentence(i);
                int j;
                for (j = 2; j < sentence.WordCount(); j++)
                {
                    var parseList = fsm.MorphologicalAnalysis(sentence.GetWord(j).GetName());
                    parseList.ReduceToParsesWithSameRootAndPos(((DisambiguatedWord) sentence.GetWord(j)).GetParse()
                        .GetWordWithPos());
                    if (parseList.Size() > 1)
                    {
                        String disambiguationProblem = parseList.ParsesWithoutPrefixAndSuffix();
                        DataSet dataSet;
                        if (dataSets.ContainsKey(disambiguationProblem))
                        {
                            dataSet = dataSets[disambiguationProblem];
                        }
                        else
                        {
                            dataSet = new DataSet(dataDefinition);
                            dataSets[disambiguationProblem] = dataSet;
                        }

                        var attributes = new List<Attribute>();
                        var previousIg = ((DisambiguatedWord) sentence.GetWord(j - 2)).GetParse().LastInflectionalGroup();
                        AddAttributes(previousIg, attributes);
                        var ig = ((DisambiguatedWord) sentence.GetWord(j - 1)).GetParse().LastInflectionalGroup();
                        AddAttributes(ig, attributes);
                        var classLabel = ClassificationProblem(disambiguationProblem,
                            ((DisambiguatedWord) sentence.GetWord(j)).GetParse());
                        if (classLabel != null)
                        {
                            dataSet.AddInstance(new Instance(classLabel, attributes));
                        }
                    }
                }
            }

            _models = new Dictionary<string, Model>();
            i = 0;
            foreach (var problem in dataSets.Keys)
            {
                Classifier currentClassifier;
                if (dataSets[problem].SampleSize() >= 10)
                {
                    currentClassifier = _classifier;
                }
                else
                {
                    currentClassifier = new Dummy();
                }

                currentClassifier.Train(dataSets[problem].GetInstanceList(), _parameters);
                _models[problem] = currentClassifier.GetModel();
                i++;
            }
        }

        /**
         * <summary> The disambiguate method gets an array of fsmParses. Then loops through these parses and finds the most probable root
         * word and removes the other words which are identical to the most probable root word. For the first two items and
         * the last item, it gets the most probable ig parse among the fsmParses and adds it to the correctFsmParses {@link ArrayList} and returns it.
         * For the other cases, it gets the classification model,  considering the previous two ig it performs a prediction
         * and at the end returns the correctFsmParses that holds the best parses.</summary>
         *
         * <param name="fsmParses">{@link FsmParseList} to disambiguate.</param>
         * <returns>The correctFsmParses that holds the best parses.</returns>
         */
        public override List<FsmParse> Disambiguate(FsmParseList[] fsmParses)
        {
            int i;
            FsmParse bestParse = null;
            var correctFsmParses = new List<FsmParse>();
            for (i = 0; i < fsmParses.Length; i++)
            {
                var bestWord = GetBestRootWord(fsmParses[i]);
                fsmParses[i].ReduceToParsesWithSameRootAndPos(bestWord);
                if (i < 2 || i != correctFsmParses.Count)
                {
                    bestParse = GetParseWithBestIgProbability(fsmParses[i], correctFsmParses, i);
                }
                else
                {
                    if (fsmParses[i].Size() == 0)
                    {
                        bestParse = null;
                    }
                    else
                    {
                        if (fsmParses[i].Size() == 1)
                        {
                            bestParse = fsmParses[i].GetFsmParse(0);
                        }
                        else
                        {
                            var disambiguationProblem = fsmParses[i].ParsesWithoutPrefixAndSuffix();
                            if (_models.ContainsKey(disambiguationProblem))
                            {
                                var model = _models[disambiguationProblem];
                                var attributes = new List<Attribute>();
                                var previousIg = correctFsmParses[i - 2].LastInflectionalGroup();
                                AddAttributes(previousIg, attributes);
                                var ig = correctFsmParses[i - 1].LastInflectionalGroup();
                                AddAttributes(ig, attributes);
                                var predictedParse = model.Predict(new Instance("", attributes));
                                for (var j = 0; j < fsmParses[i].Size(); j++)
                                {
                                    if (fsmParses[i].GetFsmParse(j).TransitionList().Contains(predictedParse))
                                    {
                                        bestParse = fsmParses[i].GetFsmParse(j);
                                        break;
                                    }
                                }

                                if (bestParse == null)
                                {
                                    bestParse = GetParseWithBestIgProbability(fsmParses[i], correctFsmParses, i);
                                }
                            }
                            else
                            {
                                bestParse = GetParseWithBestIgProbability(fsmParses[i], correctFsmParses, i);
                            }
                        }
                    }
                }

                if (bestParse != null)
                {
                    correctFsmParses.Add(bestParse);
                }
            }

            return correctFsmParses;
        }
    }
}