using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

using Neo4jClient;

using ImportFromCompendium.DataAccess;
using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendiumTests
{
    [TestClass]
    public class ResourceDAOTests
    {

        private const string GRAPH_DB_URL = @"http://localhost:7474/db/data";

        private GraphClient _client;

        private ResourceDAO _testDAO;

        [TestInitialize]
        public void SetUp()
        {
            _client = new GraphClient(new Uri(GRAPH_DB_URL));
            _client.Connect();

            _client.Cypher
                .Match("(n)")
                .OptionalMatch("(n)-[r]-()")
                .Delete("n, r")
                .ExecuteWithoutResults();

            _testDAO = new ResourceDAO(GRAPH_DB_URL);

        }

        [TestMethod]
        public void ResourceCreatedTest()
        {
            _testDAO.Create(new Resource() { Name = "Test Resource" });

            var result = _client.Cypher
                            .Match("(r:Resource)")
                            .Where((Resource r) => r.Name == "Test Resource")
                            .ReturnDistinct(r => r.As<Resource>())
                            .Results
                            .FirstOrDefault();

            Assert.IsNotNull(result);

        }


        [TestMethod]
        public void ResourceCreatedWithNodeTypeResource()
        {
            _testDAO.Create(new Resource() { Name = "Test Resource", NodeType= "Resource" });

            var result = _client.Cypher
                            .Match("(r:Resource)")
                            .Where((Resource r) => r.Name == "Test Resource")
                            .ReturnDistinct(r => r.As<Resource>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Resource", result.NodeType);

        }



        [TestMethod]
        public void ReadResourceTest()
        {
            _testDAO.Create(new Resource() { Name = "Test Resource" });

            List<Resource> result = _testDAO.GetResources();

            Assert.AreEqual("Test Resource", result[0].Name);

        }

        [TestMethod]
        public void ReadMultipleConceptsTest()
        {
            _testDAO.Create(new Resource() { Name = "Test Resource 1" });
            _testDAO.Create(new Resource() { Name = "Test Resource 2" });

            List<Resource> result = _testDAO.GetResources();

            Assert.AreEqual("Test Resource 2", result[1].Name);
        }

    }
}
