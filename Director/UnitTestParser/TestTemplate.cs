using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Director.ParserLib;
using System.Collections.Generic;

namespace UnitTestParser
{
    [TestClass]
    public class TestTemplate
    {

        Parser parser = new Parser();


        [TestMethod]
        public void OperationWithoutType()
        {
            String template = "\"foo\": [\"##gt#10##\"";
            ParserResult pr = parser.validateResponse(template, null);

            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());
        }


        [TestMethod]
        public void MissingVariable()
        {
            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"a", "10"}
            };
            String template = "\"foo\": [\"#integer#ne#b##\"";
            ParserResult pr = parser.validateResponse(template, customVar);
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());
        }

        [TestMethod]
        public void WrongType() 
        {
            ParserResult pr;
            String template = "\"foo\": [\"#integer#eq#2.33##\"";

            pr = parser.validateResponse(template, null);
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());

            template = "\"foo\": [\"#integer#eq#2.33##\"";
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());
        }

        [TestMethod]
        public void MissingHash() 
        {
            ParserResult pr;
            String template = "\"foo\": [\"integer#eq#2##\"";

            pr = parser.validateResponse(template, null);
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());

            template = "\"foo\": [\"#integer#ne#2#\"";

            pr = parser.validateResponse(template, null);
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());
        }


        [TestMethod]
        public void WrongVariableType() 
        {
            ParserResult pr;
            String template = "\"foo\": [\"#integer#eq#ahoj##\"";

            Dictionary<string, string> customVar = new Dictionary<string, string>
            {
                {"ahoj", "abcd"}
            };

            pr = parser.validateResponse(template, customVar);
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());
        }
    }
}
