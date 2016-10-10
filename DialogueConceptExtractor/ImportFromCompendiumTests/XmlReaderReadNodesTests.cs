using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

using ImportFromCompendium.DataAccess;
using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendiumTests
{
    /// <summary>
    /// Some Compendium information....
    /// 
    /// Node elements have a type attribute that use magic numbers to classify the types:
    /// 
    ///     1 = Inbox
    ///     2 = Map
    ///     3 = Question
    ///     4 = Idea
    ///     5 = TBD
    ///     6 = Pro    
    ///     7 = Con
    ///     8 = Decision
    /// 
    /// </summary>
    
    [TestClass]
    public class XmlReaderReadNodesTests
    {
        private const string RELATIVE_FILE_PATH = @"TestXML\DerbyMuseumsExport.xml";
        private CompendiumXMLReader _reader;

        [TestInitialize]
        public void SetUp()
        {
            _reader = new CompendiumXMLReader();
            _reader.ReadFile(RELATIVE_FILE_PATH);

        }

        [TestMethod]
        public void WhenTheTestFileIsOpened142StatementsAreReturned()
        {
            List<Statement> result = _reader.StatementList;
            Assert.AreEqual(142, result.Count);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedTheLastStatementHasLabelOfInbox()
        {
            List<Statement> result = _reader.StatementList;
            Assert.AreEqual("Inbox", result.LastOrDefault().Label);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedTheFourthStatementHasIdOf131231108391414583736220()
        {
            List<Statement> result = _reader.StatementList;
            Assert.AreEqual("131231108391414583736220", result[3].Id);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedTheTenthStatementHasATypeOfQuestion()
        {
            List<Statement> result = _reader.StatementList;
            Assert.AreEqual(StatementType.Question, result[9].Type);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedAListOf135StatementLinksIsReturned()
        {
            List<StatementLink> result = _reader.StatementLinkList;
            Assert.AreEqual(135, result.Count);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedTheFirstStatementHasAFromIdOf131231108391414583912028()
        {
            List<StatementLink> result = _reader.StatementLinkList;
            Assert.AreEqual("131231108391414583912028", result.First().FromId);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedTheLastStatementHasAToIdOf10199381531414677502604()
        {
            List<StatementLink> result = _reader.StatementLinkList;
            Assert.AreEqual("10199381531414677502604", result.Last().ToId);
        }


    }
}
