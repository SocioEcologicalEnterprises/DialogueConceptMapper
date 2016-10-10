using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using ImportFromCompendium.BusinessLogic;

namespace ImportFromCompendium.DataAccess
{
    public class CompendiumXMLReader
    {

        public string DialogueName { get; set; }

        public List<Statement> StatementList { get; set; }

        public List<StatementLink> StatementLinkList { get; set; }

        public CompendiumXMLReader() { }

        public CompendiumXMLReader(string xmlFilePath) 
        {
            ReadFile(xmlFilePath);
        }

        public void ReadFile(string xmlFilePath)
        {
            StatementList = new List<Statement>();

            try
            {            
                XmlDocument compendiumDoc = new XmlDocument();
                compendiumDoc.Load(xmlFilePath);

                foreach (XmlNode statementNode in compendiumDoc.GetElementsByTagName("node"))
                {
                    EntityExtractor extractor = new EntityExtractor();
                    
                    Statement node = new Statement();
                    node.DialogueName = DialogueName;
                    node.Id = statementNode.Attributes["id"].Value;
                    node.Label = statementNode.Attributes["label"].Value;
                    node.Type = (StatementType)Int32.Parse(statementNode.Attributes["type"].Value);
                    node.NodeType = "Statement";

                    extractor.ExtractEntities(node.Label);
                    node.RelatedConcepts = extractor.Concepts;
                    node.RelatedResources = extractor.Resources;
                    node.RelatedPeople = extractor.People;

                    StatementList.Add(node);
                }

                StatementLinkList = new List<StatementLink>();

                foreach (XmlNode linkNode in compendiumDoc.GetElementsByTagName("link"))
                {
                    StatementLink link = new StatementLink();
                    link.FromId = linkNode.Attributes["from"].Value;
                    link.ToId = linkNode.Attributes["to"].Value;

                    StatementLinkList.Add(link);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            

        }

    }
}
