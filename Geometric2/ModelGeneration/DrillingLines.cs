using System;
using System.Collections.Generic;
using System.Drawing;
using Geometric2.Functions;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public class DrillingLines : Element
    {
        public bool DrawPolyline { get; set; }
        public List<Vector3> drillPoints { get; set; }
        public int drillLinesVBO, drillLinesVAO, drillLinesEBO;
        private float[] polylinePoints;
        uint[] polylineIndices;

        Camera _camera;
        int width, height;

        public DrillingLines(List<Vector3> drillPoints)
        {
            this.drillPoints = drillPoints;
        }

        public override void CreateGlElement(Shader _shader)
        {
            CreateLines();
            drillLinesVAO = GL.GenVertexArray();
            drillLinesVBO = GL.GenBuffer();
            drillLinesEBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(drillLinesVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, drillLinesVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, drillLinesEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre)
        {
            //TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);

            if (DrawPolyline)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(drillLinesVAO);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Red));
                GL.DrawElements(PrimitiveType.Lines, 2 * polylinePoints.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            _shader.Use();
        }


        public void CreateLines()
        {
            polylinePoints = new float[3 * drillPoints.Count];
            polylineIndices = new uint[2 * (drillPoints.Count - 1)];
            int indiceIdx = 0;
            for (int i=0;i< drillPoints.Count; i++)
            {
                polylinePoints[3 * i] = drillPoints[i].X;
                polylinePoints[3 * i + 1] = drillPoints[i].Y;
                polylinePoints[3 * i + 2] = drillPoints[i].Z;
                if (i < drillPoints.Count - 1)
                {
                    polylineIndices[indiceIdx] = (uint)i;
                    indiceIdx++;
                    polylineIndices[indiceIdx] = (uint)i + 1;
                    indiceIdx++;
                }
            }
        }
    }
}
