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

namespace Geometric2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    glControl1.Invalidate();
                    //Thread.Sleep(16);
                }
            });

            thread.Start();

            coursor = new Coursor();
            millModel = new MillModel();
            coursor.CoursorMode = CoursorMode.Auto;
            transformCenterLines.selectedElements = SelectedElements;
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
        }

        private Shader _shader;
        private Shader _millshader;
        private Camera _camera;
        private Coursor coursor;
        private MillModel millModel;


        private XyzLines xyzLines = new XyzLines();
        private List<Element> Elements = new List<Element>();
        private List<Element> SelectedElements = new List<Element>();
        DrawingStatus drawingStatus = DrawingStatus.No;
        private TransformCenterLines transformCenterLines = new TransformCenterLines();

        private Torus selectedTorus = null;

        int prev_xPosMouse = -1, prev_yPosMouse = -1;
        int torusNumber;

        private void loadModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Elements.Clear();
            SelectedElements.Clear();
            Elements.Add(xyzLines);
            Elements.Add(transformCenterLines);
            torusNumber = 0;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.Title = "Select Xml File";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string previous = "";
                List<ModelGeneration.Torus> toruses = new List<ModelGeneration.Torus>();

                //using (XmlReader reader = XmlReader.Create(fileName, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                //{
                //    while (reader.Read())
                //    {
                //        switch (reader.NodeType)
                //        {
                //            case XmlNodeType.Element:
                //                if (reader.Name == "Point")
                //                {
                //                    previous = "Point";
                //                    var pointName = reader.GetAttribute("Name");
                //                    ModelGeneration.Point point = new ModelGeneration.Point();
                //                    point._camera = _camera;
                //                    point.FullName = pointName;
                //                    point.pointNumber = pointNumber;
                //                    pointNumber++;
                //                    points.Add(point);
                //                }
                //                else if (reader.Name == "Position" && previous == "Point")
                //                {
                //                    ModelGeneration.Point pp = points.Last();
                //                    var X = float.Parse(reader.GetAttribute("X"));
                //                    var Y = float.Parse(reader.GetAttribute("Y"));
                //                    var Z = float.Parse(reader.GetAttribute("Z"));
                //                    pp.CenterPosition = new Vector3(X, Y, Z);
                //                }
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //}

                using (XmlReader reader = XmlReader.Create(fileName, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "Torus")
                                {
                                    previous = "Torus";
                                    var torusName = reader.GetAttribute("Name");
                                    var MinorRadius = float.Parse(reader.GetAttribute("MinorRadius"));
                                    var MajorRadius = float.Parse(reader.GetAttribute("MajorRadius"));
                                    var MajorSegments = int.Parse(reader.GetAttribute("MajorSegments"));
                                    var MinorSegments = int.Parse(reader.GetAttribute("MinorSegments"));
                                    ModelGeneration.Torus torus = new ModelGeneration.Torus();
                                    torus.FullName = torusName;
                                    torus.torusNumber = torusNumber;
                                    torus.torusMajorDividions = MajorSegments;
                                    torus.torusMinorDividions = MinorSegments;
                                    torus.torus_r = MinorRadius;
                                    torus.torus_R = MajorRadius;
                                    torusNumber++;
                                    toruses.Add(torus);
                                }
  

                                Console.WriteLine("Start Element {0}", reader.Name);
                                break;
                            case XmlNodeType.Text:
                                Console.WriteLine("Text Node: {0}", reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                break;
                            default:
                                Console.WriteLine("Other node {0} with value {1}",
                                                reader.NodeType, reader.Value);
                                break;
                        }
                    }
                }

                foreach (var t in toruses)
                {
                    t.CreateGlElement(_shader);
                }
                Elements.AddRange(toruses);

                torusNumber += toruses.Count;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.FileName = "Model";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (XmlWriter writer = XmlWriter.Create(saveFileDialog.FileName, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true }))
                {
                    writer.WriteStartElement("Scene");
                    foreach (var el in Elements)
                    {
                        if (el is ModelGeneration.Torus torus)
                        {
                            writer.WriteStartElement("Torus");
                            writer.WriteAttributeString("MinorRadius", torus.torus_r.ToString());
                            writer.WriteAttributeString("MajorRadius", torus.torus_R.ToString());
                            writer.WriteAttributeString("MajorSegments", torus.torusMajorDividions.ToString());
                            writer.WriteAttributeString("MinorSegments", torus.torusMinorDividions.ToString());
                            writer.WriteAttributeString("Name", torus.FullName);
                            writer.WriteStartElement("Position");
                            writer.WriteAttributeString("X", torus.Position().X.ToString());
                            writer.WriteAttributeString("Y", torus.Position().Y.ToString());
                            writer.WriteAttributeString("Z", torus.Position().Z.ToString());
                            writer.WriteStartElement("Rotation");
                            writer.WriteAttributeString("X", torus.RotationQuaternion.X.ToString());
                            writer.WriteAttributeString("Y", torus.RotationQuaternion.Y.ToString());
                            writer.WriteAttributeString("Z", torus.RotationQuaternion.Z.ToString());
                            writer.WriteAttributeString("W", torus.RotationQuaternion.W.ToString());
                            writer.WriteStartElement("Scale");
                            writer.WriteAttributeString("X", torus.ElementScale.ToString());
                            writer.WriteAttributeString("Y", torus.ElementScale.ToString());
                            writer.WriteAttributeString("Z", torus.ElementScale.ToString());
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                }
            }
        }
    }
}
