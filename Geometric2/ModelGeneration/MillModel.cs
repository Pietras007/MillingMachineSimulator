using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Geometric2.Functions;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public class MillModel : Element
    {
        public float[,] topLayer { get; set; }
        public int MillModelTopLayerVBO, MillModelTopLayerVAO, MillModelTopLayerEBO;
        private int TopLayerSize = 1000;
        private float[] TopLayerPoints;
        uint[] TopLayerIndices;
        Texture texture;
        Texture specular;
        float divider = 100.0f;
        Texture heightmap;
        int width, height;

        public MillModel(int width, int height)
        {
            topLayer = new float[width, height];
            CenterPosition = new Vector3(0, 0, 0);
            this.width = width;
            this.height = height;
        }

        public override void CreateGlElement(Shader _shader)
        {
            texture = new Texture("./../../Resources/wood.jpg");
            specular = new Texture("./../../Resources/50specular.png");
            RegenerateTexture();
            GenerateTopLevel();
            MillModelTopLayerVAO = GL.GenVertexArray();
            MillModelTopLayerVBO = GL.GenBuffer();
            MillModelTopLayerEBO = GL.GenBuffer();

            GL.BindVertexArray(MillModelTopLayerVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MillModelTopLayerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TopLayerPoints.Length * sizeof(float), TopLayerPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MillModelTopLayerEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, TopLayerIndices.Length * sizeof(uint), TopLayerIndices, BufferUsageHint.DynamicDraw);
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
            var aNormal = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(aNormal);
            GL.VertexAttribPointer(aNormal, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            var aTexCoords = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(aTexCoords);
            GL.VertexAttribPointer(aTexCoords, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            CreateCenterOfElement(_shader);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre)
        {
            _shader.Use();
            RegenerateTexture();
            Translation = new Vector3(-width / (2 * 100.0f), 0, -height / (2 * 100.0f)); ;
            Matrix4 model = ModelMatrix.CreateModelMatrix(new Vector3(width / 1000.0f, 1, height / 1000.0f), RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(MillModelTopLayerVAO);
            texture.Use();
            specular.Use(TextureUnit.Texture1);
            heightmap.Use(TextureUnit.Texture2);
            GL.DrawElements(PrimitiveType.Triangles, 3 * TopLayerPoints.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            RenderCenterOfElement(_shader);
        }

        private void RegenerateTexture()
        {
            if (heightmap != null)
            {
                heightmap.DeleteTexture();
            }

            float[] layer = new float[topLayer.Length];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var x = topLayer[i, j];
                    layer[j * width + i] = x;
                }
            }

            heightmap = new Texture(width, height, layer);
        }

        public void DrillAll(List<Vector3> loadedPositions)
        {
            Thread thread = new Thread(() =>
            {
                List<Vector3> processedPoints = new List<Vector3>();
                foreach (var p in loadedPositions)
                {
                    Vector3 point = new Vector3(p.X * 100 + 1000, p.Y, p.Z * 100 + 2000);
                    processedPoints.Add(point);
                }

                Vector3 prev = processedPoints.First();
                for (int i = 1; i < processedPoints.Count; i++)
                {
                    Vector3 current = processedPoints[i];
                    Brezenham((int)prev.X, (int)prev.Z, (int)current.X, (int)current.Z, current.Y, prev.Y, current.Y);
                    prev = current;
                }
            });
            thread.Start();
        }

        public void Brezenham(int x, int y, int x2, int y2, float height, float z_From, float z_To)
        {
            if (x != x2 || y != y2)
            {
                int w = x2 - x;
                int h = y2 - y;
                int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
                if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
                if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
                if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
                int longest = Math.Abs(w);
                int shortest = Math.Abs(h);
                if (!(longest > shortest))
                {
                    longest = Math.Abs(h);
                    shortest = Math.Abs(w);
                    if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                    dx2 = 0;
                }
                int numerator = longest >> 1;
                for (int i = 0; i <= longest; i++)
                {
                    DrillHole(new Vector3(x, height, y), 50);
                    numerator += shortest;
                    if (!(numerator < longest))
                    {
                        numerator -= longest;
                        x += dx1;
                        y += dy1;
                    }
                    else
                    {
                        x += dx2;
                        y += dy2;
                    }
                }
            }
            else
            {
                if (z_From <= z_To)
                {
                    for (float i = z_From; i < z_To; i += 0.01f)
                    {
                        //wait
                    }
                }
                else
                {
                    for (float i = z_From; i >= z_To; i -= 0.01f)
                    {
                        //wait
                    }
                }

                DrillHole(new Vector3(x, height, y), 50);
            }
        }

        public void DrillHole(Vector3 point, int radius)
        {
            int x = (int)point.X;
            int y = (int)point.Y;
            int z = (int)point.Z;

            for (int _y = -radius; _y <= radius; _y++)
            {
                for (int _x = -radius; _x <= radius; _x++)
                {
                    if (_x * _x + _y * _y <= radius * radius)
                    {
                        if (topLayer[x + _x, z + _y] > point.Y)
                        {
                            topLayer[x + _x, z + _y] = point.Y;
                        }
                    }
                }
            }
        }

        private void GenerateTopLevel()
        {
            TopLayerPoints = new float[8 * TopLayerSize * TopLayerSize];
            var TopLayerPointsHelper = new float[8 * TopLayerSize * TopLayerSize];
            TopLayerIndices = new uint[6 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            int idx = 0;
            int indiceidx = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    topLayer[i, j] = 5;
                }
            }

            for (int i = 0; i < TopLayerSize; i++)
            {
                for (int j = 0; j < TopLayerSize; j++)
                {
                    TopLayerPointsHelper[idx] = i / divider; idx++; //x
                    TopLayerPointsHelper[idx] = topLayer[i, j]; idx++; //y
                    TopLayerPointsHelper[idx] = j / divider; idx++; //z

                    TopLayerPointsHelper[idx] = 0; idx++; //norm x
                    TopLayerPointsHelper[idx] = 1; idx++; //norm y
                    TopLayerPointsHelper[idx] = 0; idx++; //norm z

                    TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                    TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy

                    if (i < TopLayerSize - 1 && j < TopLayerSize - 1)
                    {
                        TopLayerIndices[indiceidx] = (uint)(i * TopLayerSize + j); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(i * TopLayerSize + j + 1); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerSize + j); indiceidx++;

                        TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerSize + j); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(i * TopLayerSize + j + 1); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerSize + j + 1); indiceidx++;
                    }
                }
            }

            TopLayerPoints = TopLayerPointsHelper;
        }
    }
}
