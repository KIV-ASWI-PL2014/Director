using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Director.ParserLib;
using System.Collections.Generic;
using System.Globalization;

namespace UnitTestParser
{
    [TestClass]
    public class TestRequest
    {

        Parser parser = new Parser();

        [TestMethod]
        public void RandInt1()
        { 
            ParserResult pr;
            String result;
            pr = parser.generateRequest("{ \"foo\": \"#randInt(4, 4)#\" }", null);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");

            Assert.AreEqual(result, "{\"foo\":4}");
            
        }

        [TestMethod]
        public void RandInt2()
        {
            ParserResult pr;
            String result;           

            Boolean []res = {false, false};
            int i = 10;
            while(i-- > 0 && (!res[0] || !res[1])){
                pr = parser.generateRequest("{ \"foo\": \"#randInt(1, 2)#\" }", null);
                Assert.IsNotNull(pr);
                Assert.IsTrue(pr.isSuccess());
                result = pr.getResult().Replace(" ", "");
                if(result.Equals("{\"foo\":1}"))
                    res[0] = true;
                else if(result.Equals("{\"foo\":2}"))
                    res[1] = true;
            }

            if (!res[0] || !res[1]) 
            {
                Assert.Fail("Rand int method does not generate distinct values.");
            }
      
        }

        [TestMethod]
        public void StringRandIntString()
        {
            ParserResult pr;
            String result;
            pr = parser.generateRequest("{ \"foo\": \"ab#randInt(1, 1)#de\" }", null);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");
            Assert.AreEqual(result, "{\"foo\":\"ab1de\"}");

        }

        [TestMethod]
        public void RandFloat()
        {
            
            ParserResult pr;
            String result;
            pr = parser.generateRequest("{ \"foo\": \"#randFloat(5,7,6)#\" }", null);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");
            result = result.Substring(result.IndexOf(":") + 1);
            result = result.Replace("}", "");

            Assert.IsTrue(result.Length <= "6.123456".Length);

            float dec = float.Parse(result, CultureInfo.InvariantCulture.NumberFormat);
            if (dec < 5 || dec > 7)
                Assert.Fail("Value out of range");
        }

        [TestMethod]
        public void RandString1()
        {
            ParserResult pr;
            String result;
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"a", "2"},
                {"b", "3"},
                {"c", "AB"},
            };

            pr = parser.generateRequest("{ \"foo\": \"$c$#randString($a$, $b$, A)#$c$\" }", values);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");
            result = result.Substring(result.IndexOf(":") + 1);
            result = result.Replace("}", "");

            Assert.AreEqual(result, result.ToUpper());
            if(result.Length != 8 && result.Length != 9)
                Assert.Fail("Wrong length of generated string.");
        }

        [TestMethod]
        public void TestOnlyVariables()
        {
            ParserResult pr;
            String result;
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"token", "2"}
            };

            pr = parser.generateRequest("{ \"foo\": \"$token$\" }", values);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");
            result = result.Substring(result.IndexOf(":") + 1);
            result = result.Replace("}", "");
            Assert.AreEqual("\"2\"", result);
        }


        [TestMethod]
        public void SequenceTestBasic()
        {
            ParserResult pr;
            String result;
            String currentSequenceVal;
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"curr", "10"},
            };
            pr = parser.generateRequest("{ \"foo\": \"#sequence(10, 4, -2, curr)#\" }", values);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");
            result = result.Substring(result.IndexOf(":") + 1);
            result = result.Replace("}", "");
            Assert.AreEqual(result, "8");
            values.TryGetValue("curr", out currentSequenceVal);
            Assert.AreEqual(currentSequenceVal, "8");
            Assert.AreEqual(values.Count, 1);
        }

        [TestMethod]
        public void SequenceTestNoVariable()
        {
            ParserResult pr;
            String result;
            String currentSequenceVal;
            Dictionary<string, string> values = new Dictionary<string, string>();
            pr = parser.generateRequest("{ \"foo\": \"#sequence(10, 4, -2, curr)#\" }", values);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");
            result = result.Substring(result.IndexOf(":") + 1);
            result = result.Replace("}", "");
            Assert.AreEqual(result, "10");
            values.TryGetValue("curr", out currentSequenceVal);
            Assert.AreEqual(currentSequenceVal, "10");
        }

        [TestMethod]
        public void SequenceReverseMinMax()
        {
            ParserResult pr;
            String result;
            String currentSequenceVal;
            Dictionary<string, string> values = new Dictionary<string, string>();
            pr = parser.generateRequest("{ \"foo\": \"#sequence(10, 0, 5, curr)#\" }", values);
            pr = parser.generateRequest("{ \"foo\": \"#sequence(10, 0, 5, curr)#\" }", values);
            Assert.IsNotNull(pr);
            Assert.IsTrue(pr.isSuccess());
            result = pr.getResult().Replace(" ", "");
            result = result.Substring(result.IndexOf(":") + 1);
            result = result.Replace("}", "");
            Assert.AreEqual(result, "10");
            values.TryGetValue("curr", out currentSequenceVal);
            Assert.AreEqual(currentSequenceVal, "10");
            Assert.AreEqual(values.Count, 1);
        }

        [TestMethod]
        public void SequenceWithoutMax()
        {
            ParserResult pr;
            String result;
            String currentSequenceVal;
            Dictionary<string, string> values = new Dictionary<string, string>();
            int repeat = 5;
            while (repeat-- > 0)
            {
                pr = parser.generateRequest("{ \"foo\": \"#sequence(4, , -1, curr)#\" }", values);
                Assert.IsNotNull(pr);
                Assert.IsTrue(pr.isSuccess());
                result = pr.getResult().Replace(" ", "");
                result = result.Substring(result.IndexOf(":") + 1);
                result = result.Replace("}", "");
                Assert.AreEqual(result, Convert.ToString(repeat));
                values.TryGetValue("curr", out currentSequenceVal);
                Assert.AreEqual(currentSequenceVal, Convert.ToString(repeat));
                Assert.AreEqual(values.Count, 1);
            }
        }

        [TestMethod]
        public void SequenceWithDoubleVariable()
        {
            ParserResult pr;
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"curr", "10.5"},
            };
            pr = parser.generateRequest("{ \"foo\": \"#sequence(11, 10, -0.1, curr)#\" }", values);
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());
        }

        [TestMethod]
        public void SequenceWithStringVariable()
        {
            ParserResult pr;
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"curr", "xx"},
            };
            pr = parser.generateRequest("{ \"foo\": \"#sequence(1, 10, 2, curr)#\" }", values);
            Assert.IsNotNull(pr);
            Assert.IsFalse(pr.isSuccess());
        }
    }
}
