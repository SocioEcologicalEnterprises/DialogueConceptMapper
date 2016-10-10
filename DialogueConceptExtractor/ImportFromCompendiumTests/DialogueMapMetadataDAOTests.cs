using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Neo4jClient;

using ImportFromCompendium.DataAccess;
using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendiumTests
{
    [TestClass]
    public class DialogueMapMetadataDAOTests
    {
        private const string GRAPH_DB_URL = @"http://localhost:7474/db/data";

        private GraphClient _client;
        private Guid _testMapId;
        private DialogueMapMetadata _dmMetadata;

        private DialogueMapMetadataDAO _testDAO;
        
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

            _testMapId = Guid.NewGuid();

            List<Person> testParticipants = new List<Person>() {
                new Person() { Name = "Test Participant 1", Email = "test1@participant.com" },
                new Person() { Name = "Test Participant 2", Email = "test2@participant.com" }
            };

            _dmMetadata = new DialogueMapMetadata() { Id = _testMapId, Name = "Test Dialogue Map", DateCreated = 20150401093830, Participants = testParticipants };

            _testDAO = new DialogueMapMetadataDAO(GRAPH_DB_URL);

        }

        [TestMethod]
        public void TestCreateDialogueMapMetadataAddsMetadataNode()
        {
            
            _testDAO.Create(_dmMetadata);

            var result = _client.Cypher
                            .Match("(d:Dialogue)")
                            .Where((DialogueMapMetadata d) => d.Id == _testMapId)
                            .ReturnDistinct(d => d.As<DialogueMapMetadata>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Dialogue Map", result.Name);

        }

        [TestMethod]
        public void TestCreateDialogueMapAddsTheFirstParticipantNodes()
        {
            _testDAO.Create(_dmMetadata);

            var result = _client.Cypher
                            .Match("(p:Person)")
                            .Where((Person p) => p.Email == "test1@participant.com")
                            .ReturnDistinct(p => p.As<Person>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Participant 1", result.Name);

        }

        [TestMethod]
        public void TestCreateLinksBetweenParticipantsAndDialogueMap()
        {
            _testDAO.Create(_dmMetadata);

            var result = _client.Cypher
                            .Match("(d:Dialogue)<-[:PARTICIPATED_IN]-(p:Person)")
                            .Where((DialogueMapMetadata d) => d.Id == _testMapId)
                            .Return(p => p.As<Person>())
                            .Results
                            .ToList();

            Assert.AreEqual(2, result.Count);
        }
    }
}
