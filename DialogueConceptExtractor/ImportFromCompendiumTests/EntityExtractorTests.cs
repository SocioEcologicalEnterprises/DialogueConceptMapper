using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendiumTests
{
    [TestClass]
    public class EntityExtractorTests
    {
        private EntityExtractor _extractor = new EntityExtractor();

        [TestInitialize]
        public void SetUp()
        {
            _extractor = new EntityExtractor();
        }

        [TestMethod]
        public void TestSingleConceptWithNameConceptExtracted()
        {
            _extractor.ExtractEntities("This has one #concept in it");

            List<Concept> result = _extractor.Concepts;

            Assert.AreEqual("concept", result.First().Name);
        }

        [TestMethod]
        public void TestSingleConceptWithNameDifferentExtracted()
        {
            _extractor.ExtractEntities("This has one has a #different concept in it");

            List<Concept> result = _extractor.Concepts;

            Assert.AreEqual("different", result.First().Name);
        }

        [TestMethod]
        public void TestStringWithFourConceptsReturnsAListOfFourConcepts()
        {
            _extractor.ExtractEntities("This has #lots of #different #concepts in the #text_string");

            List<Concept> result = _extractor.Concepts;

            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void TestStringWithFourConceptsHasLastConceptOfText_String()
        {
            _extractor.ExtractEntities("This has #lots of #different #concepts in the #text_string");

            List<Concept> result = _extractor.Concepts;

            Assert.AreEqual("text_string", result.Last().Name);
        }

        [TestMethod]
        public void TestConceptExtractorIgnoresLetterCases()
        {
            _extractor.ExtractEntities("This has a #Concept with an uppercase first letter");

            List<Concept> result = _extractor.Concepts;
            Assert.AreEqual("concept", result.Last().Name);
        }

        [TestMethod]
        public void TestConceptEndsInAQuestionMark()
        {
            _extractor.ExtractEntities("This has a concept that ends with a #question_mark?");

            List<Concept> result = _extractor.Concepts;
            Assert.AreEqual("question_mark", result.Last().Name);
        }

        [TestMethod]
        public void TestSingleResourceInStatement()
        {
            _extractor.ExtractEntities("This has a >Resource in it");

            List<Resource> result = _extractor.Resources;
            Assert.AreEqual("resource", result.First().Name);
        }

        [TestMethod]
        public void TestSinglePersonInStatement()
        {
            _extractor.ExtractEntities("This has a @Person in it");

            List<Person> result = _extractor.People ;
            Assert.AreEqual("person", result.First().Name);
        }

    }
}
