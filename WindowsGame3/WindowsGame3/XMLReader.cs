using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;

namespace Foldit3D
{
    class XMLReader
    {
        static XElement root;
        public static void Load(String fileName)
        {

            root = XElement.Load("../../../"+fileName);
        }

        //get specific data from level
        public static List<IDictionary<string, string>> Get(int level,string typeName)
        {
            var data = getLevelData(level).Element(typeName);
            List<IDictionary<string, string>> lst = new List<IDictionary<string,string>>();

            foreach (XElement e in data.Elements().ToList())
            {
                lst.Add(e.Elements().ToDictionary(element => element.Name.ToString(), element => element.Value));
            }

            return lst;
        }

        //returns all the level data
        public static XElement getLevelData(int level)
        {
            return root.Element("level" + level);

        }
    }
}
