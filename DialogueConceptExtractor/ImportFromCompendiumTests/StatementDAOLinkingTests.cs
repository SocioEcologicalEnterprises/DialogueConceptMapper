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
    public class StatementDAOLinkingTests
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

            // create a couple of test statements 

            _client.Cypher
                     .Create("(s:Statement {Id : {id}, Label : {label}, Type: {type}})")
                     .WithParams(new { id = "1", label = "Test statement 1", type = StatementType.Question.ToString() })
                     .ExecuteWithoutResults();

            _client.Cypher
                 .Create("(s:Statement {Id : {id}, Label : {label}, Type: {type}})")
                 .WithParams(new { id = "2", label = "Test statement 2", type = StatementType.Idea.ToString() })
                 .ExecuteWithoutResults();

            _statementDAO = new StatementDAO(GRAPH_DB_URL);



        }

        [TestMethod]
        public void TestStatementsLinked()
        {
            StatementLink testStatementLink = new StatementLink() { FromId="1", ToId="2" };
            _statementDAO.LinkStatements(testStatementLink);

            var result = _client.Cypher
                            .Match("(s1:Statement)-[:IN_RESPONSE_TO]->(s2:Statement)")
                            .Where((Statement s1) => s1.Id == "1")
                            .ReturnDistinct(s2 => s2.As<Statement>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test statement 2", result.Label);
        }
    }
}
