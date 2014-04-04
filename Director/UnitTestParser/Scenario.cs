using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Director.ParserLib;
using Microsoft.VisualStudio.TestTools.UnitTesting; 

namespace UnitTestParser
{
    class Scenario
    {
        public const String template1 = "\"{0}\"";
        public const String template2 = "\"###{0}##\"";
        public const String template3 = "\"#{0}##{1}##\"";
        public const String template4 = "\"#{0}#{1}#{2}##\"";

        Dictionary<string, string> values; 
        String type; 
        String operation;
        Parser parser;

        public Scenario(Dictionary<string, string> values, String type, String operation, Parser parser) 
        {
            this.values = values;
            this.type = type;
            this.operation = operation;
            this.parser = parser;
        }

        public void Test(Boolean positive)
        {
            String response, template;
            ParserResult pr;
            response = generateResponse();

            for (int i = 1; i <= 4; i++ )
            {
                template = generateTemplate(i);
                pr = parser.parseResponse(template, response, null);
                
                
                Assert.IsNotNull(pr);
                if (positive)
                    Assert.IsTrue(pr.isSuccess());
                else
                    Assert.IsFalse(pr.isSuccess());             
            }
        }

        public String generateResponse() 
        {
            char key = 'a';
            String response = "{ ";
            Boolean first = true;

            foreach (KeyValuePair<string, string> item in values)
            {
                if (!first) 
                    response += ", ";

                if(type.Equals("string"))
                    response += key++ + " : " + "\"" + item.Key + "\"";
                else
                    response += key++ + " : " + item.Key;

                first = false;
            }
            response += " }";
            return response;
        }

        public String generateTemplate(int templateConst) 
        {
            char key = 'a';
            String template = "{ ";
            String tmp;

            Boolean first = true;

            foreach (KeyValuePair<string, string> item in values)
            {
                if (!first)
                    template += ", ";
                switch (templateConst) 
                { 
                    case 1:
                        tmp = template1;
                        tmp = String.Format(tmp, item.Value);
                        break;
                    case 2:
                        tmp = template2;
                        tmp = String.Format(tmp, item.Value);
                        break;
                    case 3:
                        tmp = template3;
                        tmp = String.Format(tmp, type, item.Value);
                        break;
                    case 4:
                        tmp = template4;
                        tmp = String.Format(tmp, type, operation, item.Value);
                        break;
                    default:
                        return "";
                }
                template += key++ + " : " + tmp;

                first = false;
            }
            template += " }";
            return template;
        }
    }
}
