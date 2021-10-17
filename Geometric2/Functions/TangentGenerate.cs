using OpenTK;

namespace Geometric2.Functions
{
    public class Triangle
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2 texCoord;
        public Vector3 tangent;
        public Vector3 bitangent;
    }

    public static class TangentGenerate
    {
        public static float[] GenerateModelWithTangent(this float[] model)
        {
            float[] modelTangent = new float[(model.Length / 8) * 14];
            int nrTriangle = 0;
            for (int i = 0; i < model.Length; i += 3 * 8)
            {

                Vector3 v0 = new Vector3(model[i], model[i + 1], model[i + 2]);
                Vector3 v1 = new Vector3(model[8 + i], model[8 + i + 1], model[8 + i + 2]);
                Vector3 v2 = new Vector3(model[16 + i], model[16 + i + 1], model[16 + i + 2]);

                Vector2 uv0 = new Vector2(model[i + 6], model[i + 7]);
                Vector2 uv1 = new Vector2(model[8 + i + 6], model[8 + i + 7]);
                Vector2 uv2 = new Vector2(model[16 + i + 6], model[16 + i + 7]);

                Vector3 deltaPos1 = v1 - v0;
                Vector3 deltaPos2 = v2 - v0;

                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                //float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                //Vector3 tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                //Vector3 bitangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;

                Vector3 tangent;
                Vector3 bitangent;
                float f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

                tangent.X = f * (deltaUV2.Y * deltaPos1.X - deltaUV1.Y * deltaPos2.X);
                tangent.Y = f * (deltaUV2.Y * deltaPos1.Y - deltaUV1.Y * deltaPos2.Y);
                tangent.Z = f * (deltaUV2.Y * deltaPos1.Z - deltaUV1.Y * deltaPos2.Z);
                tangent = Vector3.Normalize(tangent);

                bitangent.X = f * (-deltaUV2.X * deltaPos1.X + deltaUV1.X * deltaPos2.X);
                bitangent.Y = f * (-deltaUV2.X * deltaPos1.Y + deltaUV1.X * deltaPos2.Y);
                bitangent.Z = f * (-deltaUV2.X * deltaPos1.Z + deltaUV1.X * deltaPos2.Z);
                bitangent = Vector3.Normalize(bitangent);

                for (int j = 0; j < 14; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (j < 8)
                        {
                            modelTangent[nrTriangle * 3 * 14 + k * 14 + j] = model[nrTriangle * 3 * 8 + k * 8 + j];
                        }
                        else
                        {
                            modelTangent[nrTriangle * 3 * 14 + k * 14 + 8] = tangent.X;
                            modelTangent[nrTriangle * 3 * 14 + k * 14 + 9] = tangent.Y;
                            modelTangent[nrTriangle * 3 * 14 + k * 14 + 10] = tangent.Z;

                            modelTangent[nrTriangle * 3 * 14 + k * 14 + 11] = bitangent.X;
                            modelTangent[nrTriangle * 3 * 14 + k * 14 + 12] = bitangent.Y;
                            modelTangent[nrTriangle * 3 * 14 + k * 14 + 13] = bitangent.Z;
                        }
                    }
                }

                nrTriangle++;
            }


            return modelTangent;
        }


    }
}
