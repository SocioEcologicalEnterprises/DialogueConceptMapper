using System;
using System.Collections.Generic;

namespace ImportFromCompendium.BusinessLogic
{
    public class DialogueMapMetadata : Node
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long DateCreated { get; set; }
        public List<Person> Participants { get; set; }

    }
}
