For Developers
============

You can also see [Java](https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation), [Python](https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation-Py), or [C++](https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation-CPP) repository.

Detailed Description
============

+ [Creating MorphologicalDisambiguator](#creating-morphologicaldisambiguator)
+ [Training MorphologicalDisambiguator](#training-morphologicaldisambiguator)
+ [Sentence Disambiguation](#sentence-disambiguation)

## Creating MorphologicalDisambiguator 

MorphologicalDisambiguator provides Turkish morphological disambiguation. There are possible disambiguation techniques. Depending on the technique used, disambiguator can be instantiated as follows:

* Using `RootFirstDisambiguation`, the one that chooses only the root amongst the given analyses

        MorphologicalDisambiguator morphologicalDisambiguator = new RootFirstDisambiguation();

* Using `LongestRootFirstDisambiguation`, the one that chooses the root that is the most frequently used amongst the given analyses

        MorphologicalDisambiguator morphologicalDisambiguator = new LongestRootFirstDisambiguation();

* Using `HmmDisambiguation`, the one that chooses using an Hmm-based algorithm
        
        MorphologicalDisambiguator morphologicalDisambiguator = new HmmDisambiguation();

* Using `DummyDisambiguation`, the one that chooses a random one amongst the given analyses 
     
        MorphologicalDisambiguator morphologicalDisambiguator = new DummyDisambiguation();
    

## Training MorphologicalDisambiguator

To train the disambiguator, an instance of `DisambiguationCorpus` object is needed. This can be instantiated and the disambiguator can be trained and saved as follows:

    DisambiguationCorpus corpus = new DisambiguationCorpus("penn_treebank.txt");
    morphologicalDisambiguator.Train(corpus);
    morphologicalDisambiguator.SaveModel();
    
      
## Sentence Disambiguation

To disambiguate a sentence, a `FsmMorphologicalAnalyzer` instance is required. This can be created as below, further information can be found [here](https://github.com/olcaytaner/MorphologicalAnalysis/blob/master/README.md#creating-fsmmorphologicalanalyzer).

    FsmMorphologicalAnalyzer fsm = new FsmMorphologicalAnalyzer();
    
A sentence can be disambiguated as follows: 
    
    Sentence sentence = new Sentence("Yarın doktora gidecekler");
    FsmParseList[] fsmParseList = fsm.RobustMorphologicalAnalysis(sentence);
    Console.WriteLine("All parses");
    Console.WriteLine("--------------------------");
    for(int i = 0; i < fsmParseList.Length; i++){
        Console.WriteLine(fsmParseList[i]);
    }
    List<FsmParse> candidateParses = morphologicalDisambiguator.Disambiguate(fsmParseList);
    Console.WriteLine("Parses after disambiguation");
    Console.WriteLine("--------------------------");
    for(int i = 0; i < candidateParses.Size(); i++){
        Console.WriteLine(candidateParses[i]);
    }

Output

    
    All parses
    --------------------------
    yar+NOUN+A3SG+P2SG+NOM
    yar+NOUN+A3SG+PNON+GEN
    yar+VERB+POS+IMP+A2PL
    yarı+NOUN+A3SG+P2SG+NOM
    yarın+NOUN+A3SG+PNON+NOM
    
    doktor+NOUN+A3SG+PNON+DAT
    doktora+NOUN+A3SG+PNON+NOM
    
    git+VERB+POS+FUT+A3PL
    git+VERB+POS^DB+NOUN+FUTPART+A3PL+PNON+NOM
    
    Parses after disambiguation
    --------------------------
    yarın+NOUN+A3SG+PNON+NOM
    doktor+NOUN+A3SG+PNON+DAT
    git+VERB+POS+FUT+A3PL
