using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFromCompendium.BusinessLogic
{
    public class EntityExtractor
    {

        public List<Concept> Concepts { get; set; }
        public List<Resource> Resources { get; set; }
        public List<Person> People { get; set; }

        public EntityExtractor()
        {
            Concepts = new List<Concept>();
            Resources = new List<Resource>();
            People = new List<Person>();
        }


        public void ExtractEntities(string statement)
        {
            List<string> tokens = statement.Split(' ').ToList();

            ExtractConcepts(FindRelevantTokens(tokens, "#"));
            ExtractResources(FindRelevantTokens(tokens, ">"));
            ExtractPeople(FindRelevantTokens(tokens, "@"));

        }

        private List<String> FindRelevantTokens(List<String> tokens, string tokenStart)
        {
            return (from t in tokens where t.StartsWith(tokenStart) select t).ToList<string>();
        }


        private void ExtractConcepts(List<String> hashTaggedTokens)
        {

            foreach (string taggedToken in hashTaggedTokens)
            {
                Concepts.Add(new Concept() { Name = CleanEntity(taggedToken, "#"), NodeType = "Concept" });
            }

        }

        private void ExtractResources(List<String> greaterThannedTokens)
        {

            foreach (string taggedToken in greaterThannedTokens)
            {
                Resources.Add(new Resource() { Name = CleanEntity(taggedToken, ">"), NodeType = "Resource" });
            }

        }

        private void ExtractPeople(List<String> attedTokens)
        {

            foreach (string taggedToken in attedTokens)
            {
                People.Add(new Person() { Name = CleanEntity(taggedToken, "@"), NodeType = "Person" });
            }

        }

        private String CleanEntity(String dirtyEntity, String replaceChars)
        {
            string token = dirtyEntity.Replace(replaceChars, "").ToLower();
            StringBuilder sb = new StringBuilder();

            foreach (char c in token)
            {
                if (c.Equals('_'))
                {
                    sb.Append(c);
                }
                else if (!char.IsPunctuation(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();

        }

    }
}
