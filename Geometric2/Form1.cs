using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Geometric2.RasterizationClasses;
using OpenTK.Graphics.OpenGL4;
using Geometric2.MatrixHelpers;
using Geometric2.ModelGeneration;
using System.Numerics;
using OpenTK;
using Geometric2.Helpers;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using Geometric2.Global;
using System.Xml;
using System.Linq;
using Geometric2.Models;

namespace Geometric2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitSolution();
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    glControl1.Invalidate();
                    Thread.Sleep(16);
                }
            });

            thread.Start();
            coursor = new Coursor();
            millModel = new MillModel(dataModel.Width, dataModel.Height, dataModel.Altitude);
            coursor.CoursorMode = CoursorMode.Auto;
            transformCenterLines.selectedElements = SelectedElements;
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private List<Vector3> drillPositions;
        private Shader _shader;
        private Shader _millshader;
        private Camera _camera;
        private Coursor coursor;
        private MillModel millModel;
        public InitializeDataModel dataModel;


        private XyzLines xyzLines = new XyzLines();
        private DrillingLines drillingLines;
        private List<Element> Elements = new List<Element>();
        private List<Element> SelectedElements = new List<Element>();
        DrawingStatus drawingStatus = DrawingStatus.No;
        private TransformCenterLines transformCenterLines = new TransformCenterLines();

        int prev_xPosMouse = -1, prev_yPosMouse = -1;
        int torusNumber;

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.Title = "Select Paths File";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                List<string> file = new List<string>();
                drillPositions = new List<Vector3>();
                foreach (string line in System.IO.File.ReadLines(fileName))
                {
                    file.Add(line);
                }

                foreach (var line in file)
                {
                    string afterX = line.Split('X')[1];
                    string X = afterX.Split('Y')[0];
                    string afterY = afterX.Split('Y')[1];
                    string Y = afterY.Split('Z')[0];
                    string Z = afterY.Split('Z')[1];

                    float _x, _y, _z;
                    if (float.TryParse(X, out _x) && float.TryParse(Y, out _y) && float.TryParse(Z, out _z))
                    {
                        drillPositions.Add(new Vector3(_x, _z, -_y));
                    }
                }

                drillingLines = new DrillingLines(drillPositions);
                foreach(var el in Elements)
                {
                    if(el is DrillingLines)
                    {
                        Elements.Remove(el);
                    }
                }

                Elements.Add(drillingLines);
                drillingLines.CreateGlElement(_shader);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitSolution();
        }

        private void drillingLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (drillingLines != null)
            {
                drillingLines.DrawPolyline = drillingLineCheckBox.Checked;
            }

            if(drillingLineCheckBox.Checked)
            {
                millModel.DrillAll(drillingLines.drillPoints);
            }
        }

        

        private void InitSolution()
        {
            dataModel = new InitializeDataModel();
            InitializeData initializeData = new InitializeData(dataModel);
            initializeData.ShowDialog();
        }
    }
}
