using System;
using System.Drawing;
using System.Drawing.Imaging;
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
        private int TopLayerSize = 200;
        private int TextureDensity = 100;
        private float[] TopLayerPoints;
        uint[] TopLayerIndices;
        Random random = new Random(5000);
        Texture texture;
        Texture specular;

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
            Translation = new Vector3(-(TopLayerSize-1) / 2.0f, 0, -(TopLayerSize - 1) / 2.0f);
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
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

        private void GenerateTopLevel()
        {
            TopLayerPoints = new float[6 * 14 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            var TopLayerPointsHelper = new float[6 * 14 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            TopLayerIndices = new uint[6 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            int idx = 0;
            int indiceidx = 0;

            //for (int i = 0; i < width; i++)
            //{
            //    for (int j = 0; j < height; j++)
            //    {
            //        topLayer[i, j] = random.Next(5, 6);
            //    }
            //}

            for (int i = 0; i < TopLayerSize; i++)
            {
                for (int j = 0; j < TopLayerSize; j++)
                {
                    if (i < TopLayerSize - 1 && j < TopLayerSize - 1)
                    {
                        Vector3 N1 = NormalsGenerator.GenerateNormals(new Vector3(i, topLayer[i, j], j), new Vector3(i, topLayer[i, j + 1], j + 1), new Vector3(i + 1, topLayer[i + 1, j], j));

                        TopLayerPointsHelper[idx] = i; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j; idx++; //z
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

                        TopLayerPointsHelper[idx] = i; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = j + 1; idx++; //z
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

                        TopLayerPointsHelper[idx] = i + 1; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j; idx++; //z
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


                        Vector3 N2 = NormalsGenerator.GenerateNormals(new Vector3(i+1, topLayer[i+1, j], j), new Vector3(i, topLayer[i, j + 1], j + 1), new Vector3(i + 1, topLayer[i + 1, j+1], j+1));

                        TopLayerPointsHelper[idx] = i + 1; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j; idx++; //z
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

                        TopLayerPointsHelper[idx] = i; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = j + 1; idx++; //z
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

                        TopLayerPointsHelper[idx] = i + 1; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = j + 1; idx++; //z
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
