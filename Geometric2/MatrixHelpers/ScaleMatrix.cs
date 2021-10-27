using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class ScaleMatrix
    {
        public static Matrix4 CreateScaleMatrix(float scale)
        {
            Matrix4 Scale = Matrix4.Identity * scale;
            Scale.M44 = 1;

            return Scale;
        }

        public static Matrix4 CreateScaleMatrix(Vector3 scale)
        {
            Matrix4 Scale = Matrix4.Identity;
            Scale.M11 = scale.X;
            Scale.M22 = scale.Y;
            Scale.M33 = scale.Z;
            Scale.M44 = 1;

            return Scale;
        }
    }
}
