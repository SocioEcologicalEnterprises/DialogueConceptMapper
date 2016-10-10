using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

using ImportFromCompendium.DataAccess;
using ImportFromCompendium.BusinessLogic;


namespace ImportFromCompendiumTests
{
    [TestClass]
    public class XmlReaderReadsNodesWithConcepts
    {
        private const string RELATIVE_FILE_PATH = @"TestXML\WhatAreSocioEcologicalEnterprises.xml";
        private CompendiumXMLReader _reader;

        [TestInitialize]
        public void SetUp()
        {
            _reader = new CompendiumXMLReader();
            _reader.ReadFile(RELATIVE_FILE_PATH);

        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedStatement3Has3Concepts()
        {
            List<Statement> result = _reader.StatementList;
            Assert.AreEqual(3, result[2].RelatedConcepts.Count);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedStatement6Has1Resource()
        {
            List<Statement> result = _reader.StatementList;
            Assert.AreEqual(1, result[5].RelatedResources.Count);
        }

        [TestMethod]
        public void WhenTheTestFileIsOpenedStatement7Has1Person()
        {
            List<Statement> result = _reader.StatementList;
            Assert.AreEqual(1, result[7].RelatedPeople.Count);
        }

    }
}
