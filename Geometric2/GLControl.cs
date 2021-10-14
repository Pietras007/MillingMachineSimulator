﻿using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL4;
using Geometric2.MatrixHelpers;
using Geometric2.ModelGeneration;
using System.Numerics;
using OpenTK;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using Geometric2.Global;
using Geometric2.RasterizationClasses;
using System.Collections.Generic;
using Geometric2.Helpers;

namespace Geometric2
{
    public partial class Form1 : Form
    {
        private void glControl1_Load(object sender, EventArgs e)
        {
            Elements.Add(xyzLines);
            Elements.Add(transformCenterLines);

            coursor.width = glControl1.Width;
            coursor.height = glControl1.Height;
            GL.ClearColor(Color.LightCyan);
            GL.Enable(EnableCap.DepthTest);
            _shader = new Shader("./../../Shaders/VertexShader.vert", "./../../Shaders/FragmentShader.frag");

            coursor.CreateCoursor(_shader);
            foreach (var el in Elements)
            {
                el.CreateGlElement(_shader);
            }

            _camera = new Camera(new Vector3(0, 5, 15), glControl1.Width / (float)glControl1.Height);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            Matrix4 viewMatrix = _camera.GetViewMatrix();
            Matrix4 projectionMatrix = _camera.GetProjectionMatrix();
            _shader.SetMatrix4("view", viewMatrix);
            _shader.SetMatrix4("projection", projectionMatrix);
            _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));


            RenderScene(viewMatrix, projectionMatrix);

            GL.Flush();
            glControl1.SwapBuffers();
        }

        private void RenderScene(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            _shader.Use();
            coursor.DrawCoursor(_shader, viewMatrix, projectionMatrix, _camera);
            foreach (var el in Elements)
            {
                el.RenderGlElement(_shader, coursor.CoursorGloalPosition);
            }
           
        }

        private void glControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (drawingStatus)
                {
                    case DrawingStatus.No:
                        break;

                    case DrawingStatus.Torus:
                        Torus torus = new Torus(coursor.CoursorGloalPosition, torusNumber);
                        torusNumber++;
                        torus.CreateGlElement(_shader);
                        Elements.Add(torus);
                        selectedTorus = torus;
                        break;
                }
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (coursor.CoursorMode != CoursorMode.Manual || coursor.CoursorMoving == true)
            {
                coursor.CoursorScreenPosition = (e.X, e.Y);
            }

            int xPosMouse, yPosMouse;
            if (e.Button == MouseButtons.Middle)
            {
                xPosMouse = e.X;
                yPosMouse = e.Y;
                if (prev_xPosMouse != -1 && prev_yPosMouse != -1)
                {
                    var deltaX = xPosMouse - prev_xPosMouse;
                    var deltaY = yPosMouse - prev_yPosMouse;

                    _camera.RotationX -= (float)(2 * Math.PI * deltaY / glControl1.Height);
                    _camera.RotationY += (float)(2 * Math.PI * deltaX / glControl1.Width);

                }

                prev_xPosMouse = xPosMouse;
                prev_yPosMouse = yPosMouse;
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                prev_xPosMouse = -1;
                prev_yPosMouse = -1;
            }
        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            if (_camera.CameraDist - numberOfTextLinesToMove > 1.0f)
            {
                _camera.CameraDist -= numberOfTextLinesToMove;
            }
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                coursor.CoursorMoving = true;
            }
        }

        private void glControl1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                coursor.CoursorMoving = false;
            }
        }
    }
}
