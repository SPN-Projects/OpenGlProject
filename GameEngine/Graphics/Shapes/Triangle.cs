using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Graphics.Shapes
{
    public class Triangle
    {
        Shader shader;

        Color Color;
        float Alpha = 1.0f;
        Vector3 Position = Vector3.Zero;
        private Vector3[] _unscaledVertices = new Vector3[3];
        public Vector3[] Vertices = new Vector3[3];


        private float size;

        public float Size
        {
            get => size;
            set
            {
                if (value < 0)
                    return;

                for (int i = 0; i < 3; i++)
                {
                    Vertices[i].X = Position.X + (_unscaledVertices[i].X - Position.X) * value;
                    Vertices[i].Y = Position.Y + (_unscaledVertices[i].Y - Position.Y) * value;
                    Vertices[i].Z = Position.Z + (_unscaledVertices[i].Z - Position.Z) * value;
                }

                size = value;
            }
        }


        public Triangle(float[] vertices_)
        {
            Size = 1.0f;
            if(vertices_.Length > 0) 
            {
                _unscaledVertices[0] = new Vector3(vertices_[0], vertices_[1], vertices_[2]);
                _unscaledVertices[1] = new Vector3(vertices_[3], vertices_[4], vertices_[5]);
                _unscaledVertices[2] = new Vector3(vertices_[6], vertices_[7], vertices_[8]);

                Vertices[0] = new Vector3(vertices_[0], vertices_[1], vertices_[2]);
                Vertices[1] = new Vector3(vertices_[3], vertices_[4], vertices_[5]);
                Vertices[2] = new Vector3(vertices_[6], vertices_[7], vertices_[8]);

                CalculateCenter();
            }


        } 
        
        public void CalculateCenter()
        {
            var x = (Vertices[0].X + Vertices[1].X + Vertices[2].X) / 3;
            var y = (Vertices[0].Y + Vertices[1].Y + Vertices[2].Y) / 3;
            var z = (Vertices[0].Z + Vertices[1].Z + Vertices[2].Z) / 3;

            Position = new Vector3(x, y, z);
        }


    }
}
