using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Neo4jClient;

using ImportFromCompendium.DataAccess;
using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendiumTests
{
    [TestClass]
    public class PersonDAOTests
    {
        private const string GRAPH_DB_URL = @"http://localhost:7474/db/data";

        private GraphClient _client;

        private PersonDAO _personDAO;

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

            _personDAO = new PersonDAO(GRAPH_DB_URL);


        }

        [TestMethod]
        public void TestCreateParticipant()
        {
            
            Person testPerson = new Person() { Name="Test Participant", Email="test@participant.com" };

            _personDAO.Create(testPerson);

            var result = _client.Cypher
                            .Match("(p:Person)")
                            .Where((Person p) => p.Email == "test@participant.com")
                            .ReturnDistinct(p => p.As<Person>())
                            .Results
                            .FirstOrDefault();

            Assert.AreEqual("Test Participant", result.Name);

        }

        [TestMethod]
        public void ReadPersonTest()
        {
            _personDAO.Create(new Person() { Name = "Test Person", Email = "Test Email" });

            List<Person> result = _personDAO.GetPeople();

            Assert.AreEqual("Test Person", result[0].Name);

        }

        [TestMethod]
        public void ReadPeopleTest()
        {
            _personDAO.Create(new Person() { Name = "Test Person 1", Email = "Test email 1" });
            _personDAO.Create(new Person() { Name = "Test Person 2", Email = "Test email 2" });

            List<Person> result = _personDAO.GetPeople();

            Assert.AreEqual("Test Person 2", result[1].Name);
        }

    }
}
