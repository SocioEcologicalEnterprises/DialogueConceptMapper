using System;
using System.Collections.Generic;
using System.Linq;

using Neo4jClient;

using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendium.DataAccess
{
    public class PersonDAO : BaseDAO
    {
        public PersonDAO(string graphDBUrl) : base(graphDBUrl)
        {
            
        }

        public void Create(Person person)
        {
            try 
            {
                _client.Cypher
                    .Merge("(p:Person { Email : {email} })")
                    .OnCreate()
                    .Set("p.Name = {name}")
                    .WithParams(new { email = person.Email, name = person.Name })
                    .ExecuteWithoutResults();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public List<Person> GetPeople()
        {
            return _client.Cypher
                .Match("(p:Person)")
                .Return(p => p.As<Person>())
                .Results
                .ToList<Person>();
        }
    }
}
