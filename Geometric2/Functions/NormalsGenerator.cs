using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Functions
{
    public static class NormalsGenerator
    {
        public static Vector3 GenerateNormals(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 A = b - a;
            Vector3 B = c - a;
            float Nx = A.Y * B.Z - A.Z * B.Y;
            float Ny = A.Z * B.X - A.X * B.Z;
            float Nz = A.X * B.Y - A.Y * B.X;
            return new Vector3(Nx, Ny, Nz);
        }
    }
}
