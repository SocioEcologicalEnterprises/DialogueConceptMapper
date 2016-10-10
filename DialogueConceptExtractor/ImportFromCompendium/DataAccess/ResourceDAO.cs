using System;
using System.Linq;
using System.Collections.Generic;
using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendium.DataAccess
{
    public class ResourceDAO : BaseDAO
    {
        public ResourceDAO(string graphDBUrl) : base(graphDBUrl) { }

        public void Create(Resource resource)
        {
            try
            {
                _client.Cypher
                     .Create("(r:Resource {resource})")
                     .WithParam("resource", resource)
                     .ExecuteWithoutResults();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public List<Resource> GetResources()
        {
            return _client.Cypher
                .Match("(r:Resource)")
                .Return(r => r.As<Resource>())
                .Results
                .ToList<Resource>();
        }
    }
}
