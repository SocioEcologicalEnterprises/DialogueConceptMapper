using System;
using System.Collections.Generic;

namespace ImportFromCompendium.BusinessLogic
{
    public class Statement : Node
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public string DialogueName { get; set; }

        public StatementType Type { get; set; }
 
        public List<Concept> RelatedConcepts { get; set; }
        public List<Person> RelatedPeople { get; set; }
        public List<Resource> RelatedResources { get; set; }

    }
}
