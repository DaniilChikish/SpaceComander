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
        public void FormatIntTest()
        {
            string input = "111";
            int expected = 111;
            Assert.AreEqual(expected, ValidString.FormatInt(input));
        }
        [TestMethod()]
        public void MultipleFormatIntTest()
        {
            string[] input = { "111", "q111", "111q", "1q1q1" };
            int assert = 111;
            bool passed = true;
            foreach (string x in input)
            {
                if (ValidString.FormatInt(x) != assert)
                    passed = false;
            }
            Assert.AreEqual(true, passed);
        }
    }
}