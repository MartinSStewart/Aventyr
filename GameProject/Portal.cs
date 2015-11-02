﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class Portal
    {
        public Scene Scene { get; private set; }
        public Portal Linked { get; private set; }
        /// <summary>
        /// The distance at which an entity enters and exits a portal.  
        /// It is used to avoid situations where an entity can skip over a portal by sitting exactly on top of it.
        /// </summary>
        public const float EnterMinDistance = 0.001f;
        public abstract Entity EntityParent { get; }
        /// <summary>
        /// If OneSided is true then the portal can only be viewed through it's front side.
        /// Entities can still travel though the portal in both directions however.
        /// </summary>
        public bool OneSided { get; set; }
        private Exception _nullScene = new Exception("Portal must be assigned to a scene.");
        public Portal(Scene scene)
        {
            if (scene == null)
            {
                throw _nullScene;
            }
            OneSided = true;
            Scene = scene;
            Scene.PortalList.Add(this);
        }
        public abstract Transform2D GetTransform();

        public Vector2[] GetFOV(Vector2 origin, float distance)
        {
            return GetFOV(origin, distance, 10);
        }

        /// <summary>
        /// Converts a Transform2D from one portal's coordinate space to the portal it is linked with.  If it isn't linked then the Transform2D is unchanged
        /// </summary>
        /// <param name="entityPos"></param>
        public void Enter(Transform2D entityPos)
        {
            Matrix4 m = GetPortalMatrix();
            Vector2 v0 = VectorExt2.Transform(entityPos.Position, m);
            Vector2 v1 = VectorExt2.Transform(entityPos.Position + new Vector2(1, 0), m);
            Vector2 v2 = VectorExt2.Transform(entityPos.Position + new Vector2(0, 1), m);

            entityPos.Position = new Vector2(v0.X, v0.Y);

            Transform2D tEnter = GetTransform();
            Transform2D tExit = Linked.GetTransform();
            float flipX = 1;
            float flipY = 1;
            if (Math.Sign(tEnter.WorldScale.X) == Math.Sign(tExit.WorldScale.X))
            {
                flipX = -1;
            }
            if (Math.Sign(tEnter.WorldScale.Y) != Math.Sign(tExit.WorldScale.Y))
            {
                flipY = -1;
            }
            entityPos.Scale *= new Vector2(flipX * (v1 - v0).Length, flipY * (v2 - v0).Length);

            float angle;
            if (flipX != flipY)
            {
                entityPos.Rotation = -entityPos.Rotation;
                entityPos.Rotation += (float)(MathExt.AngleWrap(GetTransform().WorldRotation) + MathExt.AngleWrap(Linked.GetTransform().WorldRotation));
            }
            else
            {
                angle = Linked.GetTransform().WorldRotation - GetTransform().WorldRotation;
                entityPos.Rotation += angle;
            }
        }

        public void Enter(Transform entity)
        {
            Transform2D entity2D = entity.GetTransform2D();
            Enter(entity2D);
            entity.Rotation = new Quaternion(entity.Rotation.X, entity.Rotation.Y, entity.Rotation.Z, entity2D.Rotation);
            entity.Position = new Vector3(entity2D.Position.X, entity2D.Position.Y, entity.Position.Z);
            entity.Scale = new Vector3(entity2D.Scale.X, entity2D.Scale.Y, entity.Scale.Z);
        }

        public static void ConnectPortals(Portal portal0, Portal portal1)
        {
            portal0.Linked = portal1;
            portal1.Linked = portal0;
        }

        private void SetPortal(Portal portal)
        {
            if (Linked != portal)
            {
                if (Linked != null)
                {
                    Linked.SetPortal(null);
                }
                Linked = portal;
                if (Linked != null)
                {
                    Linked.SetPortal(this);
                }
            }
        }

        /// <summary>
        /// Returns an array of two Vectors defining the Portals local location
        /// </summary>
        public Vector2[] GetVerts()
        {
            return new Vector2[] { new Vector2(0, 0.5f), new Vector2(0, -0.5f) };
        }

        public Vector2[] GetWorldVerts()
        {
            return VectorExt2.Transform(GetVerts(), GetTransform().GetWorldMatrix());
        }

        public Matrix4 GetPortalMatrix()
        {
            Debug.Assert(Linked != null, "Portal must be linked to another portal.");
            return GetPortalMatrix(this, Linked);
        }

        /// <summary>
        /// Returns matrix to transform between one portals coordinate space to another
        /// </summary>
        public static Matrix4 GetPortalMatrix(Portal portalEnter, Portal portalExit)
        {
            //The portalExit is temporarily mirrored before getting the transformation matrix
            Transform2D transform = portalExit.GetTransform();
            Vector2 v = transform.Scale;
            transform.Scale = new Vector2(-v.X, v.Y);
            Matrix4 m = portalEnter.GetTransform().GetWorldMatrix().Inverted() * transform.GetWorldMatrix();
            transform.Scale = v;
            return m;
        }

        /// <summary>
        /// Returns a polygon representing the 2D FOV through the portal.  If the polygon is degenerate then an array of length 0 will be returned.
        /// </summary>
        public Vector2[] GetFOV(Vector2 viewPoint, float distance, int detail)
        {
            Matrix4 a = GetTransform().GetWorldMatrix();
            Vector2[] verts = new Vector2[detail + 2];
            Vector2[] portal = GetVerts();
            for (int i = 0; i < portal.Length; i++)
            {
                Vector4 b = Vector4.Transform(new Vector4(portal[i].X, portal[i].Y, 0, 1), a);
                verts[i] = new Vector2(b.X, b.Y);
            }
            //minumum distance in order to prevent self intersections
            const float errorMargin = 0.01f;
            float distanceMin = Math.Max((verts[0] - viewPoint).Length, (verts[1] - viewPoint).Length) + errorMargin;
            distance = Math.Max(distance, distanceMin);
            //get the leftmost and rightmost edges of the FOV
            verts[verts.Length - 1] = (verts[0] - viewPoint).Normalized() * distance + viewPoint;
            verts[2] = (verts[1] - viewPoint).Normalized() * distance + viewPoint;
            //find the angle between the edges of the FOV
            double angle0 = MathExt.AngleLine(verts[verts.Length - 1], viewPoint);
            double angle1 = MathExt.AngleLine(verts[2], viewPoint);
            double diff = MathExt.AngleDiff(angle0, angle1);
            Debug.Assert(diff <= Math.PI + double.Epsilon && diff >= -Math.PI);
            //handle case where lines overlap eachother
            const double angleDiffMin = 0.0001f;
            if (Math.Abs(diff) < angleDiffMin)
            {
                return new Vector2[0];
            }

            Matrix2 Rot = Matrix2.CreateRotation((float)diff / (detail - 1));
            for (int i = 3; i < verts.Length - 1; i++)
            {
                verts[i] = MathExt.Matrix2Mult(verts[i - 1] - viewPoint, Rot) + viewPoint;
            }
            return verts;
        }
    }
}
