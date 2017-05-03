﻿using System.Collections.Generic;
using Game.Serialization;
using System.Linq;
using Game.Common;
using OpenTK;
using System.Diagnostics;

namespace Game.Models
{
    public interface IMesh : IShallowClone<IMesh>
    {
        /// <summary>
        /// Get a shallow copy of list containing vertices in the mesh.
        /// </summary>
        List<Vertex> GetVertices();
        /// <summary>
        /// Get a shallow copy of list containing indices in the mesh.  Every 3 indices defines a triangle.
        /// </summary>
        List<int> GetIndices();
    }

    public static class IMeshEx
    {
        public static bool IsValid(this IMesh mesh)
        {
            var vertices = mesh.GetVertices();
            var indices = mesh.GetIndices();
            return indices.Count % 3 == 0 && 
                vertices.All(item => item != null) &&
                (indices.Count == 0 ||
                (indices.Max() < vertices.Count() && indices.Min() >= 0));
        }

        public static Mesh Bisect(this IMesh mesh, LineF bisector, Side keepSide = Side.Left)
        {
            return Bisect(mesh, bisector, Matrix4.Identity, keepSide);
        }

        public static Mesh Bisect(this IMesh mesh, LineF bisector, Matrix4 transform, Side keepSide = Side.Left)
        {
            Debug.Assert(bisector != null);
            Debug.Assert(keepSide != Side.Neither);
            Triangle[] triangles = GetTriangles(mesh);

            var meshBisected = new Mesh();
            for (int i = 0; i < triangles.Length; i++)
            {
                Triangle[] bisected = triangles[i].Transform(transform).Bisect(bisector, keepSide);
                meshBisected.AddTriangleRange(bisected);
            }
            meshBisected.RemoveDuplicates();
            meshBisected.Transform(transform.Inverted());
            return meshBisected;
        }

        public static Triangle[] GetTriangles(this IMesh mesh)
        {
            List<Vertex> vertices = mesh.GetVertices();
            List<int> indices = mesh.GetIndices();
            Triangle[] triangles = new Triangle[indices.Count / 3];
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = new Triangle(
                    vertices[indices[i * 3]],
                    vertices[indices[i * 3 + 1]],
                    vertices[indices[i * 3 + 2]]);
            }
            return triangles;
        }
    }
}
