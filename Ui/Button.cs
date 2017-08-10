﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Game;
using Game.Common;
using Game.Models;
using Game.Rendering;
using OpenTK;
using OpenTK.Graphics;

namespace Ui
{
    public class Button : BranchElement, IElement
    {
        public delegate void ClickHandler();
        public event ClickHandler OnClick;

        public Vector2 Size { get; set; }

        public Func<ElementArgs, Transform2> Transform { get; }

        public bool Hidden { get; set; }

        public Button(Func<ElementArgs, Transform2> transform = null, Vector2 size = new Vector2(), Action onClick = null)
        {
            Transform = transform ?? (_ => new Transform2());
            Size = size;
            if (onClick != null)
            {
                OnClick += () => { onClick(); };
            }
        }

        public Button(out Button id, Func<ElementArgs, Transform2> transform = null, Vector2 size = new Vector2(), Action onClick = null)
            : this(transform, size, onClick)
        {
            id = this;
        }

        public void Click()
        {
            OnClick?.Invoke();
        }

        public List<Model> GetModels()
        {
            return new[]
            {
                ModelFactory.CreatePlane(Size, Color4.Black),
            }.ToList();
        }

        public bool IsInside(Vector2 localPoint)
        {
            return MathEx.PointInRectangle(new Vector2(), Size, localPoint);
        }
    }
}
