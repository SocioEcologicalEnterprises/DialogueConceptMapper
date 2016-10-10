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
    public class StatementDAOTests
    {
        private const string GRAPH_DB_URL = @"http://localhost:7474/db/data";

        private GraphClient _client;

        private StatementDAO _statementDAO;
        private Statement _testStatement;

        private Guid _testMetadataGuid;

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

            // create a test concept to link to

            _client.Cypher
                    .Create("(c:Concept {Name : {name} })")
                    .WithParam("name", "Test Concept")
                    .ExecuteWithoutResults();

            // create a test person to link to

            _client.Cypher
                    .Create("(p:Person {Name : {name} })")
                    .WithParam("name", "Test Person")
                    .ExecuteWithoutResults();

            // create a test resource to link to

            _client.Cypher
                    .Create("(r:Resource {Name : {name} })")
                    .WithParam("name", "Test Resource")
                    .ExecuteWithoutResults();


            // create a dialogue metadata node to link to

            _testMetadataGuid = Guid.NewGuid();

            _client.Cypher
                 .Create("(d:Dialogue {Id:{id}, Name:{name}})")
                 .WithParams(new { id = _testMetadataGuid, name = "Test Dialogue Map Name" })
                 .ExecuteWithoutResults();

            _statementDAO = new StatementDAO(GRAPH_DB_URL);

            _testStatement = new Statement()
            {
                Id = "1",
                NodeType = "Statement",
                Label = "Test statement text",
                Type = StatementType.Question,
                DialogueName = "Test Dialogue",
                RelatedConcepts = new List<Concept>() {
                    new Concept() { Name="Test Concept"}
                },
                RelatedPeople = new List<Person>() {
                    new Person() { Name="Test Person"}
                },
                RelatedResources = new List<Resource>() {
                    new Resource() { Name="Test Resource"}
                }
            };
        }

        [TestMethod]
        public void TestCreateStatementCreatesStatementWithLabelTestStatement()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(s => s.As<Statement>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test statement text", result.Label);
        }

        [TestMethod]
        public void TestCreateStatementWithNodeTypeStatement()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(s => s.As<Statement>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Statement", result.NodeType);
        }

        [TestMethod]
        public void TestCreateStatementWithDialogueNameTestDialogue()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(s => s.As<Statement>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Dialogue", result.DialogueName);
        }


        [TestMethod]
        public void TestCreateStatementCreatesStatementWithTypeQuestion()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(s => s.As<Statement>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual(StatementType.Question, result.Type);
        }

        [TestMethod]
        public void TestCreateStatementCreatesALinkToTheTestConcept()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)-[:REFERS_TO]->(c:Concept)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(c => c.As<Concept>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Concept", result.Name);
        }

        [TestMethod]
        public void TestCreateStatementCreatesALinkToTheTestPerson()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)-[:REFERS_TO]->(p:Person)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(p => p.As<Person>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Person", result.Name);
        }

        [TestMethod]
        public void TestCreateStatementCreatesALinkToTheTestResource()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)-[:REFERS_TO]->(r:Resource)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(r => r.As<Resource>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Resource", result.Name);
        }

        [TestMethod]
        public void TestCreatingAStatementWithNoLinkedConcepts()
        {
            Statement conceptlessStatement = new Statement() {
                Id = "2",
                Label = "Concept free statement",
                Type=  StatementType.Con,
                RelatedConcepts = new List<Concept>()
            };

            _statementDAO.Create(conceptlessStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)")
                            .Where((Statement s) => s.Id == "2")
                            .ReturnDistinct(s => s.As<Statement>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual(StatementType.Con, result.Type);
        }

        [TestMethod]
        public void TestCreatingAStatementLinksItToTheDialogueMetadata()
        {
            _statementDAO.Create(_testStatement, _testMetadataGuid);

            var result = _client.Cypher
                            .Match("(s:Statement)-[:PART_OF]->(d:Dialogue)")
                            .Where((Statement s) => s.Id == "1")
                            .ReturnDistinct(d => d.As<DialogueMapMetadata>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Dialogue Map Name", result.Name);
        }

    }
}
