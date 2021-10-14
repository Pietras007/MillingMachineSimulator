using Geometric2.MatrixHelpers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.RasterizationClasses
{
    public class Camera
    {
        public Vector3 CameraCenterPoint { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float CameraDist { get; set; }
        public float EyeSeparation { get; set; }
        public float FocalLength { get; set; }

        private float _fov = MathHelper.PiOver6;
        private float AspectRatio;


        public Camera(Vector3 position, float aspectRatio)
        {
            AspectRatio = aspectRatio;
            CameraDist = 10f;
            CameraCenterPoint = new Vector3(0, 0, 0);
            RotationX = -(float)Math.PI / 4;
            RotationY = (float)Math.PI / 4; 
            EyeSeparation = 0.1f;
            FocalLength = 10f;
        }

        public Vector3 GetCameraPosition()
        {
            return new Vector3(new Vector4(0, 0, 0, 1) * TranslationMatrix.CreateTranslationMatrix(new Vector3(0, 0, CameraDist)) * (RotationMatrix.CreateRotationMatrix_X(-RotationX) * RotationMatrix.CreateRotationMatrix_Y(-RotationY) * TranslationMatrix.CreateTranslationMatrix(CameraCenterPoint)));
        }

        public Matrix4 GetViewMatrix(float eyeDist = 0)
        {
            return ViewMatrix.CreateViewMatrix(this, eyeDist);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return ProjectionMatrix.CreateProjectionMatrix(_fov, AspectRatio, 0.01f, 100f);
        }

        public Matrix4 GetAnaglyphProjectionMatrixRed()
        {
            float _near = 0.01f;
            float _far = 1000f;
            float eyeOff = -1 * (EyeSeparation * 0.5f) * (_near / FocalLength);
            float top = _near * (float)Math.Tan((_fov) * 0.5f);
            float right = AspectRatio * top;

            return ProjectionMatrix.CreateAnaglyphProjectionMatrix(- right - eyeOff, right - eyeOff, -top, top, _near, _far);
        }

        public Matrix4 GetAnaglyphProjectionMatrixBlue()
        {

            float _near = 0.01f;
            float _far = 1000f;
            float eyeOff = 1 * (EyeSeparation * 0.5f) * (_near / FocalLength);
            float top = _near * (float)Math.Tan((_fov) * 0.5f);
            float right = AspectRatio * top;

            return ProjectionMatrix.CreateAnaglyphProjectionMatrix(-right - eyeOff, right - eyeOff, -top, top, _near, _far);
        }
    }
}
