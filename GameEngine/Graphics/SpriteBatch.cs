using GameEngine.Graphics.Shapes;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Graphics
{
    public class SpriteBatch : IDisposable
    {

        private bool _disposedValue;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private List<float> vertices = new();
        private List<Triangle> triangles = new();

        public SpriteBatch() 
        {
        }

        public void UpdateData() 
        {
            vertices.Clear();
            foreach(var triangle in  triangles)
            {
                foreach (var vertex in triangle.Vertices)
                {
                    vertices.Add(vertex.X);
                    vertices.Add(vertex.Y);
                    vertices.Add(vertex.Z);
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.DynamicDraw);
        }

        public void Add(Triangle triangle) // todo: change to a tringle class
        {
            triangles.Add(triangle);

            foreach(var vertex in triangle.Vertices)
            {
                vertices.Add(vertex.X);
                vertices.Add(vertex.Y);
                vertices.Add(vertex.Z);
            }
            //Update buffer data to fit newly added vertices
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);
        }

        public void Load()
        {
            // create vertex buffer object -> we need to create a buffer object to store the vertices in the GPU memory
            _vertexBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            // create vertex array object -> we need to create a vertex array object to store the vertex attribute pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    GL.DeleteBuffer(_vertexBufferObject);
                    GL.DeleteVertexArray(_vertexArrayObject);
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
