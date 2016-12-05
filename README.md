# DialogueConceptMapper
Extracts concepts from dialogue mapping.

All the code is currently on the Dev branch. This is a very basic Proof of Concept that:

1. Reads some data from the XML output by Compendium's dialogue mapping tool.
2. Imports information about Concepts, People and Resources from the dialogue map output into a Neo4J Database.

Graphs can then be created by importing from Neo4J into Gephi using Gephi's (not quite finished) Neo4J importer.

See the [front page of the Wiki](https://github.com/SocioEcologicalEnterprises/DialogueConceptMapper/wiki) for a development road map and some pseudocode about how to implement the logic of version 0.2 (which is intended to emphasise the most commonly mentioned concepts).

