using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Geometric2.Functions;
using Geometric2.Global;
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
        private int TopLayerX, TopLayerY;
        private float[] TopLayerPoints;
        uint[] TopLayerIndices;

        public int MillModelBottomLayerVBO, MillModelBottomLayerVAO, MillModelBottomLayerEBO;
        private float[] BottomLayerPoints;
        uint[] BottomLayerIndices;

        public int MillModelRoundLayerVBO, MillModelRoundLayerVAO, MillModelRoundLayerEBO;
        private float[] RoundLayerPoints;
        uint[] RoundLayerIndices;
        float[] layer;
        bool working = false;
        bool drilling = true;

        public int[] simulationTick = new int[] { 10 };
        public int[] percentCompleted = new int[] { 0 };
        public int[] nonCuttingPart = new int[] { 0 };
        public int[] showDriller = new int[] { 0 };
        public int[] stopButton = new int[] { 0 };

        Texture texture;
        Texture specular;
        float divider = 100.0f;
        Texture heightmap;
        int width, height;
        float altitude;
        Thread thread = null;
        Torus Torus;
        bool lastTime = false;

        public MillModel(int width, int height, float altitude, int TopLayerX, int TopLayerY, int[] simulationTick, int[] percentCompleted, int[] nonCuttingPart, int[] stopButton)
        {
            Torus = new Torus(0, 0, 0);
            this.simulationTick = simulationTick;
            this.percentCompleted = percentCompleted;
            this.nonCuttingPart = nonCuttingPart;
            this.stopButton = stopButton;
            topLayer = new float[TopLayerX, TopLayerY];
            layer = new float[topLayer.Length];
            CenterPosition = new Vector3(0, 0, 0);
            this.width = width;
            this.height = height;
            this.altitude = altitude;
            this.TopLayerX = TopLayerX;
            this.TopLayerY = TopLayerY;
        }

        public override void CreateGlElement(Shader _shader, Shader _millshader)
        {
            Torus.CreateGlElement(_shader, _millshader);
            texture = new Texture("./../../../Resources/wood.jpg");
            specular = new Texture("./../../../Resources/50specular.png");
            RegenerateTexture();
            GenerateTopLevel();
            GenerateBottomLayerLevel();
            GenerateRoundLevel();
            MillModelTopLayerVAO = GL.GenVertexArray();
            MillModelTopLayerVBO = GL.GenBuffer();
            MillModelTopLayerEBO = GL.GenBuffer();

            GL.BindVertexArray(MillModelTopLayerVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MillModelTopLayerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TopLayerPoints.Length * sizeof(float), TopLayerPoints, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MillModelTopLayerEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, TopLayerIndices.Length * sizeof(uint), TopLayerIndices, BufferUsageHint.StaticDraw);
            var a_Position_Location = _millshader.GetAttribLocation("a_Position");
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
            var aNormal = _millshader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(aNormal);
            GL.VertexAttribPointer(aNormal, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            var aTexCoords = _millshader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(aTexCoords);
            GL.VertexAttribPointer(aTexCoords, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            CreateCenterOfElement(_millshader);


            MillModelBottomLayerVAO = GL.GenVertexArray();
            MillModelBottomLayerVBO = GL.GenBuffer();
            MillModelBottomLayerEBO = GL.GenBuffer();

            GL.BindVertexArray(MillModelBottomLayerVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MillModelBottomLayerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, BottomLayerPoints.Length * sizeof(float), BottomLayerPoints, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MillModelBottomLayerEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, BottomLayerIndices.Length * sizeof(uint), BottomLayerIndices, BufferUsageHint.StaticDraw);
            a_Position_Location = _millshader.GetAttribLocation("a_Position");
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
            aNormal = _millshader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(aNormal);
            GL.VertexAttribPointer(aNormal, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            aTexCoords = _millshader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(aTexCoords);
            GL.VertexAttribPointer(aTexCoords, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            CreateCenterOfElement(_millshader);

            MillModelRoundLayerVAO = GL.GenVertexArray();
            MillModelRoundLayerVBO = GL.GenBuffer();
            MillModelRoundLayerEBO = GL.GenBuffer();

            GL.BindVertexArray(MillModelRoundLayerVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MillModelRoundLayerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, RoundLayerPoints.Length * sizeof(float), RoundLayerPoints, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MillModelRoundLayerEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, RoundLayerIndices.Length * sizeof(uint), RoundLayerIndices, BufferUsageHint.StaticDraw);
            a_Position_Location = _millshader.GetAttribLocation("a_Position");
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
            aNormal = _millshader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(aNormal);
            GL.VertexAttribPointer(aNormal, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            aTexCoords = _millshader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(aTexCoords);
            GL.VertexAttribPointer(aTexCoords, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            CreateCenterOfElement(_millshader);
        }

        public override void RenderGlElement(Shader _shader, Shader _millshader, Vector3 rotationCentre)
        {
            Torus.RenderGlElement(_shader, _millshader, rotationCentre);
            if (showDriller[0] == 1)
            {
                Torus.IsSelected = true;
            }
            else
            {
                Torus.IsSelected = false;
            }

            _millshader.Use();
            RegenerateTexture();
            Translation = new Vector3(-width / (2 * 100.0f), 0, -height / (2 * 100.0f));
            Matrix4 model = ModelMatrix.CreateModelMatrix(new Vector3(width / (float)TopLayerX, 1, height / (float)TopLayerY), RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            _millshader.SetMatrix4("model", model);
            GL.BindVertexArray(MillModelTopLayerVAO);
            texture.Use();
            specular.Use(TextureUnit.Texture1);
            heightmap.Use(TextureUnit.Texture2);
            GL.DrawElements(PrimitiveType.Triangles, 3 * TopLayerPoints.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            //GL.BindVertexArray(MillModelBottomLayerVAO);
            //texture.Use();
            //specular.Use(TextureUnit.Texture1);
            //heightmap.Use(TextureUnit.Texture2);
            //GL.DrawElements(PrimitiveType.Triangles, 3 * BottomLayerPoints.Length, DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(0);

            GL.BindVertexArray(MillModelRoundLayerVAO);
            texture.Use();
            specular.Use(TextureUnit.Texture1);
            heightmap.Use(TextureUnit.Texture2);
            GL.DrawElements(PrimitiveType.Triangles, 3 * RoundLayerPoints.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            RenderCenterOfElement(_millshader);
        }

        private async void RegenerateTexture()
        {
            if (!working &&(drilling || lastTime))
            {
                working = true;
                await new TaskFactory().StartNew(() =>
                {
                    Parallel.For(0, TopLayerX, i =>
                    {
                        for (int j = 0; j < TopLayerY; j++)
                        {
                            var x = topLayer[i, j];
                            layer[j * TopLayerX + i] = x;
                        }
                    });
                });
                if (heightmap != null)
                {
                    heightmap.DeleteTexture();
                }
                heightmap = new Texture(TopLayerX, TopLayerY, layer);
                working = false;
            }
        }

        public void DrillAll(List<Vector3> loadedPositions, CutterType cutterType, DrillType drillType, float radius, int drillHeight, float minimumH)
        {
            nonCuttingPart[0] = -1;

            if (thread != null)
            {
                thread.Abort();
            }

            thread = new Thread(() =>
            {
                drilling = true;
                List<Vector3> processedPoints = new List<Vector3>();
                foreach (var p in loadedPositions)
                {
                    Vector3 point = new Vector3(p.X * 100 / (width / (float)TopLayerX) + TopLayerX / 2.0f, p.Y, p.Z * 100 / (height / (float)TopLayerY) + TopLayerY / 2.0f);
                    processedPoints.Add(point);
                }

                if (drillType == DrillType.Parallel)
                {
                    percentCompleted[0] = 50;
                    Parallel.For(1, processedPoints.Count, i =>
                    {
                        Vector3 prev = processedPoints[i - 1];
                        Vector3 current = processedPoints[i];
                        Brezenham((int)prev.X, (int)prev.Z, (int)current.X, (int)current.Z, current.Y, prev.Y, current.Y, cutterType, drillType, radius, drillHeight, minimumH);
                    });
                }
                else
                {
                    for (int i = 1; i < processedPoints.Count; i++)
                    {
                        percentCompleted[0] = (int)(i * 100 / processedPoints.Count);
                        Vector3 prev = processedPoints[i - 1];
                        Vector3 current = processedPoints[i];
                        Brezenham((int)prev.X, (int)prev.Z, (int)current.X, (int)current.Z, current.Y, prev.Y, current.Y, cutterType, drillType, radius, drillHeight, minimumH);
                    }
                }

                percentCompleted[0] = 100;
                drilling = false;
                lastTime = true;
            });
            thread.Start();
        }

        public void Brezenham(int x, int y, int x2, int y2, float height, float z_From, float z_To, CutterType cutterType, DrillType drillType, float radius, int drillHeight, float minimumH)
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
                float z_Diff = z_To - z_From;
                float z_DiffPart = z_Diff / longest;
                for (int i = 0; i <= longest; i++)
                {
                    DrillHole(new Vector3(x, z_From + i * z_DiffPart, y), radius, cutterType, drillHeight, drillType, false, minimumH);
                    if (drillType == DrillType.Normal)
                    {
                        Thread.Sleep(simulationTick[0]);
                    }

                    if(stopButton[0] == 1)
                    {
                        nonCuttingPart[0] = 3;
                        stopButton[0] = 0;
                        thread.Abort();
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
            else
            {
                if (drillType == DrillType.Normal)
                {
                    if (z_From <= z_To)
                    {
                        for (float i = z_From; i < z_To; i += 0.01f)
                        {
                            DrillHole(new Vector3(x, i, y), radius, cutterType, drillHeight, drillType, true, minimumH);
                        }
                    }
                    else
                    {
                        for (float i = z_From; i >= z_To; i -= 0.01f)
                        {
                            DrillHole(new Vector3(x, i, y), radius, cutterType, drillHeight, drillType, true, minimumH);
                        }
                    }

                    Thread.Sleep(simulationTick[0]);
                }
                else
                {
                    DrillHole(new Vector3(x, height, y), radius, cutterType, drillHeight, drillType, true, minimumH);
                }
            }
        }

        public void DrillHole(Vector3 point, float r, CutterType cutterType, int drillHeight, DrillType drillType, bool goingDown, float minimumH)
        {
            if(drillType != DrillType.Parallel&& point.Y < minimumH)
            {
                nonCuttingPart[0] = 4;
                //thread.Abort();
            }

            float x = point.X;
            float y = point.Y;
            float z = point.Z;
            float f_radous = r / 2;

            float rX = f_radous / (width / (float)TopLayerX);
            float rY = f_radous / (height / (float)TopLayerY);

            float NPX = (point.X - TopLayerX / 2.0f) * (width / (float)TopLayerX);
            float NPY = (point.Z - TopLayerY / 2.0f) * (height / (float)TopLayerY);
            Torus.CenterPosition = new Vector3(NPX / 100, y, NPY / 100);

            for (float _x = -rX; _x <= rX; _x += 1)
            {
                for (float _y = -rY; _y <= rY; _y += 1)
                {
                    if (_x + x >= 0 && _y + z >= 0 && _x + x < TopLayerX && _y + z < TopLayerY)
                    {
                        float OldX = (_x * (f_radous / rX));
                        float OldY = (_y * (f_radous / rY));

                        if (OldX * OldX + OldY * OldY <= f_radous * f_radous)
                        {
                            float yb = y;
                            if (cutterType == CutterType.Spherical)
                            {
                                float xa = x;
                                float ya = y * 100 + f_radous;
                                float za = z;

                                float xb = x + OldX;
                                float zb = z + OldY;

                                float val = f_radous * f_radous - (xb - xa) * (xb - xa) - (zb - za) * (zb - za);
                                yb = -(float)Math.Sqrt(val) + ya;
                                yb /= 100;
                            }

                            if (drillType != DrillType.Parallel && topLayer[(int)(_x + x), (int)(_y + z)] > y + drillHeight / 100.0f)
                            {
                                nonCuttingPart[0] = 1;
                                //thread.Abort();
                            }

                            if (drillType != DrillType.Parallel && goingDown && cutterType == CutterType.Flat && topLayer[(int)(_x + x), (int)(_y + z)] > yb)
                            {
                                nonCuttingPart[0] = 2;
                                //thread.Abort();
                            }

                            if (topLayer[(int)(_x + x), (int)(_y + z)] > yb)
                            {
                                topLayer[(int)(_x + x), (int)(_y + z)] = yb;
                            }
                        }
                    }
                }
            }
        }

        private void GenerateTopLevel()
        {
            TopLayerPoints = new float[2 * 8 * TopLayerX * TopLayerY];
            var TopLayerPointsHelper = new float[TopLayerPoints.Length];
            TopLayerIndices = new uint[2 * 6 * (TopLayerX - 1) * (TopLayerY - 1)];
            int idx = 0;
            int indiceidx = 0;

            for (int i = 0; i < TopLayerX; i++)
            {
                for (int j = 0; j < TopLayerY; j++)
                {
                    topLayer[i, j] = altitude;
                }
            }

            //For Top
            for (int i = 0; i < TopLayerX; i++)
            {
                for (int j = 0; j < TopLayerY; j++)
                {
                    TopLayerPointsHelper[idx] = i / divider; idx++; //x
                    TopLayerPointsHelper[idx] = 0; idx++; //y
                    TopLayerPointsHelper[idx] = j / divider; idx++; //z
                    TopLayerPointsHelper[idx] = 0; idx++; //norm x
                    TopLayerPointsHelper[idx] = 1; idx++; //norm y
                    TopLayerPointsHelper[idx] = 0; idx++; //norm z
                    TopLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                    TopLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
                    if (i < TopLayerX - 1 && j < TopLayerY - 1)
                    {
                        TopLayerIndices[indiceidx] = (uint)(i * TopLayerY + j); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(i * TopLayerY + j + 1); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerY + j); indiceidx++;

                        TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerY + j); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)(i * TopLayerY + j + 1); indiceidx++;
                        TopLayerIndices[indiceidx] = (uint)((i + 1) * TopLayerY + j + 1); indiceidx++;
                    }
                }
            }

            TopLayerPoints = TopLayerPointsHelper;
        }

        //For bottom
        private void GenerateBottomLayerLevel()
        {
            BottomLayerPoints = new float[8 * 4];
            var BottomLayerPointsHelper = new float[BottomLayerPoints.Length];
            BottomLayerIndices = new uint[6];
            int idx = 0;
            int indiceidx = 0;

            for (int i = 0; i < TopLayerX; i += (TopLayerX - 1))
            {
                for (int j = 0; j < TopLayerY; j += (TopLayerY - 1))
                {
                    BottomLayerPointsHelper[idx] = i / divider; idx++; //x
                    BottomLayerPointsHelper[idx] = 0; idx++; //y
                    BottomLayerPointsHelper[idx] = j / divider; idx++; //z
                    BottomLayerPointsHelper[idx] = 0; idx++; //norm x
                    BottomLayerPointsHelper[idx] = -1; idx++; //norm y
                    BottomLayerPointsHelper[idx] = 0; idx++; //norm z
                    BottomLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                    BottomLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
                }
            }

            BottomLayerIndices[indiceidx] = (uint)(0); indiceidx++;
            BottomLayerIndices[indiceidx] = (uint)(1); indiceidx++;
            BottomLayerIndices[indiceidx] = (uint)(2); indiceidx++;
            BottomLayerIndices[indiceidx] = (uint)(2); indiceidx++;
            BottomLayerIndices[indiceidx] = (uint)(1); indiceidx++;
            BottomLayerIndices[indiceidx] = (uint)(3); indiceidx++;

            BottomLayerPoints = BottomLayerPointsHelper;
        }

        //For round
        private void GenerateRoundLevel()
        {
            RoundLayerPoints = new float[8 * (4 * TopLayerX + 4 * TopLayerY)];
            var RoundLayerPointsHelper = new float[RoundLayerPoints.Length];
            RoundLayerIndices = new uint[4 * (3 * (TopLayerX - 1) + 3 * (TopLayerY - 1))];
            int idx = 0;
            int indiceidx = 0;

            // -x
            int i = 0;
            int j = 0;
            for (j = 0; j < TopLayerY; j++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = -200; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = -1; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = 0; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            for (j = 0; j < TopLayerY; j++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = 0; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = -1; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = 0; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            for (j = 0; j < TopLayerY; j++)
            {
                if (j < TopLayerY - 1)
                {
                    RoundLayerIndices[indiceidx] = (uint)(j); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerY + j); indiceidx++;

                    RoundLayerIndices[indiceidx] = (uint)(TopLayerY + j); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerY + j + 1); indiceidx++;
                }
            }

            // x
            i = TopLayerX - 1;
            j = 0;
            for (j = 0; j < TopLayerY; j++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = -200; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = 1; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = 0; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            for (j = 0; j < TopLayerY; j++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = 0; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = 1; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = 0; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            int amount = 2 * TopLayerY;
            for (j = 0; j < TopLayerY; j++)
            {
                if (j < TopLayerY - 1)
                {
                    RoundLayerIndices[indiceidx] = (uint)(j + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1 + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerY + j + amount); indiceidx++;

                    RoundLayerIndices[indiceidx] = (uint)(TopLayerY + j + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1 + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerY + j + 1 + amount); indiceidx++;
                }
            }

            // -y
            i = 0;
            j = 0;
            for (i = 0; i < TopLayerX; i++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = -200; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = 0; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = -1; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            for (i = 0; i < TopLayerX; i++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = 0; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = 0; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = -1; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            amount = 4 * TopLayerY;
            for (j = 0; j < TopLayerX; j++)
            {
                if (j < TopLayerX - 1)
                {
                    RoundLayerIndices[indiceidx] = (uint)(j + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1 + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerX + j + amount); indiceidx++;

                    RoundLayerIndices[indiceidx] = (uint)(TopLayerX + j + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1 + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerX + j + 1 + amount); indiceidx++;
                }
            }

            // y
            i = 0;
            j = TopLayerY - 1;
            for (i = 0; i < TopLayerX; i++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = -200; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = 0; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = 1; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            for (i = 0; i < TopLayerX; i++)
            {
                RoundLayerPointsHelper[idx] = i / divider; idx++; //x
                RoundLayerPointsHelper[idx] = 0; idx++; //y
                RoundLayerPointsHelper[idx] = j / divider; idx++; //z
                RoundLayerPointsHelper[idx] = 0; idx++; //norm x
                RoundLayerPointsHelper[idx] = 0; idx++; //norm y
                RoundLayerPointsHelper[idx] = 1; idx++; //norm z
                RoundLayerPointsHelper[idx] = (float)i / (TopLayerX - 1); idx++; //texCoordx
                RoundLayerPointsHelper[idx] = (float)j / (TopLayerY - 1); idx++; //texCoordy
            }

            amount = 4 * TopLayerY + 2 * TopLayerX;
            for (j = 0; j < TopLayerX; j++)
            {
                if (j < TopLayerX - 1)
                {
                    RoundLayerIndices[indiceidx] = (uint)(j + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1 + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerX + j + amount); indiceidx++;

                    RoundLayerIndices[indiceidx] = (uint)(TopLayerX + j + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(j + 1 + amount); indiceidx++;
                    RoundLayerIndices[indiceidx] = (uint)(TopLayerX + j + 1 + amount); indiceidx++;
                }
            }

            RoundLayerPoints = RoundLayerPointsHelper;
        }
    }
}
