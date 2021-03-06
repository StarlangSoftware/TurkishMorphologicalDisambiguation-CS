Morphological Disambiguation
============

Morphological disambiguation is the problem of selecting accurate morphological parse of a word given its possible parses. These parses are generated by a morphological analyzer. In morphologically rich languages like Turkish, the number of possible parses for a given word is generally more than one. Each parse is considered as a different interpretation of a single word. Each interpretation consists of a root word and sequence of inflectional and derivational suffixes. The following table illustrates different interpretations of the word ‘‘üzerine’’.

üzer+Noun+A3sg+P3sg+Dat  
üzer+Noun+A3sg+P2sg+Dat  
üz+Verb+Pos+Aor+^DB+Adj+Zero+^DB+Noun+Zero+A3sg+P3sg+Dat  
üz+Verb+Pos+Aor+^DB+Adj+Zero+^DB+Noun+Zero+A3sg+P2sg+Dat

As seen above, the first two parses share the same root but different suffix sequences. Similarly, the last two parses also share the same root, however sequence of morphemes are different. Given a parse such as

üz+Verb+Pos+Aor+^DB+Adj+Zero+^DB+Noun+Zero+A3sg+P3sg+Dat

each item is separated by ‘‘+’’ is a morphological feature such as Pos or Aor. Inflectional groups are identified as sequence of morphological features separated by derivational boundaries ^DB. The sequence of inflectional groups forms the term tag. Root word plus tag is named as word form.  So, a word form is defined as follows:

IGroot+IG<sub>1</sub>+^DB+IG<sub>2</sub>+^DB+...+^DB+IG<sub>n</sub>

Then the morphological disambiguation problem can be defined as follows: For a given sentence represented by a sequence of words W = w<sub>1</sub><sup>n</sup> = w<sub>1</sub>, w<sub>2</sub>, ..., w<sub>n</sub>, determine the sequence of parses T = t<sub>1</sub><sup>n</sup> = t<sub>1</sub>, t<sub>2</sub>, ..., t<sub>n</sub>; where t<sub>i</sub> represents the correct parse of the word w<sub>i</sub>.

For Developers
============

You can also see [Java](https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation), [Python](https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation-Py), [Cython](https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation-Cy), or [C++](https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation-CPP) repository.

## Requirements

* C# Editor
* [Git](#git)

### Git

Install the [latest version of Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

## Download Code

In order to work on code, create a fork from GitHub page. 
Use Git for cloning the code to your local or below line for Ubuntu:

	git clone <your-fork-git-link>

A directory called TurkishMorphologicalDisambiguation-CS will be created. Or you can use below link for exploring the code:

	git clone https://github.com/starlangsoftware/TurkishMorphologicalDisambiguation-CS.git

## Open project with Rider IDE

To import projects from Git with version control:

* Open Rider IDE, select Get From Version Control.

* In the Import window, click URL tab and paste github URL.

* Click open as Project.

Result: The imported project is listed in the Project Explorer view and files are loaded.


## Compile

**From IDE**

After being done with the downloading and opening project, select **Build Solution** option from **Build** menu. After compilation process, user can run TurkishMorphologicalDisambiguation-CS.

Detailed Description
============

+ [Creating MorphologicalDisambiguator](#creating-morphologicaldisambiguator)
+ [Training MorphologicalDisambiguator](#training-morphologicaldisambiguator)
+ [Sentence Disambiguation](#sentence-disambiguation)

## Creating MorphologicalDisambiguator 

MorphologicalDisambiguator provides Turkish morphological disambiguation. There are possible disambiguation techniques. Depending on the technique used, disambiguator can be instantiated as follows:

* Using `RootFirstDisambiguation`, the one that chooses only the root amongst the given analyses

        morphologicalDisambiguator = new RootFirstDisambiguation();

* Using `RootWordStatisticsDisambiguation`, the one that chooses the root that is the most frequently used amongst the given analyses

        morphologicalDisambiguator = new RootWordStatisticsDisambiguation();

* Using `LongestRootFirstDisambiguation`, the one that chooses the longest root among the given roots
        
        morphologicalDisambiguator = new LongestRootFirstDisambiguation();

* Using `HmmDisambiguation`, the one that chooses using an Hmm-based algorithm
        
        morphologicalDisambiguator = new HmmDisambiguation();

* Using `DummyDisambiguation`, the one that chooses a random one amongst the given analyses 
     
        morphologicalDisambiguator = new DummyDisambiguation();
    

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

# Cite

	@InProceedings{gorgunyildiz12,
	author="G{\"o}rg{\"u}n, Onur
	and Yildiz, Olcay Taner",
	editor="Gelenbe, Erol
	and Lent, Ricardo
	and Sakellari, Georgia",
	title="A Novel Approach to Morphological Disambiguation for Turkish",
	booktitle="Computer and Information Sciences II",
	year="2012",
	publisher="Springer London",
	address="London",
	pages="77--83",
	isbn="978-1-4471-2155-8"
	}
