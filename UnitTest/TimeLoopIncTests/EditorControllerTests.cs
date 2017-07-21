﻿using Game.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLoopInc;
using TimeLoopInc.Editor;

namespace TimeLoopIncTests
{
    [TestFixture]
    public class EditorControllerTests
    {
        [Test]
        public void PortalValidEdgesTest0()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 4),
                new Vector2i(2, 4),
                new Vector2i(1, 4)
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle>();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PortalValidEdgesTest1()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 5),
                new Vector2i(2, 5),
                new Vector2i(1, 5)
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle> { GridAngle.Up };
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PortalValidEdgesTest2()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 5),
                new Vector2i(2, 5),
                new Vector2i(1, 5),
                new Vector2i(3, 3),
                new Vector2i(2, 3),
                new Vector2i(1, 3)
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle> { GridAngle.Up, GridAngle.Down };
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PortalValidEdgesTest3()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 5),
                new Vector2i(2, 5),
                new Vector2i(1, 5),
                new Vector2i(3, 4)
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle>();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PortalValidEdgesTest4()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 5),
                new Vector2i(3, 4),
                new Vector2i(3, 3)
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle> { GridAngle.Right };
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PortalValidEdgesTest5()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 5),
                new Vector2i(3, 4),
                new Vector2i(3, 3),
                new Vector2i(2, 3)
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle>();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PortalValidEdgesTest6()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 5),
                new Vector2i(3, 4),
                new Vector2i(3, 3),
                new Vector2i(1, 5),
                new Vector2i(1, 4),
                new Vector2i(1, 3),
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle> { GridAngle.Left, GridAngle.Right };
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void PortalValidEdgesTest7()
        {
            var walls = new HashSet<Vector2i>
            {
                new Vector2i(3, 5),
                new Vector2i(3, 4),
            };

            var result = EditorController.PortalValidSides(new Vector2i(2, 4), walls);
            var expected = new HashSet<GridAngle>();
            Assert.AreEqual(expected, result);
        }
    }
}
