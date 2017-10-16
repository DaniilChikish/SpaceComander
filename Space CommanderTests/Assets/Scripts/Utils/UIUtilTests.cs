using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace DeusUtility.UI.Tests
{
    [TestClass()]
    public class UIUtilTests
    {
        [TestMethod()]
        public void GetRatioTest()
        {
            //arrange
            Vector2 rectSize = new Vector2(1920, 1080);
            Vector2 rectPropExpected = new Vector2(16, 9);
            //act
            Vector2 rectProp = UIUtil.GetRatio(rectSize);
            //accert
            Assert.AreEqual(rectPropExpected, rectProp);
        }

        [TestMethod()]
        public void GetRectTest()
        {
            //arrange
            Vector2 parentRectSize = new Vector2(1000, 1000);
            Vector2 RectSize = new Vector2(100, 100);
            Rect rectExpected = new Rect(new Vector2(500, 500), RectSize);
            //act
            Rect rectAccert = UIUtil.GetRect(RectSize, PositionAnchor.Center, parentRectSize, new Vector2(50, 50));
            //accert
            Assert.AreEqual(rectExpected, rectAccert);
        }
    }
}