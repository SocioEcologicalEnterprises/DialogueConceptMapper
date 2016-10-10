using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFromCompendium.BusinessLogic
{
    public abstract class Node
    {

        /// <summary>
        /// Used to give every node a NodeType so nodes of each type can be selected in Gephi
        /// because Gephi doesn't seem to have any way of coping with Neo4J Labels
        /// </summary>
        public string NodeType { get; set; }

    }
}
