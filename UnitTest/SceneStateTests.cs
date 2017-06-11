﻿using Game.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimeLoopInc;

namespace GameTests
{
    [TestFixture]
    public class SceneStateTests
    {
        [Test]
        public void DeepCloneTest0()
        {
            var sceneState = new SceneState();
            var clone = sceneState.DeepClone();

            AreEqual(sceneState, clone);
        }

        [Test]
        public void DeepCloneTest1()
        {
            var sceneState = new SceneState();
            var blockTimeline = new Timeline<Block>();
            blockTimeline.Add(new Block(new Transform2i(new Vector2i(1, 1)), 2, 2));
            sceneState.BlockTimelines.Add(blockTimeline);
            var clone = sceneState.DeepClone();

            AreEqual(sceneState, clone);
        }

        static void AreEqual(SceneState expected, SceneState result)
        {
            Assert.IsFalse(ReferenceEquals(expected, result));
            using (var stream0 = new MemoryStream())
            using (var stream1 = new MemoryStream())
            {
                new Serializer().Serialize(stream0, expected);
                new Serializer().Serialize(stream1, result);

                Assert.IsTrue(CompareMemoryStreams(stream0, stream1));
            }
        }

        static bool CompareMemoryStreams(MemoryStream ms1, MemoryStream ms2)
        {
            if (ms1.Length != ms2.Length)
                return false;
            ms1.Position = 0;
            ms2.Position = 0;

            var msArray1 = ms1.ToArray();
            var msArray2 = ms2.ToArray();

            return msArray1.SequenceEqual(msArray2);
        }

        [Test]
        public void EmptySceneStateStartTime()
        {
            Assert.AreEqual(0, new SceneState().StartTime);
        }

        [Test]
        public void GetStateInstantTest0()
        {
            var block = new Block(new Transform2i(), 0);
            var scene = new Scene(new HashSet<Vector2i>(), new List<TimePortal>(), null, new[] { block });

            var result = scene.GetStateInstant(0).Entities.Count;
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetStateInstantTest1()
        {
            var block = new Block(new Transform2i(), 0);
            var scene = new Scene(new HashSet<Vector2i>(), new List<TimePortal>(), null, new[] { block });

            var result = scene.GetStateInstant(1).Entities.Count;
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetStateInstantTest2()
        {
            var block = new Block(new Transform2i(), 0);
            var scene = new Scene(new HashSet<Vector2i>(), new List<TimePortal>(), null, new[] { block });

            var result = scene.GetStateInstant(-1).Entities.Count;
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetStateInstantTest3()
        {
            var block = new Block(new Transform2i(), 2);
            var scene = new Scene(new HashSet<Vector2i>(), new List<TimePortal>(), null, new[] { block });

            Assert.AreEqual(0, scene.GetStateInstant(1).Entities.Count);
        }

        [Test]
        public void GetStateInstantTest4()
        {
            var blocks = new[] {
                new Block(new Transform2i(new Vector2i(2, 0)), 0, 1),
                new Block(new Transform2i(new Vector2i(2, 1)), 1, 1),
                new Block(new Transform2i(new Vector2i(2, 2)), 2, 1),
            };
            var scene = new Scene(new HashSet<Vector2i>(), new List<TimePortal>(), null, blocks);

            Assert.AreEqual(3, scene.GetStateInstant(10).Entities.Count);
        }

        [Test]
        public void RotationRoundingBug()
        {
            var player = new Player(new Transform2i(gridRotation: new GridAngle(5)), 0);
            var scene = new Scene(new HashSet<Vector2i>(), new List<TimePortal>(), player, new List<Block>());

            scene.Step(new Input(new GridAngle()));

            Assert.AreEqual(5, scene.State.CurrentInstant.Entities[player].Transform.Direction.Value);
            Assert.AreEqual(5 * Math.PI / 2, scene.State.CurrentInstant.Entities[player].Transform.Angle, 0.0000001);
        }
    }
}