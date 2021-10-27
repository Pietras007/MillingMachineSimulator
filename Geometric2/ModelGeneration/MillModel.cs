using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
        private int TextureDensity = 100;
        private float[] TopLayerPoints;
        uint[] TopLayerIndices;
        int counter = 0;
        Random random = new Random(5000);
        Texture texture;
        Texture specular;

        float divider = 10.0f;

        //Bitmap heightBitmap;
        //BitmapData bitmapDataForHeightmap;
        //IntPtr heightMapPointer;
        Texture heightmap;




        public float torus_R = 2.2f;
        public float torus_r = 1.4f;
        public int torusMajorDividions = 6;
        public int torusMinorDividions = 6;
        public int torusNumber;
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
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 14 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
            var aNormal = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(aNormal);
            GL.VertexAttribPointer(aNormal, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 3 * sizeof(float));
            var aTexCoords = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(aTexCoords);
            GL.VertexAttribPointer(aTexCoords, 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 6 * sizeof(float));
            var heightCoords1 = _shader.GetAttribLocation("heightCoords1");
            GL.EnableVertexAttribArray(heightCoords1);
            GL.VertexAttribPointer(heightCoords1, 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 8 * sizeof(float));
            var heightCoords2 = _shader.GetAttribLocation("heightCoords2");
            GL.EnableVertexAttribArray(heightCoords2);
            GL.VertexAttribPointer(heightCoords2, 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 10 * sizeof(float));
            var heightCoords3 = _shader.GetAttribLocation("heightCoords3");
            GL.EnableVertexAttribArray(heightCoords3);
            GL.VertexAttribPointer(heightCoords3, 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 12 * sizeof(float));


            CreateCenterOfElement(_shader);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre)
        {
            _shader.Use();
            RegenerateTexture();

            float scale = 0.3f;
            Translation = new Vector3(-(TopLayerSize - 1) / (2.0f* divider * (height/(float)width)), 0, -(TopLayerSize - 1) / (2.0f * divider));

            //Translation = new Vector3(0, 0, -100);
            Matrix4 model = ModelMatrix.CreateModelMatrix(new Vector3(width/(float)height, 1, 1), RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
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
            if (counter % 100 == 0)
            {
                //DrillHole(new Vector3(300, 0, 1000), 50);

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
        }

        public void DrillAll(List<Vector3> loadedPositions)
        {
            List<Vector3> processedPoints = new List<Vector3>();
            foreach(var p in loadedPositions)
            {
                Vector3 point = new Vector3(p.X * 10 + 1000, p.Y, p.Z * 10 + 2000);
                processedPoints.Add(point);
            }

            Vector3 prev = processedPoints.First();
            for(int i=1;i<processedPoints.Count;i++)
            {
                Vector3 current = processedPoints[i];
                Brezenham((int)prev.X, (int)prev.Z, (int)current.X, (int)current.Z, current.Y);
                DrillHole(current, 50);

                prev = current;
            }

            Brezenham(50, 50, 500, 500, 0);
        }

        public void Brezenham(int x, int y, int x2, int y2, float height)
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
                    if (topLayer[x, y] > height)
                    {
                        topLayer[x, y] = height;
                    }
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
        }

        public void DrillHole(Vector3 point, float R)
        {
            float x = point.X;
            float y = point.Y;
            float z = point.Z;
            for(int i=0;i<width;i++)
            {
                for(int j=0;j<height;j++)
                {
                    if ((x - i)* (x - i) + (z - j) * (z - j) <= R * R)
                    {
                        if (topLayer[i, j] > point.Y)
                        {
                            topLayer[i, j] = point.Y;
                        }
                    }
                }
            }
        }

        private void GenerateTopLevel()
        {
            TopLayerPoints = new float[6 * 14 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            var TopLayerPointsHelper = new float[6 * 14 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            TopLayerIndices = new uint[6 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            int idx = 0;
            int indiceidx = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    topLayer[i, j] = 50;
                }
            }



            for (int i = 0; i < TopLayerSize; i++)
            {
                for (int j = 0; j < TopLayerSize; j++)
                {
                    if (i < TopLayerSize - 1 && j < TopLayerSize - 1)
                    {
                        Vector3 N1 = new Vector3(0, 0, 0);// NormalsGenerator.GenerateNormals(new Vector3(i, topLayer[i, j], j), new Vector3(i, topLayer[i, j + 1], j + 1), new Vector3(i + 1, topLayer[i + 1, j], j));

                        TopLayerPointsHelper[idx] = i/ divider; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j / divider; idx++; //z
                        TopLayerPointsHelper[idx] = N1.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N1.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N1.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height1x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height1y
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height2x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height2y
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height3x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height3y

                        TopLayerPointsHelper[idx] = i / divider; idx++; //x
                         TopLayerPointsHelper[idx] = topLayer[i, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = (j + 1) / divider; idx++; //z
                        TopLayerPointsHelper[idx] = N1.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N1.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N1.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //texCoordy
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height1x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height1y
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height2x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height2y
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height3x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height3y

                        TopLayerPointsHelper[idx] = (i + 1) / divider; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j / divider; idx++; //z
                        TopLayerPointsHelper[idx] = N1.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N1.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N1.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height1x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height1y
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height2x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height2y
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height3x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height3y


                        Vector3 N2 = new Vector3(0, 0, 0);// NormalsGenerator.GenerateNormals(new Vector3(i+1, topLayer[i+1, j], j), new Vector3(i, topLayer[i, j + 1], j + 1), new Vector3(i + 1, topLayer[i + 1, j+1], j+1));

                        TopLayerPointsHelper[idx] = (i + 1) / divider; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j / divider; idx++; //z
                        TopLayerPointsHelper[idx] = N2.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N2.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N2.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height1x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height1y
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height2x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height2y
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height3x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height3y

                        TopLayerPointsHelper[idx] = i / divider; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = (j + 1) / divider; idx++; //z
                        TopLayerPointsHelper[idx] = N2.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N2.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N2.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //texCoordy
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height1x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height1y
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height2x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height2y
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height3x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height3y

                        TopLayerPointsHelper[idx] = (i + 1) / divider; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = (j + 1) / divider; idx++; //z
                        TopLayerPointsHelper[idx] = N2.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N2.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N2.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //texCoordy
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height1x
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //height1y
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //height2x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height2y
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //height3x
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //height3y

                        TopLayerIndices[indiceidx] = (uint)(indiceidx); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(indiceidx); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(indiceidx); indiceidx++;

                        TopLayerIndices[indiceidx] = (uint)(indiceidx); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(indiceidx); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(indiceidx); indiceidx++;
                    }


                    //TopLayerPointsHelper[idx] = i; idx++; //x
                    //TopLayerPointsHelper[idx] = topLayer[i, j]; idx++; //y
                    //TopLayerPointsHelper[idx] = j; idx++; //z

                    //TopLayerPointsHelper[idx] = 0; idx++; //norm x
                    //TopLayerPointsHelper[idx] = 1; idx++; //norm y
                    //TopLayerPointsHelper[idx] = 0; idx++; //norm z

                    //TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                    //TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy

                    //if (i < TopLayerSize - 1 && j < TopLayerSize - 1)
                    //{
                    //    TopLayerIndices[indiceidx] = (uint)(i * TopLayerSize + j); indiceidx++;
                    //    TopLayerIndices[indiceidx] = (uint)(i * TopLayerSize + j + 1); indiceidx++;
                    //    TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerSize + j); indiceidx++;

                    //    TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerSize + j); indiceidx++;
                    //    TopLayerIndices[indiceidx] = (uint)(i * TopLayerSize + j + 1); indiceidx++;
                    //    TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerSize + j + 1); indiceidx++;
                    //}
                }
            }

            TopLayerPoints = TopLayerPointsHelper;
        }
    }
}
