using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Director.ParserLib;

namespace UnitTestParser
{
    [TestClass]
    public class TestHeaderParsing
    {

        Dictionary<string, string> custom_variables;
        ParserResult pr;

        public TestHeaderParsing()
        {
            custom_variables = new Dictionary<string, string>();
            custom_variables.Add("first", "second");
            custom_variables.Add("x", "42");
        }

        [TestMethod]
        public void simpleParseHeader()
        {
            pr = Parser.parseHeader("", null);
            Assert.IsTrue(pr.isSuccess());
            Assert.AreEqual(pr.getResult(), "");

            pr = Parser.parseHeader("simple $first$ string", custom_variables);
            Assert.IsTrue(pr.isSuccess());
            Assert.AreEqual(pr.getResult(), "simple second string");

            pr = Parser.parseHeader("\\\\$", custom_variables);
            Assert.IsTrue(pr.isSuccess());
            Assert.AreEqual(pr.getResult(), "\\$");
        }

        [TestMethod]
        public void complexParseHeader()
        {
            pr = Parser.parseHeader("Simple $unknown_var$ string. It costs $x$\\$ and $ is money.", custom_variables);
            Assert.IsFalse(pr.isSuccess());
            Assert.AreEqual(pr.getResult(), "Simple $unknown_var$ string. It costs 42$ and $ is money.");
            Assert.AreEqual(pr.getErrors().Count, 2); // one error for unknown variable and the other for unescaped dollar char at the end missing it's pair
        }
    }
}
