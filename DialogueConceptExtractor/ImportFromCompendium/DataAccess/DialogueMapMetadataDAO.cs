using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendium.DataAccess
{
    public class DialogueMapMetadataDAO : BaseDAO
    {

        public DialogueMapMetadataDAO(string graphDBUrl) : base(graphDBUrl)
        {
        }

        public void Create(DialogueMapMetadata dmMetadata)
        {
            try
            {
                _client.Cypher
                    .Create("(d:Dialogue {Id:{id}, Name:{name}})")
                    .WithParams(new { id = dmMetadata.Id, name = dmMetadata.Name })
                    .ExecuteWithoutResults();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            PersonDAO personDAO = new PersonDAO(_graphDBUrl);
            foreach(Person particpant in dmMetadata.Participants)
            {
                personDAO.Create(particpant);

                _client.Cypher
                    .Match("(d:Dialogue)", "(p:Person)")
                    .Where((DialogueMapMetadata d) => d.Id == dmMetadata.Id)
                    .AndWhere((Person p) => p.Email == particpant.Email)
                    .Create("d <-[:PARTICIPATED_IN]-p")
                    .ExecuteWithoutResults();

            }

        }
    }
}
