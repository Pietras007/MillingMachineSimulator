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
            millModel = new MillModel(dataModel.Width, dataModel.Height, dataModel.Altitude, dataModel.Divisions_X, dataModel.Divisions_Y, simulationTick, percentCompleted, nonCuttingPart);
            coursor.CoursorMode = CoursorMode.Auto;
            transformCenterLines.selectedElements = SelectedElements;
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    this.Invoke((MethodInvoker)delegate { 
                        progressBar.Value = percentCompleted[0];
                        if (nonCuttingPart[0] != 0)
                        {
                            errorLabel.Text = "ERROR: MILLING WITH A NON-CUTTING PART";
                            nonCuttingPart[0] = 0;
                        }
                    });
                    if (percentCompleted[0] == 100)
                    {
                        percentCompleted[0] = 0;
                    }

                    Thread.Sleep(50);
                }
            });
            thread.Start();

            cameraLightCheckBox.Checked = true;
            radiousTextBox.Text = "5.0";
            drillHeightTextBox.Text = "5.0";
            normalRadioButton.Checked = true;
            drillButton.Enabled = false;
        }

        private List<Vector3> drillPositions;
        private Shader _shader;
        private Shader _millshader;
        private Camera _camera;
        private Coursor coursor;
        private MillModel millModel;
        public InitializeDataModel dataModel;
        private bool cameraLight = true;
        DrillType drillType = DrillType.Normal;
        CutterType cutterType = CutterType.Spherical;
        int radious = 50;
        int drillHeight = 50;
        int[] simulationTick = new int[] { 0 };
        int[] percentCompleted = new int[] { 0 };
        int[] nonCuttingPart = new int[] { 0 };


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
            string fileName = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
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
                        if(_z < 0)
                        {
                            MessageBox.Show("GIONG TO DRILL UNDER SURFACE");
                        }
                    }
                }

                drillingLines = new DrillingLines(drillPositions);
                Element elToRemove = null;
                foreach(var el in Elements)
                {
                    if(el is DrillingLines)
                    {
                        elToRemove = el;
                    }
                }

                if(elToRemove != null)
                {
                    Elements.Remove(elToRemove);
                    drillingLineCheckBox.Checked = false;
                }

                string fileExt = fileName.Split('.').LastOrDefault();
                if(fileExt.Length == 3)
                {
                    char drill = fileExt[0];
                    if(drill == 'k')
                    {
                        cutterType = CutterType.Spherical;
                    }
                    else
                    {
                        cutterType = CutterType.Flat;
                    }

                    string size = fileExt[1].ToString() + fileExt[2].ToString();
                    int r;
                    if (int.TryParse(size, out r))
                    {
                        radious = r * 10;
                    }
                    else
                    {
                        radious = 50;
                    }
                }
                else
                {
                    cutterType = CutterType.Spherical;
                    radious = 50;
                }

                if(cutterType == CutterType.Spherical)
                {
                    sphericalRadioButton.Checked = true;
                }
                else
                {
                    flatRadioButton.Checked = true;
                }

                radiousTextBox.Text = (radious / 10.0f).ToString();
                drillButton.Enabled = true;

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
        }

        private void cameraLightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            cameraLight = cameraLightCheckBox.Checked;
        }

        private void radiousTextBox_TextChanged(object sender, EventArgs e)
        {
            float r;
            if (float.TryParse(radiousTextBox.Text, out r))
            {
                radious = (int)(r * 10);
                if (radiousTextBox.Text.Contains(".") && radiousTextBox.Text.Split('.').LastOrDefault().Length > 1 || radiousTextBox.Text.Split('.').Length > 2)
                {
                    radiousTextBox.Text = (radious / 10.0f).ToString();
                }
            }
            else
            {
                radiousTextBox.Text = (radious / 10.0f).ToString();
            }
        }

        private void drillButton_Click(object sender, EventArgs e)
        {
            millModel.DrillAll(drillingLines.drillPoints, cutterType, drillType, radious, drillHeight);
        }

        private void normalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            drillType = DrillType.Normal;
        }

        private void quickRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            drillType = DrillType.Fast;
        }

        private void parallelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            drillType = DrillType.Parallel;
        }

        private void sphericalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            cutterType = CutterType.Spherical;
        }

        private void flatRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            cutterType = CutterType.Flat;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(millModel != null && millModel.topLayer != null)
            {
                for(int i=0;i<dataModel.Width;i++)
                {
                    for(int j=0;j<dataModel.Height;j++)
                    {
                        millModel.topLayer[i, j] = dataModel.Altitude;
                    }
                }
            }
        }

        private void progressBar_Click(object sender, EventArgs e)
        {
        }

        private void simulationTickTrackBar_Scroll(object sender, EventArgs e)
        {
            simulationTick[0] = simulationTickTrackBar.Value;
        }

        private void drillHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            float drillH;
            if (float.TryParse(drillHeightTextBox.Text, out drillH))
            {
                drillHeight = (int)(drillH * 10);
                if (drillHeightTextBox.Text.Contains(".") && drillHeightTextBox.Text.Split('.').LastOrDefault().Length > 1 || drillHeightTextBox.Text.Split('.').Length > 2)
                {
                    drillHeightTextBox.Text = (drillHeight / 10.0f).ToString();
                }
            }
            else
            {
                drillHeightTextBox.Text = (drillHeight / 10.0f).ToString();
            }
        }

        private void InitSolution()
        {
            dataModel = new InitializeDataModel();
            InitializeData initializeData = new InitializeData(dataModel);
            initializeData.ShowDialog();

            if(dataModel.Width == -1)
            {
                Environment.Exit(Environment.ExitCode);
            }
            else
            {
                widthLabelValue.Text = (dataModel.Width / 10.0f).ToString() + " mm";
                heightLabelValue.Text = (dataModel.Height / 10.0f).ToString() + " mm";
                altitudeLabelValue.Text = (dataModel.Altitude * 10.0f).ToString() + " mm";
                divisionsXLabelValue.Text = (dataModel.Divisions_X).ToString();
                divisionsYLabelValue.Text = (dataModel.Divisions_Y).ToString();
            }
        }
    }
}
