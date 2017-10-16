using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeusUtility.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusUtility.UI.Tests
{
    [TestClass()]
    public class ValidStringTests
    {
        [TestMethod()]
        public void FormatUnsignedTest()
        {
            string input = "-111";
            int expected = 111;
            Assert.AreEqual(expected, ValidString.FormatUnsigned(input));
        }
        [TestMethod()]
        public void MultipleFormatUnsignedTest()
        {
            string[] input = { "111", "q111", "111q", "1q1q1" };
            int assert = 111;
            bool passed = true;
            foreach (string x in input)
            {
                if (ValidString.FormatUnsigned(x) != assert)
                    passed = false;
            }
            Assert.AreEqual(true, passed);
        }
    }
}