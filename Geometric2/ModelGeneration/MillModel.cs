using System;
using System.Drawing;
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
        private int TopLayerSize = 10;
        private int TextureTimes = 1;
        private float[] TopLayerPoints;
        uint[] TopLayerIndices;
        Random random = new Random();
        Texture texture;
        Texture specular;


        public float torus_R = 2.2f;
        public float torus_r = 1.4f;
        public int torusMajorDividions = 6;
        public int torusMinorDividions = 6;
        public int torusNumber;

        public MillModel()
        {
            topLayer = new float[TopLayerSize, TopLayerSize];
            CenterPosition = new Vector3(0, 0, 0);
        }

        public override void CreateGlElement(Shader _shader)
        {
            texture = new Texture("./../../Resources/wood.jpg");
            specular = new Texture("./../../Resources/50specular.png");
            //FillTorusGeometry();
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
            var aTangent = _shader.GetAttribLocation("aTangent");
            GL.EnableVertexAttribArray(aTangent);
            GL.VertexAttribPointer(aTangent, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 8 * sizeof(float));
            var aBitangent = _shader.GetAttribLocation("aBitangent");
            GL.EnableVertexAttribArray(aBitangent);
            GL.VertexAttribPointer(aBitangent, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 11 * sizeof(float));

            CreateCenterOfElement(_shader);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre)
        {
            _shader.Use();
            //Render TopLayer
            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Translation = new Vector3(-(TopLayerSize-1) / 2.0f, 0, -(TopLayerSize - 1) / 2.0f);
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(MillModelTopLayerVAO);
            texture.Use();
            //specular.Use(TextureUnit.Texture1);
            GL.DrawElements(PrimitiveType.Triangles, 3 * TopLayerPoints.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            RenderCenterOfElement(_shader);




            //Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, (float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360), CenterPosition + Translation + TemporaryTranslation);
            //TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            //Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            //_shader.SetMatrix4("model", model);
            //GL.BindVertexArray(MillModelTopLayerVAO);
            //if (IsSelected)
            //{
            //    _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            //}
            //else
            //{
            //    _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            //}

            //GL.DrawElements(PrimitiveType.Lines, 2 * TopLayerPoints.Length, DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(0);

            //RenderCenterOfElement(_shader);
        }

        private void GenerateTopLevel()
        {
            TopLayerPoints = new float[6 * 14 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            var TopLayerPointsHelper = new float[6 * 8 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            TopLayerIndices = new uint[6 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            int idx = 0;
            int indiceidx = 0;

            for (int i = 0; i < TopLayerSize; i++)
            {
                for (int j = 0; j < TopLayerSize; j++)
                {
                    topLayer[i, j] = random.Next(1, 5);
                }
            }

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

                        TopLayerPointsHelper[idx] = i; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = j + 1; idx++; //z
                        TopLayerPointsHelper[idx] = N1.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N1.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N1.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //texCoordy

                        TopLayerPointsHelper[idx] = i + 1; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j; idx++; //z
                        TopLayerPointsHelper[idx] = N1.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N1.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N1.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy


                        Vector3 N2 = NormalsGenerator.GenerateNormals(new Vector3(i+1, topLayer[i+1, j], j), new Vector3(i, topLayer[i, j + 1], j + 1), new Vector3(i + 1, topLayer[i + 1, j+1], j+1));

                        TopLayerPointsHelper[idx] = i + 1; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j]; idx++; //y
                        TopLayerPointsHelper[idx] = j; idx++; //z
                        TopLayerPointsHelper[idx] = N2.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N2.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N2.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy

                        TopLayerPointsHelper[idx] = i; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = j + 1; idx++; //z
                        TopLayerPointsHelper[idx] = N2.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N2.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N2.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //texCoordy

                        TopLayerPointsHelper[idx] = i + 1; idx++; //x
                        TopLayerPointsHelper[idx] = topLayer[i + 1, j + 1]; idx++; //y
                        TopLayerPointsHelper[idx] = j + 1; idx++; //z
                        TopLayerPointsHelper[idx] = N2.X; idx++; //norm x
                        TopLayerPointsHelper[idx] = N2.Y; idx++; //norm y
                        TopLayerPointsHelper[idx] = N2.Z; idx++; //norm z
                        TopLayerPointsHelper[idx] = (float)(i + 1) / (TopLayerSize - 1); idx++; //texCoordx
                        TopLayerPointsHelper[idx] = (float)(j + 1) / (TopLayerSize - 1); idx++; //texCoordy

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

            TopLayerPoints = TangentGenerate.GenerateModelWithTangent(TopLayerPointsHelper);
        }

        public void RegenerateTorusVertices()
        {
            FillTorusGeometry();
            GL.BindVertexArray(MillModelTopLayerVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MillModelTopLayerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TopLayerPoints.Length * sizeof(float), TopLayerPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MillModelTopLayerEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, TopLayerIndices.Length * sizeof(uint), TopLayerIndices, BufferUsageHint.DynamicDraw);
        }

        private (float[], uint[]) GenerateTorus(double majorRadius, double minorRadius, int majorSegments, int minorSegments)
        {
            float[] resultVertices = new float[3 * majorSegments * minorSegments];
            uint[] indices = new uint[4 * majorSegments * minorSegments];
            for (int maj = 0; maj < majorSegments; maj++)
            {
                double majorAngle = 2 * Math.PI * maj / majorSegments;
                for (int min = 0; min < minorSegments; min++)
                {
                    double minorAngle = 2 * Math.PI * min / minorSegments;

                    resultVertices[3 * maj * minorSegments + 3 * min] = (float)((majorRadius + minorRadius * Math.Cos(minorAngle)) * Math.Cos(majorAngle));
                    resultVertices[3 * maj * minorSegments + 3 * min + 1] = (float)(minorRadius * Math.Sin(minorAngle)); //(float)((majorRadius + minorRadius * Math.Cos(minorAngle)) * Math.Sin(majorAngle));
                    resultVertices[3 * maj * minorSegments + 3 * min + 2] = (float)((majorRadius + minorRadius * Math.Cos(minorAngle)) * Math.Sin(majorAngle)); //(float)(minorRadius * Math.Sin(minorAngle));

                    if (min != minorSegments - 1)
                    {
                        indices[4 * maj * minorSegments + 4 * min] = (uint)(maj * minorSegments + min);
                        indices[4 * maj * minorSegments + 4 * min + 1] = (uint)(maj * minorSegments + min + 1);
                        indices[4 * maj * minorSegments + 4 * min + 2] = (uint)(maj * minorSegments + min);
                        var _maj = maj != majorSegments - 1 ? maj + 1 : 0;
                        indices[4 * maj * minorSegments + 4 * min + 3] = (uint)(_maj * minorSegments + min);
                    }
                    else
                    {
                        indices[4 * maj * minorSegments + 4 * min] = (uint)(maj * minorSegments + min);
                        indices[4 * maj * minorSegments + 4 * min + 1] = (uint)(maj * minorSegments);
                        indices[4 * maj * minorSegments + 4 * min + 2] = (uint)(maj * minorSegments + min);
                        var _maj = maj != majorSegments - 1 ? maj + 1 : 0;
                        indices[4 * maj * minorSegments + 4 * min + 3] = (uint)(_maj * minorSegments + min);
                    }
                }
            }

            return (resultVertices, indices);
        }

        private void FillTorusGeometry()
        {
            var x = GenerateTorus(torus_R, torus_r, torusMajorDividions, torusMinorDividions);
            TopLayerPoints = x.Item1;
            TopLayerIndices = x.Item2;
        }
        public Vector3 Position()
        {
            return this.CenterPosition + this.TemporaryTranslation + this.Translation;
        }
    }
}
