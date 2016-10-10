using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendium.DataAccess
{
    public class StatementDAO : BaseDAO
    {
        public StatementDAO(string graphDBUrl) : base(graphDBUrl) { }

        public void Create(Statement statement, Guid mapId)
        {
            try
            {
                _client.Cypher
                     .Create("(s:Statement {Id : {id}, Label : {label}, Type: {type}, NodeType: {nodeType}, DialogueName: {dialogueName} })")
                     .WithParams(new { id = statement.Id, label = statement.Label, type = statement.Type.ToString(), nodeType = statement.NodeType, dialogueName = statement.DialogueName })
                     .ExecuteWithoutResults();

                _client.Cypher
                    .Match("(s:Statement)", "(d:Dialogue)")
                    .Where((Statement s) => s.Id == statement.Id)
                    .AndWhere((DialogueMapMetadata d) => d.Id == mapId)
                    .Create("s-[:PART_OF]->d")
                    .ExecuteWithoutResults();

                foreach(Concept concept in statement.RelatedConcepts)
                {
                    _client.Cypher
                        .Match("(s:Statement)", "(c:Concept)")
                        .Where((Statement s) => s.Id == statement.Id)
                        .AndWhere((Concept c) => c.Name == concept.Name)
                        .Create("s-[:REFERS_TO]->c")
                        .ExecuteWithoutResults();
                }

                foreach (Person person in statement.RelatedPeople)
                {
                    _client.Cypher
                        .Match("(s:Statement)", "(p:Person)")
                        .Where((Statement s) => s.Id == statement.Id)
                        .AndWhere((Person p) => p.Name == person.Name)
                        .Create("s-[:REFERS_TO]->p")
                        .ExecuteWithoutResults();
                }

                foreach (Resource resource in statement.RelatedResources)
                {
                    _client.Cypher
                        .Match("(s:Statement)", "(r:Resource)")
                        .Where((Statement s) => s.Id == statement.Id)
                        .AndWhere((Resource r) => r.Name == resource.Name)
                        .Create("s-[:REFERS_TO]->r")
                        .ExecuteWithoutResults();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void LinkStatements(StatementLink statementLink)
        {
            try
            {
                _client.Cypher
                    .Match("(s1:Statement)", "(s2:Statement)")
                    .Where((Statement s1) => s1.Id == statementLink.FromId)
                    .AndWhere((Statement s2) => s2.Id == statementLink.ToId)
                    .Create("s1-[:IN_RESPONSE_TO]->s2")
                    .ExecuteWithoutResults();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
