using System;
using System.Linq;
using System.Collections.Generic;
using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendium.DataAccess
{
    public class ConceptDAO : BaseDAO
    {
        public ConceptDAO(string graphDBUrl) : base(graphDBUrl) { }

        public void Create(Concept concept)
        {
            try
            {
               _client.Cypher
                    .Create("(c:Concept {concept})")
                    .WithParam("concept", concept)
                    .ExecuteWithoutResults();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public List<Concept> GetConcepts()
        {
            return _client.Cypher
                .Match("(c:Concept)")
                .Return( c => c.As<Concept>())
                .Results
                .ToList<Concept>();

        }
    }
}
