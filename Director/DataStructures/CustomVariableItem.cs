using System.Xml.Serialization;

namespace Director.DataStructures 
{
    public class CustomVariableItem
    { 
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string value;
    }
}
