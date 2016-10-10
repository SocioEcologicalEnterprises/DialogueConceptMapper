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
    public class ConceptDAOTests
    {
        private const string GRAPH_DB_URL = @"http://localhost:7474/db/data";

        private GraphClient _client;

        private ConceptDAO _testDAO;

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

            _testDAO = new ConceptDAO(GRAPH_DB_URL);

        }

        [TestMethod]
        public void ConceptCreatedTest()
        {
            _testDAO.Create(new Concept() { Name = "Test Concept" });

            var result = _client.Cypher
                            .Match("(c:Concept)")
                            .Where((Concept c) => c.Name == "Test Concept")
                            .ReturnDistinct(c => c.As<Concept>())
                            .Results
                            .FirstOrDefault();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ConceptCreatedWithNodeType()
        {
            _testDAO.Create(new Concept() { Name = "Test Concept", NodeType = "Concept" });

            var result = _client.Cypher
                .Match("(c:Concept)")
                .Where((Concept c) => c.Name == "Test Concept")
                .ReturnDistinct(c => c.As<Concept>())
                .Results
                .ToList<Concept>();

            Assert.AreEqual("Concept", result[0].NodeType);

        }



        [TestMethod]
        public void ReadConceptTest()
        {
            _testDAO.Create(new Concept() { Name = "Test Concept" });

            List <Concept> result = _testDAO.GetConcepts();

            Assert.AreEqual("Test Concept", result[0].Name);

        }

        [TestMethod]
        public void ReadMultipleConceptsTest()
        {
            _testDAO.Create(new Concept() { Name = "Test Concept 1" });
            _testDAO.Create(new Concept() { Name = "Test Concept 2" });

            List<Concept> result = _testDAO.GetConcepts();

            Assert.AreEqual("Test Concept 2", result[1].Name);
        }

    }
}
