using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Director.ParserLib;
using System.Collections.Generic;

namespace UnitTestParser
{
    [TestClass]
    public class TestResponse
    {
        public Boolean[] templateAllExComp =    { false, false, false, true, true };
        public Boolean[] templateAllExType =    { true, true, true, true, false };
        public Boolean[] templateAll =          { true, true, true, true, true };
        public Boolean[] templateFullNotation = { false, false, false, true, false };

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
                {"123x", "123x"}, 
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
                {"0", "-0"},
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
                {"0.0", "0.0"},
                {"-0.0", "0.0"},
                {"-1.0", "-1.0"} 
            };
            Scenario sc = new Scenario(values, "real", "eq", parser, templateAll, null);
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
                {"1.2.3", " 1.2.3"}, 
                {"ABC", "abc"},
                {"abcd", "ABCD"}
            };
            Scenario sc = new Scenario(values, "string", "ne", parser, templateFullNotation, null);
            sc.Test(true);
        }

        [TestMethod]
        public void NotEqualInteger()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"10", "-10"},
                {"-60", "60"}, 
                {"0", "-10"},
                {"-0", "2"},
                {"1", "2"}
            };
            Scenario sc = new Scenario(values, "integer", "ne", parser, templateFullNotation, null);
            sc.Test(true);
        }

        [TestMethod]
        public void NotEqualBoolean()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"true", "false"},
                {"false", "true"} 
            };
            Scenario sc = new Scenario(values, "boolean", "ne", parser, templateFullNotation, null);
            sc.Test(true);
        }


        [TestMethod]
        public void NotEqualFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"1.0", "-1.0"},
                {"0.1", "-0.0"}, 
                {"-0.0", "10.0"}, 
                {"-1.0", "1.0"} 
            };
            Scenario sc = new Scenario(values, "real", "ne", parser, templateFullNotation, null);
            sc.Test(true);
        }

        //####################################
        //          less than
        //####################################

        [TestMethod]
        public void LessThanString()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"tex1", "teyt1 "},
                {"12x", "124y"}, 
                {"Ac", "abc"},
                {"1abd", "1hjkhkjhb"}
            };
            Scenario sc = new Scenario(values, "string", "lt", parser, templateAllExComp, null);
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
            Scenario sc = new Scenario(values, "integer", "lt", parser, templateAllExComp, null);
            sc.Test(true);
        }


        [TestMethod]
        public void LessThanFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-1.0", "1.0"},
                {"0.1", "1.0"}, 
                {"0.0", "10.0"}, 
                {"-2.0", "-1.0"}
            };
            Scenario sc = new Scenario(values, "real", "lt", parser, templateAllExComp, null);
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
                {"123a", "124b"}, 
                {"Abc", "abc"},
                {"d", "1b"},
                {"teXt1", "teXt1"},
                {"1234a", "1234a"}, 
                {"", ""},
                {" ", "  "}
            };
            Scenario sc = new Scenario(values, "string", "le", parser, templateAllExComp, null);
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
            Scenario sc = new Scenario(values, "integer", "le", parser, templateAllExComp, null);
            sc.Test(true);
        }


        [TestMethod]
        public void LessEqualFloat()
        { 
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-1.1", "1.0"},
                {"0.2", "1.0"}, 
                {"8.0", "10.0"}, 
                {"-2.0", "-1.0"},
                {"1.0", "1.0"},
                {"0.0", "0.0"}, 
                {"-1.0", "-1.0"} 
            };
            Scenario sc = new Scenario(values, "real", "le", parser, templateAllExComp, null);
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
                {"124ED", "123ddgd"}, 
                {"abc ", "Abc"},
                {"1abcd1abcd", "1abcd"}
            };
            Scenario sc = new Scenario(values, "string", "gt", parser, templateAllExComp, null);
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
            Scenario sc = new Scenario(values, "integer", "gt", parser, templateAllExComp, null);
            sc.Test(true);
        }


        [TestMethod]
        public void GreaterThanFloat()
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"1.0", "-1.0"},
                {"1.1", "0.0"}, 
                {"10.0", "0.0"}, 
                {"-1.0", "-2.0"}
            };
            Scenario sc = new Scenario(values, "real", "gt", parser, templateAllExComp, null);
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
                {"aaaaaa", "bbb"},
                {"124x", "123x"}, 
                {"abc ", "Abc"},
                {"abaa", "1d"},
                {"tegjggxt1", "abaa"},
                {"123x", "123x"}, 
                {"   ", " "}
            };
            Scenario sc = new Scenario(values, "string", "ge", parser, templateAllExComp, null);
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
            Scenario sc = new Scenario(values, "integer", "ge", parser, templateAllExComp, null);
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
            Scenario sc = new Scenario(values, "real", "ge", parser, templateAllExComp, null);
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
                {"teyt1 ", ".*"},
                {"c", "."}, 
                {"AZ123", "[A-Z0-9]*"},
                {"3", "[A-Z0-9]*"},
                {"text1", "text1"},
                {"123", "123"}, 
                {"teeeeeext1", "te*xt1"},
                {"ahoj", "ah*oj"}
            };
            Scenario sc = new Scenario(values, "string", "mp", parser, templateFullNotation, null);
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
                {"c", "1.###3"},
                {"d", "2,48"}
            };
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "a"},
                {"123", "b"}, 
                {"1.###3", "c"},
                {"2,48", "d"}
            };
            Scenario sc = new Scenario(values, "string", "uv_eq", parser, templateFullNotation, customVar);
            sc.Test(true);

            sc = new Scenario(values, "string", "uv_le", parser, templateFullNotation, customVar);
            sc.Test(true);

            sc = new Scenario(values, "string", "uv_ge", parser, templateFullNotation, customVar);
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
            Scenario sc = new Scenario(values, "integer", "uv_eq", parser, templateFullNotation, customVar);
            sc.Test(true);

            sc = new Scenario(values, "integer", "uv_le", parser, templateFullNotation, customVar);
            sc.Test(true);

            sc = new Scenario(values, "integer", "uv_ge", parser, templateFullNotation, customVar);
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
            Scenario sc = new Scenario(values, "integer", "uv_ne", parser, templateFullNotation, customVar);
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
            Scenario sc = new Scenario(values, "real", "uv_gt", parser, templateFullNotation, customVar);
            sc.Test(true);

            sc = new Scenario(values, "real", "uv_ge", parser, templateFullNotation, customVar);
            sc.Test(true);
        }

        //#######################################
        //          if present
        //#######################################


        [TestMethod]
        public void IpEqualString()
        {

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"text1", "text1"},
                {"123", "123"}, 
                {"c", ""},
                {"d", " "}
            };

            Scenario sc = new Scenario(values, "string", "ip_eq", parser, templateFullNotation, null);
            sc.Test(true);

            sc = new Scenario(values, "string", "ip_le", parser, templateFullNotation, null);
            sc.Test(true);

            sc = new Scenario(values, "string", "ip_ge", parser, templateFullNotation, null);
            sc.Test(true);
        }

        [TestMethod]
        public void IpNotEqualInteger()
        { 
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"-1", "10"},
                {"1", "12"}, 
                {"20", "-5"},
                {"11", "1"}
            };
            Scenario sc = new Scenario(values, "integer", "ip_ne", parser, templateFullNotation, null);
            sc.Test(true);
        }

        [TestMethod]
        public void IpGreaterThanFloat()
        {
            ParserResult pr = parser.parseResponse("{ a : \"#real#ip_gt#5##\", b: \"#real#ip_gt#0##\"  }", "{ b: 1.0 }", null, true);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());

        }


        //#######################################
        //          arrays
        //#######################################

        [TestMethod]
        public void ArrayInteger()
        {
            String result; 
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "3"},
                {"b", "10"}
            };
            String template = "{\"foo\": [\"#array#uv_ge#a#res#\", \"#integer#uv_ge#b##\"] }";
            String response = "{\"foo\": [10,20,30] }";
            ParserResult pr = parser.parseResponse(template, response, customVar, true);
            
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());

            Assert.IsTrue( customVar.TryGetValue("res", out result) );
            Assert.AreEqual(result, "3");
        }


        [TestMethod]
        public void ArrayString()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "abc"}
            };
            String template = "{\"foo\": [\"#string#uv_ne#a##\"] }";
            String response = "{\"foo\": [\"ab\", \"abcd\"] } ";
            ParserResult pr = parser.parseResponse(template, response, customVar, true);

            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
        }
    }
}
