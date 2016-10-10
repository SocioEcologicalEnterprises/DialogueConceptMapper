using System;

using Neo4jClient;

namespace ImportFromCompendium.DataAccess
{
    public abstract class BaseDAO
    {
        protected GraphClient _client;

        protected string _graphDBUrl;

        protected BaseDAO(string graphDBUrl)
        {
            _graphDBUrl = graphDBUrl;
            _client = new GraphClient(new Uri(graphDBUrl));
            _client.Connect();
        }
        
    }
}
