using Geometric2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geometric2
{
    public partial class InitializeData : Form
    {
        InitializeDataModel dataModel;
        public InitializeData(InitializeDataModel dataModel)
        {
            InitializeComponent();
            this.dataModel = dataModel;
        }

        private void InitializeData_Load(object sender, EventArgs e)
        {
            widthTextBox.Text = "200.0";
            heightTextBox.Text = "400.0";
            altitudeTextBox.Text = "50.0";
            divisions_X_textbox.Text = "1000";
            divisions_Y_textbox.Text = "1000";
        }

        private void initializeButton_Click(object sender, EventArgs e)
        {
            bool canSave = true;
            float width = 0, height = 0, altitude = 0;
            int divisionsX = 0, divisionsY = 0;
            if(!float.TryParse(widthTextBox.Text, out width) || !float.TryParse(heightTextBox.Text, out height) || !float.TryParse(altitudeTextBox.Text, out altitude))
            {
                canSave = false;
                MessageBox.Show("Width and Height have to be float values");
            }

            if (!int.TryParse(divisions_X_textbox.Text, out divisionsX) || !int.TryParse(divisions_Y_textbox.Text, out divisionsY))
            {
                canSave = false;
                MessageBox.Show("Sivisions have to be integer values");
            }

            if(canSave)
            {
                dataModel.Width = (int)(width * 10);
                dataModel.Height = (int)(height * 10);
                dataModel.Altitude = altitude / 10.0f;
                dataModel.Divisions_X = divisionsX;
                dataModel.Divisions_Y = divisionsY;
                this.Close();
            }
        }
    }
}
