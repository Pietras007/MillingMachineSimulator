using System;
using System.Drawing;
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
        private int TopLayerSize = 5;
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
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
            var texCoordLocation = _shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            CreateCenterOfElement(_shader);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre)
        {
            _shader.Use();
            //Render TopLayer
            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(MillModelTopLayerVAO);
            texture.Use();
            specular.Use(TextureUnit.Texture1);
            //texture.Use(TextureUnit.Texture2);
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
            TopLayerPoints = new float[5 * TopLayerSize * TopLayerSize];
            TopLayerIndices = new uint[6 * (TopLayerSize - 1) * (TopLayerSize - 1)];
            int idx = 0;
            int indiceidx = 0;
            for (int i = 0; i < TopLayerSize; i++)
            {
                for (int j = 0; j < TopLayerSize; j++)
                {
                    topLayer[i, j] = random.Next(1, 4);
                    TopLayerPoints[idx] = i; idx++; //x
                    TopLayerPoints[idx] = topLayer[i, j]; idx++; //y
                    TopLayerPoints[idx] = j; idx++; //z

                    TopLayerPoints[idx] = (float)i / (TopLayerSize - 1); idx++; //texCoordx
                    TopLayerPoints[idx] = (float)j / (TopLayerSize - 1); idx++; //texCoordy

                    if (i < TopLayerSize - 1 && j < TopLayerSize - 1)
                    {
                        TopLayerIndices[indiceidx] = (uint)(j * TopLayerSize + i); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(j * TopLayerSize + i + 1); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)((j + 1) * TopLayerSize + i); indiceidx++;

                        TopLayerIndices[indiceidx] = (uint)((j + 1) * TopLayerSize + i); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(j * TopLayerSize + i + 1); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)((j + 1) * TopLayerSize + i + 1); indiceidx++;
                    }
                }
            }
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
