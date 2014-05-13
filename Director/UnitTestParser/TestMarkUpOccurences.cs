using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Director.ParserLib;

namespace UnitTestParser
{
    [TestClass]
    public class TestMarkUpOccurences
    {

        Dictionary<string, string> custom_variables;
        List<ParserOccurence> occurences;
        List<ParserError> errors;

        public TestMarkUpOccurences()
        {
            custom_variables = new Dictionary<string, string>();
            custom_variables.Add("first", "second");
            custom_variables.Add("x", "42");

            errors = new List<ParserError>();
        }

        [TestMethod]
        public void simpleMarkUpOccurences()
        {
            // test 1
            occurences = Parser.findMarkUpOccurences("ahoj", null, null);

            Assert.IsNotNull(occurences);
            Assert.AreEqual(occurences.Count, 1);

            Assert.AreEqual(occurences[0].name, "ahoj");
            Assert.AreEqual(occurences[0].type, "text");
            Assert.AreEqual(occurences[0].arguments, null);

            // test 2
            occurences = Parser.findMarkUpOccurences("$first$ thing and #randString(1,$x$,A1)# for sure", custom_variables, errors);

            Assert.IsNotNull(occurences);
            Assert.AreEqual(errors.Count, 0);
            Assert.AreEqual(occurences.Count, 4);

            Assert.AreEqual(occurences[0].name, "first");
            Assert.AreEqual(occurences[0].type, "variable");
            Assert.AreEqual(occurences[0].arguments, null);

            Assert.AreEqual(occurences[1].name, " thing and ");
            Assert.AreEqual(occurences[1].type, "text");
            Assert.AreEqual(occurences[1].arguments, null);

            Assert.AreEqual(occurences[2].name, "randString");
            Assert.AreEqual(occurences[2].type, "function");
            Assert.AreEqual(occurences[2].arguments[0], "1");
            Assert.AreEqual(occurences[2].arguments[1], "$x$");
            Assert.AreEqual(occurences[2].arguments[2], "A1");

            Assert.AreEqual(occurences[3].name, " for sure");
            Assert.AreEqual(occurences[3].type, "text");
            Assert.AreEqual(occurences[3].arguments, null);

        }

        [TestMethod]
        public void complexMarkUpOccurences()
        {
            // test 1
            occurences = Parser.findMarkUpOccurences("$first$$unknown$ \\$\\#\\$thing $first$ and #randString( 1,  ,A1  )# for sure. \\#And other #randString$x$,A1)#", custom_variables, errors);

            Assert.IsNotNull(occurences);
            Assert.AreEqual(errors.Count, 2);
            Assert.AreEqual(occurences.Count, 8);

            Assert.AreEqual(occurences[0].name, "first");
            Assert.AreEqual(occurences[0].type, "variable");
            Assert.AreEqual(occurences[0].arguments, null);

            Assert.AreEqual(occurences[1].name, "unknown");
            Assert.AreEqual(occurences[1].type, "variable");
            Assert.AreEqual(occurences[1].arguments, null);

            Assert.AreEqual(occurences[2].name, " \\$\\#\\$thing ");
            Assert.AreEqual(occurences[2].type, "text");
            Assert.AreEqual(occurences[2].arguments, null);

            Assert.AreEqual(occurences[3].name, "first");
            Assert.AreEqual(occurences[3].type, "variable");
            Assert.AreEqual(occurences[3].arguments, null);

            Assert.AreEqual(occurences[4].name, " and ");
            Assert.AreEqual(occurences[4].type, "text");
            Assert.AreEqual(occurences[4].arguments, null);

            Assert.AreEqual(occurences[5].name, "randString");
            Assert.AreEqual(occurences[5].type, "function");
            Assert.AreEqual(occurences[5].arguments[0], "1");
            Assert.AreEqual(occurences[5].arguments[1], "");
            Assert.AreEqual(occurences[5].arguments[2], "A1");

            Assert.AreEqual(occurences[6].name, " for sure. \\#And other ");
            Assert.AreEqual(occurences[6].type, "text");
            Assert.AreEqual(occurences[6].arguments, null);

            Assert.AreEqual(occurences[7].name, "randString$x$,A1)");
            Assert.AreEqual(occurences[7].type, "function");
            Assert.AreEqual(occurences[7].arguments, null);
        }
    }
}
