using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImportFromCompendium.BusinessLogic;
using ImportFromCompendium.DataAccess;

namespace ImportFromCompendium
{
    class Program
    {
        private const string GRAPH_DB_URL = @"http://localhost:7474/db/data";
        private static string _dialogueName = "";
        
        static void Main(string[] args)
        {
            Guid metadataGuid = Guid.NewGuid();

            createMetadata(metadataGuid);

            Console.WriteLine("Enter the full path to the Compendium XML file");

            string xmlFilePath = Console.ReadLine();

            CompendiumXMLReader fileReader = new CompendiumXMLReader(xmlFilePath);
            fileReader.DialogueName = _dialogueName;

            Console.WriteLine(string.Format("{0} statements and {1} statement links loaded successfully", fileReader.StatementList.Count, fileReader.StatementLinkList.Count));

            //TODO: populate these lists with all the concepts, resources and people from the database
            
            List<string> existingConcepts = new List<string>();
            List<string> existingPeople = new List<string>();
            List<string> existingResources = new List<string>();

            StatementDAO statementDAO = new StatementDAO(GRAPH_DB_URL);

            Console.WriteLine("Reading data that already exists in the database");
            ConceptDAO conceptDAO = new ConceptDAO(GRAPH_DB_URL);
            List<Concept> previouslySavedConcepts = conceptDAO.GetConcepts();
            foreach(Concept c in previouslySavedConcepts)
            {
                existingConcepts.Add(c.Name);
            }
            Console.WriteLine(String.Format("{0} existing Concepts found", existingConcepts.Count));

            PersonDAO personDAO = new PersonDAO(GRAPH_DB_URL);
            List<Person> previouslySavedPeople = personDAO.GetPeople();
            foreach(Person p in previouslySavedPeople)
            {
                existingPeople.Add(p.Name);
            }
            Console.WriteLine(String.Format("{0} existing People found", existingPeople.Count));

            ResourceDAO resourceDAO = new ResourceDAO(GRAPH_DB_URL);
            List<Resource> previouslySavedResources = resourceDAO.GetResources();
            foreach(Resource r in previouslySavedResources)
            {
                existingResources.Add(r.Name);
            }
            Console.WriteLine(String.Format("{0} existing Resources found", existingResources.Count));

            foreach (Statement statement in fileReader.StatementList)
            {
                foreach(Concept concept in statement.RelatedConcepts)
                {
                    if(!existingConcepts.Contains(concept.Name))
                    {
                        conceptDAO.Create(concept);
                        Console.WriteLine(String.Format("New concept {0} found and saved", concept.Name));
                        existingConcepts.Add(concept.Name);
                    }
                }

                foreach (Person person in statement.RelatedPeople)
                {
                    if (!existingPeople.Contains(person.Name))
                    {
                        personDAO.Create(person);
                        Console.WriteLine(String.Format("New person {0} found and saved", person.Name));
                        existingPeople.Add(person.Name);
                    }
                }

                foreach (Resource resource in statement.RelatedResources)
                {
                    if (!existingResources.Contains(resource.Name))
                    {
                        resourceDAO.Create(resource);
                        Console.WriteLine(String.Format("New resource {0} found and saved", resource.Name));
                        existingResources.Add(resource.Name);
                    }
                }

                statementDAO.Create(statement, metadataGuid);
                Console.WriteLine(String.Format("Statement: {0} saved", statement.Label));
            }

            Console.WriteLine("Statements successfully saved. Now linking statements");

            foreach(StatementLink link in fileReader.StatementLinkList)
            {
                statementDAO.LinkStatements(link);
                Console.WriteLine(String.Format("Statements with Id:{0} and Id:{1} linked", link.FromId, link.ToId));
            }

            Console.WriteLine("Finished. Press any key to quit...");
            Console.ReadLine();

        }

        private static void createMetadata(Guid metadataGuid)
        {
            DialogueMapMetadata metadata = new DialogueMapMetadata();
            metadata.Id = metadataGuid;
            Console.WriteLine("Enter a name for the dialogue map you're importing:");
            metadata.Name = Console.ReadLine();
            _dialogueName = metadata.Name;

            metadata.DateCreated = Int64.Parse(DateTime.Now.ToString("yyyyMMddhhmmss"));
            metadata.Participants = new List<Person>();
            metadata.NodeType = "Dialogue";

            Console.WriteLine();

            Console.WriteLine(String.Format("Enter participant details for map name: {0}, created on: {1}", metadata.Name, metadata.DateCreated));

            Console.WriteLine();

            int i = 1;

            do
            {
                Person participant = new Person();
                Console.WriteLine(String.Format("Enter name of Participant {0}", i));
                participant.Name = Console.ReadLine();
                Console.WriteLine(String.Format("Enter email of Participant {0}", i));
                participant.Email = Console.ReadLine();
                metadata.Participants.Add(participant);
                Console.WriteLine("Finished adding participants? y/n");
                Console.WriteLine();
                i++;
            } while (!Console.ReadLine().ToLower().Equals("y"));

            Console.WriteLine();
            Console.WriteLine("Saving dialogue map data.");

            DialogueMapMetadataDAO metadataDAO = new DialogueMapMetadataDAO(GRAPH_DB_URL);
            metadataDAO.Create(metadata);
            Console.WriteLine("Dialogue map data saved successfully.");
        }
    }
}
