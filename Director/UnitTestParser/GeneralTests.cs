using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Director.ParserLib;

namespace UnitTestParser
{
    [TestClass]
    public class GeneralTests
    {

        Dictionary<string, string> custom_variables;
        Parser parser;
        ParserResult pr;

        public GeneralTests()
        {
            parser = new Parser();
            custom_variables = new Dictionary<string, string>();
            custom_variables.Add("first", "second");
            custom_variables.Add("x", "42");
        }

        [TestMethod]
        public void deserializeObjectsInArray()
        {
            // TEMPLATE:
            //{
            //    "menu": {
            //        "id": "file",
            //        "value": "File",
            //        "popup": {
            //            "int": 230,
            //            "double": 309.302,
            //            "null": null,
            //            "menuitem": [
            //                {
            //                    "value": "New",
            //                    "onclick": "CreateNewDoc()"
            //                },
            //                {
            //                    "value": "Open",
            //                    "onclick": "OpenDoc()"
            //                },
            //                {
            //                    "value": "Close",
            //                    "onclick": "CloseDoc()"
            //                }
            //            ]
            //        }
            //    }
            //}
            //
            pr = parser.generateRequest("{ \"menu\" : { \"id\": \"file\", \"value\": \"File\", \"popup\": { \"int\": 230, \"double\": 309.302, \"null\": null, \"menuitem\": [ { \"value\": \"New\", \"onclick\": \"CreateNewDoc()\" }, { \"value\": \"Open\", \"onclick\": \"OpenDoc()\" }, { \"value\": \"Close\", \"onclick\": \"CloseDoc()\" } ] } } }", null);
            Assert.IsTrue(pr.isSuccess());
        }


        [TestMethod]
        public void deserializeArrayInArray()
        {
            // TEMPLATE:
            //{
            //    "infos": [
            //        [
            //            {
            //                "title": "title 1",
            //                "desciption": "description 1"
            //            },
            //            {
            //                "title": "title 2",
            //                "description": "description 2"
            //            },
            //            {
            //                "title": "title 3",
            //                "description": "description 3"
            //            }
            //        ]
            //    ]
            //}
            //
            pr = parser.generateRequest("{ \"infos\": [ [ { \"title\": \"title 1\", \"desciption\": \"description 1\" }, { \"title\": \"title 2\", \"description\": \"description 2\" }, { \"title\": \"title 3\", \"description\": \"description 3\" } ] ] }", null);
            Assert.IsTrue(pr.isSuccess());
        }

        [TestMethod]
        public void compareResponseWithExtraKey()
        {
            // TEMPLATE:
            //{
            //    "result": {
            //        "country": {
            //            "termOfUse: ": "http"
            //        }
            //    }
            //}
            //
            // RESPONSE:
            //{
            //    "result": {
            //        "country": {
            //            "termOfUse: ": "http",
            //            "preset": "+420"
            //        }
            //    }
            //}
            //
            pr = parser.parseResponse("{ \"result\" : { \"country\" : { \"termOfUse: \" : \"http\" } } }", "{ \"result\" : { \"country\" : { \"termOfUse: \" : \"http\", \"preset\" : \"+420\" } } }", null, true);
            Assert.IsTrue(pr.isSuccess());
        }
    }
}
