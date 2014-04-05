using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Director.ParserLib;
using System.Collections.Generic;

namespace UnitTestParser
{
    [TestClass]
    public class UnitTestParserResponse
    {
        public Boolean[] templateAll = { true, true, true, true, true };
        public Boolean[] templateRegexUvIp = { false, false, false, true, false };

        Parser parser = new Parser();
        

        [TestInitialize]
        public void TestInitialize() 
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
               
        //####################################
        //          equal 
        //####################################

        [TestMethod]
        public void EqualString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "text1"},
                {"123", "123"}, 
                {"", ""},
                {" ", " "}
            };
            Scenario sc = new Scenario(values, "string", "eq", parser, templateAll, null);
            sc.Test(true);
        }

        [TestMethod]
        public void EqualInteger()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"10", "10"},
                {"-60", "-60"}, 
                {"0", "0"},
                {"-1", "-1"}
            };
            Scenario sc = new Scenario(values, "integer", "eq", parser, templateAll, null);
            sc.Test(true);
        }

        [TestMethod]
        public void EqualBoolean()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"true", "true"},
                {"false", "false"}
            };
            Scenario sc = new Scenario(values, "boolean", "eq", parser, templateAll, null);
            sc.Test(true);
        }


        [TestMethod]
        public void EqualFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"1.0", "1.0"},
                {"0", "0"}, 
                {"-1.0", "-1.0"} 
            };
            Scenario sc = new Scenario(values, "float", "eq", parser, templateAll, null);
            sc.Test(true);
        }

        //####################################
        //          not equal
        //####################################

        [TestMethod]
        public void NotEqualString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "text1 "},
                {" 123", "123"}, 
                {"ABC", "abc"},
                {"abcd", "ABCD"}
            };
            Scenario sc = new Scenario(values, "string", "ne", parser, templateAll, null);
            sc.Test(false);
        }

        [TestMethod]
        public void NotEqualInteger()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"10", "-10"},
                {"-60", "60"}, 
                {"0", "-0"},
                {"-0", "0"},
                {"1", "2"}
            };
            Scenario sc = new Scenario(values, "integer", "ne", parser, templateAll, null);
            sc.Test(false);
        }

        [TestMethod]
        public void NotEqualBoolean()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"true", "false"},
                {"false", "true"} 
            };
            Scenario sc = new Scenario(values, "boolean", "ne", parser, templateAll, null);
            sc.Test(false);
        }


        [TestMethod]
        public void NotEqualFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"1.0", "-1.0"},
                {"0", "-0"}, 
                {"-0", "0"}, 
                {"-1.0", "1.0"} 
            };
            Scenario sc = new Scenario(values, "float", "ne", parser, templateAll, null);
            sc.Test(false);
        }

        //####################################
        //          less than
        //####################################

        [TestMethod]
        public void LessThanString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "teyt1 "},
                {"123", "124"}, 
                {"Abc", "abc"},
                {"1abcd", "1b"}
            };
            Scenario sc = new Scenario(values, "string", "lt", parser, templateAll, null);
            sc.Test(true);
        }

        [TestMethod]
        public void LessThanInteger()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"10", "11"},
                {"-61", "-60"}, 
                {"0", "1"},
                {"1", "11"},
                {"-1", "1"}
            };
            Scenario sc = new Scenario(values, "integer", "lt", parser, templateAll, null);
            sc.Test(true);
        }


        [TestMethod]
        public void LessThanFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-1.0", "1.0"},
                {"0.0", "1"}, 
                {"0", "10"}, 
                {"-2.0", "-1.0"}
            };
            Scenario sc = new Scenario(values, "float", "lt", parser, templateAll, null);
            sc.Test(true);
        }

        //####################################
        //          less equal
        //####################################

        [TestMethod]
        public void LessEqualString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "teyt1 "},
                {"123", "124"}, 
                {"Abc", "abc"},
                {"1abcd", "1b"},
                {"teXt1", "teXt1"},
                {"1234", "1234"}, 
                {"", ""},
                {" ", " "}
            };
            Scenario sc = new Scenario(values, "string", "le", parser, templateAll, null);
            sc.Test(true);
        }

        [TestMethod]
        public void LessEqualInteger()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"100", "101"},
                {"-61", "-60"}, 
                {"2", "11"},
                {"1", "11"},
                {"-2", "1"},
                {"10", "10"},
                {"-60", "-60"}, 
                {"0", "0"},
                {"-1", "-1"}
            };
            Scenario sc = new Scenario(values, "integer", "le", parser, templateAll, null);
            sc.Test(true);
        }


        [TestMethod]
        public void LessEqualFloat()
        { 
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-1.1", "1.0"},
                {"0", "1"}, 
                {"8", "10"}, 
                {"-2.0", "-1.0"},
                {"1.0", "1.0"},
                {"0.0", "0"}, 
                {"-1.0", "-1.0"} 
            };
            Scenario sc = new Scenario(values, "float", "le", parser, templateAll, null);
            sc.Test(true);
        }







        //####################################
        //          greater than
        //####################################

        [TestMethod]
        public void GreaterThanString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"teyt1 ", "text1"},
                {"124", "123"}, 
                {"abc", "Abc"},
                {"1b", "1abcd"}
            };
            Scenario sc = new Scenario(values, "string", "gt", parser, templateAll, null);
            sc.Test(true);
        }

        [TestMethod]
        public void GreaterThanInteger()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"11", "10"},
                {"-60", "-61"}, 
                {"1", "0"},
                {"111", "11"},
                {"10", "-1"}
            };
            Scenario sc = new Scenario(values, "integer", "gt", parser, templateAll, null);
            sc.Test(true);
        }


        [TestMethod]
        public void GreaterThanFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"1.0", "-1.0"},
                {"1", "0"}, 
                {"10", "0"}, 
                {"-1.0", "-2.0"}
            };
            Scenario sc = new Scenario(values, "float", "gt", parser, templateAll, null);
            sc.Test(true);
        }
        
        //####################################
        //          Greater equal
        //####################################

        [TestMethod]
        public void GreaterEqualString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"teyt1 ", "text1"},
                {"124", "123"}, 
                {"abc", "Abc"},
                {"1b", "1abcd"},
                {"text1", "text1"},
                {"123", "123"}, 
                {"", ""},
                {" ", " "}
            };
            Scenario sc = new Scenario(values, "string", "ge", parser, templateAll, null);
            sc.Test(true);
        }

        [TestMethod]
        public void GreaterEqualInteger()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"11", "10"},
                {"-60", "-61"}, 
                {"1", "0"},
                {"12", "1"},
                {"2", "-1"},
                {"9", "9"},
                {"60", "60"}, 
                {"0", "0"},
                {"-1", "-1"}
            };
            Scenario sc = new Scenario(values, "integer", "ge", parser, templateAll, null);
            sc.Test(true);
        }


        [TestMethod]
        public void GreaterEqualFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"1.0", "-1.0"},
                {"1", "0"}, 
                {"10", "0"}, 
                {"-1.0", "-2.0"},
                {"1.112", "1.112"},
                {"0", "0"}, 
                {"-1.01", "-1.01"} 
            };
            Scenario sc = new Scenario(values, "float", "ge", parser, templateAll, null);
            sc.Test(true);
        }


        //####################################
        //          Matching regex
        //####################################

        [TestMethod]
        public void MatchingRegexString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"teyt1 ", "*"},
                {"c", "."}, 
                {"AZ123", "[A-Z0-9]*"},
                {"3", "[A-Z0-9]*"},
                {"text1", "text1"},
                {"123", "123"}, 
                {"te*xt1", "te24dsxt1"}
            };
            Scenario sc = new Scenario(values, "string", "mp", parser, templateRegexUvIp, null);
            sc.Test(true);
        }

        //#######################################
        //          use variable 
        //#######################################

        [TestMethod]
        public void UvEqualString()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "text1"},
                {"b", "123"}, 
                {"c", ""},
                {"d", " "}
            };
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "a"},
                {"123", "b"}, 
                {"", "c"},
                {" ", "d"}
            };
            Scenario sc = new Scenario(values, "string", "uv_eq", parser, templateRegexUvIp, customVar);
            sc.Test(true);

            sc = new Scenario(values, "string", "uv_le", parser, templateRegexUvIp, customVar);
            sc.Test(true);

            sc = new Scenario(values, "string", "uv_ge", parser, templateRegexUvIp, customVar);
            sc.Test(true);
        }

        [TestMethod]
        public void UvEqualInteger()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "-10"},
                {"b", "-1"}, 
                {"c", "0"},
                {"d", "1"}
            };
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-10", "a"},
                {"-1", "b"}, 
                {"0", "c"},
                {"1", "d"}
            };
            Scenario sc = new Scenario(values, "integer", "uv_eq", parser, templateRegexUvIp, customVar);
            sc.Test(true);

            sc = new Scenario(values, "integer", "uv_le", parser, templateRegexUvIp, customVar);
            sc.Test(true);

            sc = new Scenario(values, "integer", "uv_ge", parser, templateRegexUvIp, customVar);
            sc.Test(true);
        }

        [TestMethod]
        public void UvNotEqualInteger()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "-10"},
                {"b", "-1"}, 
                {"c", "0"},
                {"d", "1"}
            };
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-1", "a"},
                {"1", "b"}, 
                {"20", "c"},
                {"11", "d"}
            };
            Scenario sc = new Scenario(values, "integer", "uv_ne", parser, templateRegexUvIp, customVar);
            sc.Test(true);
        }

        [TestMethod]
        public void UvGreaterThanFloat()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "-10.2"},
                {"b", "-1"}, 
                {"c", "0"},
                {"d", "1.22"}
            };
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-10", "a"},
                {"1", "b"}, 
                {"20", "c"},
                {"1.23", "d"}
            };
            Scenario sc = new Scenario(values, "float", "uv_gt", parser, templateRegexUvIp, customVar);
            sc.Test(true);

            sc = new Scenario(values, "float", "uv_ge", parser, templateRegexUvIp, customVar);
            sc.Test(true);
        }

        //#######################################
        //          if present
        //#######################################


        [TestMethod]
        public void IpEqualString()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "text1"},
                {"b", "123"}, 
                {"c", ""},
                {"d", " "}
            };
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "a"},
                {"123", "b"}, 
                {"", "c"},
                {" ", "d"}
            };
            Scenario sc = new Scenario(values, "string", "ip_eq", parser, templateRegexUvIp, customVar);
            sc.Test(true);

            sc = new Scenario(values, "string", "ip_le", parser, templateRegexUvIp, customVar);
            sc.Test(true);

            sc = new Scenario(values, "string", "ip_ge", parser, templateRegexUvIp, customVar);
            sc.Test(true);
        }

        [TestMethod]
        public void IpNotEqualInteger()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "-10"},
                {"b", "-1"}, 
                {"c", "0"},
                {"d", "1"}
            };
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-1", "a"},
                {"1", "b"}, 
                {"20", "c"},
                {"11", "d"}
            };
            Scenario sc = new Scenario(values, "integer", "ip_ne", parser, templateRegexUvIp, customVar);
            sc.Test(true);
        }

        [TestMethod]
        public void IpGreaterThanFloat()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"var", "-10.2"},
            };

            ParserResult pr = parser.parseResponse("{ a : \"#float#ip_gt#var##\", b: \"#float#ip_gt#0##\"  }", "{ b: 1 }", customVar);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());

        }
    }
}
