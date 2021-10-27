
namespace Geometric2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl1 = new OpenTK.GLControl();
            this.loadModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.divisionsYLabelValue = new System.Windows.Forms.Label();
            this.divisionsXLabelValue = new System.Windows.Forms.Label();
            this.altitudeLabelValue = new System.Windows.Forms.Label();
            this.heightLabelValue = new System.Windows.Forms.Label();
            this.widthLabelValue = new System.Windows.Forms.Label();
            this.divisions_Y_Label = new System.Windows.Forms.Label();
            this.divisions_X_Label = new System.Windows.Forms.Label();
            this.altitudeLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.widthLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.drillTypeLabel = new System.Windows.Forms.Label();
            this.parallelRadioButton = new System.Windows.Forms.RadioButton();
            this.quickRadioButton = new System.Windows.Forms.RadioButton();
            this.normalRadioButton = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cutterLabel = new System.Windows.Forms.Label();
            this.flatRadioButton = new System.Windows.Forms.RadioButton();
            this.sphericalRadioButton = new System.Windows.Forms.RadioButton();
            this.cameraLightCheckBox = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.drillingLineCheckBox = new System.Windows.Forms.CheckBox();
            this.menuStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(12, 24);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(1280, 896);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
            this.glControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.glControl1_KeyPress);
            this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.glControl1_PreviewKeyDown);
            // 
            // loadModelToolStripMenuItem
            // 
            this.loadModelToolStripMenuItem.Name = "loadModelToolStripMenuItem";
            this.loadModelToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1562, 24);
            this.menuStrip2.TabIndex = 2;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.cameraLightCheckBox);
            this.panel1.Controls.Add(this.progressBar);
            this.panel1.Controls.Add(this.drillingLineCheckBox);
            this.panel1.Location = new System.Drawing.Point(1298, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 869);
            this.panel1.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.divisionsYLabelValue);
            this.panel4.Controls.Add(this.divisionsXLabelValue);
            this.panel4.Controls.Add(this.altitudeLabelValue);
            this.panel4.Controls.Add(this.heightLabelValue);
            this.panel4.Controls.Add(this.widthLabelValue);
            this.panel4.Controls.Add(this.divisions_Y_Label);
            this.panel4.Controls.Add(this.divisions_X_Label);
            this.panel4.Controls.Add(this.altitudeLabel);
            this.panel4.Controls.Add(this.heightLabel);
            this.panel4.Controls.Add(this.widthLabel);
            this.panel4.Location = new System.Drawing.Point(12, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(226, 150);
            this.panel4.TabIndex = 3;
            // 
            // divisionsYLabelValue
            // 
            this.divisionsYLabelValue.AutoSize = true;
            this.divisionsYLabelValue.Location = new System.Drawing.Point(108, 122);
            this.divisionsYLabelValue.Name = "divisionsYLabelValue";
            this.divisionsYLabelValue.Size = new System.Drawing.Size(35, 13);
            this.divisionsYLabelValue.TabIndex = 9;
            this.divisionsYLabelValue.Text = "label6";
            // 
            // divisionsXLabelValue
            // 
            this.divisionsXLabelValue.AutoSize = true;
            this.divisionsXLabelValue.Location = new System.Drawing.Point(108, 98);
            this.divisionsXLabelValue.Name = "divisionsXLabelValue";
            this.divisionsXLabelValue.Size = new System.Drawing.Size(35, 13);
            this.divisionsXLabelValue.TabIndex = 8;
            this.divisionsXLabelValue.Text = "label7";
            // 
            // altitudeLabelValue
            // 
            this.altitudeLabelValue.AutoSize = true;
            this.altitudeLabelValue.Location = new System.Drawing.Point(108, 59);
            this.altitudeLabelValue.Name = "altitudeLabelValue";
            this.altitudeLabelValue.Size = new System.Drawing.Size(35, 13);
            this.altitudeLabelValue.TabIndex = 7;
            this.altitudeLabelValue.Text = "label8";
            // 
            // heightLabelValue
            // 
            this.heightLabelValue.AutoSize = true;
            this.heightLabelValue.Location = new System.Drawing.Point(108, 35);
            this.heightLabelValue.Name = "heightLabelValue";
            this.heightLabelValue.Size = new System.Drawing.Size(35, 13);
            this.heightLabelValue.TabIndex = 6;
            this.heightLabelValue.Text = "label9";
            // 
            // widthLabelValue
            // 
            this.widthLabelValue.AutoSize = true;
            this.widthLabelValue.Location = new System.Drawing.Point(108, 13);
            this.widthLabelValue.Name = "widthLabelValue";
            this.widthLabelValue.Size = new System.Drawing.Size(41, 13);
            this.widthLabelValue.TabIndex = 5;
            this.widthLabelValue.Text = "label10";
            // 
            // divisions_Y_Label
            // 
            this.divisions_Y_Label.AutoSize = true;
            this.divisions_Y_Label.Location = new System.Drawing.Point(24, 122);
            this.divisions_Y_Label.Name = "divisions_Y_Label";
            this.divisions_Y_Label.Size = new System.Drawing.Size(68, 13);
            this.divisions_Y_Label.TabIndex = 4;
            this.divisions_Y_Label.Text = "Divisions (Y):";
            // 
            // divisions_X_Label
            // 
            this.divisions_X_Label.AutoSize = true;
            this.divisions_X_Label.Location = new System.Drawing.Point(24, 98);
            this.divisions_X_Label.Name = "divisions_X_Label";
            this.divisions_X_Label.Size = new System.Drawing.Size(68, 13);
            this.divisions_X_Label.TabIndex = 3;
            this.divisions_X_Label.Text = "Divisions (X):";
            // 
            // altitudeLabel
            // 
            this.altitudeLabel.AutoSize = true;
            this.altitudeLabel.Location = new System.Drawing.Point(42, 59);
            this.altitudeLabel.Name = "altitudeLabel";
            this.altitudeLabel.Size = new System.Drawing.Size(45, 13);
            this.altitudeLabel.TabIndex = 2;
            this.altitudeLabel.Text = "Altitude:";
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(46, 35);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(41, 13);
            this.heightLabel.TabIndex = 1;
            this.heightLabel.Text = "Height:";
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(49, 13);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(38, 13);
            this.widthLabel.TabIndex = 0;
            this.widthLabel.Text = "Width:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.drillTypeLabel);
            this.panel3.Controls.Add(this.parallelRadioButton);
            this.panel3.Controls.Add(this.quickRadioButton);
            this.panel3.Controls.Add(this.normalRadioButton);
            this.panel3.Location = new System.Drawing.Point(57, 189);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(151, 103);
            this.panel3.TabIndex = 4;
            // 
            // drillTypeLabel
            // 
            this.drillTypeLabel.AutoSize = true;
            this.drillTypeLabel.Location = new System.Drawing.Point(18, 9);
            this.drillTypeLabel.Name = "drillTypeLabel";
            this.drillTypeLabel.Size = new System.Drawing.Size(54, 13);
            this.drillTypeLabel.TabIndex = 3;
            this.drillTypeLabel.Text = "Drill Type:";
            // 
            // parallelRadioButton
            // 
            this.parallelRadioButton.AutoSize = true;
            this.parallelRadioButton.Location = new System.Drawing.Point(21, 71);
            this.parallelRadioButton.Name = "parallelRadioButton";
            this.parallelRadioButton.Size = new System.Drawing.Size(59, 17);
            this.parallelRadioButton.TabIndex = 2;
            this.parallelRadioButton.TabStop = true;
            this.parallelRadioButton.Text = "Parallel";
            this.parallelRadioButton.UseVisualStyleBackColor = true;
            // 
            // quickRadioButton
            // 
            this.quickRadioButton.AutoSize = true;
            this.quickRadioButton.Location = new System.Drawing.Point(21, 48);
            this.quickRadioButton.Name = "quickRadioButton";
            this.quickRadioButton.Size = new System.Drawing.Size(53, 17);
            this.quickRadioButton.TabIndex = 1;
            this.quickRadioButton.TabStop = true;
            this.quickRadioButton.Text = "Quick";
            this.quickRadioButton.UseVisualStyleBackColor = true;
            // 
            // normalRadioButton
            // 
            this.normalRadioButton.AutoSize = true;
            this.normalRadioButton.Location = new System.Drawing.Point(21, 25);
            this.normalRadioButton.Name = "normalRadioButton";
            this.normalRadioButton.Size = new System.Drawing.Size(58, 17);
            this.normalRadioButton.TabIndex = 0;
            this.normalRadioButton.TabStop = true;
            this.normalRadioButton.Text = "Normal";
            this.normalRadioButton.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cutterLabel);
            this.panel2.Controls.Add(this.flatRadioButton);
            this.panel2.Controls.Add(this.sphericalRadioButton);
            this.panel2.Location = new System.Drawing.Point(57, 298);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(151, 87);
            this.panel2.TabIndex = 3;
            // 
            // cutterLabel
            // 
            this.cutterLabel.AutoSize = true;
            this.cutterLabel.Location = new System.Drawing.Point(16, 9);
            this.cutterLabel.Name = "cutterLabel";
            this.cutterLabel.Size = new System.Drawing.Size(38, 13);
            this.cutterLabel.TabIndex = 4;
            this.cutterLabel.Text = "Cutter:";
            // 
            // flatRadioButton
            // 
            this.flatRadioButton.AutoSize = true;
            this.flatRadioButton.Location = new System.Drawing.Point(19, 48);
            this.flatRadioButton.Name = "flatRadioButton";
            this.flatRadioButton.Size = new System.Drawing.Size(42, 17);
            this.flatRadioButton.TabIndex = 2;
            this.flatRadioButton.TabStop = true;
            this.flatRadioButton.Text = "Flat";
            this.flatRadioButton.UseVisualStyleBackColor = true;
            // 
            // sphericalRadioButton
            // 
            this.sphericalRadioButton.AutoSize = true;
            this.sphericalRadioButton.Location = new System.Drawing.Point(19, 25);
            this.sphericalRadioButton.Name = "sphericalRadioButton";
            this.sphericalRadioButton.Size = new System.Drawing.Size(69, 17);
            this.sphericalRadioButton.TabIndex = 1;
            this.sphericalRadioButton.TabStop = true;
            this.sphericalRadioButton.Text = "Spherical";
            this.sphericalRadioButton.UseVisualStyleBackColor = true;
            // 
            // cameraLightCheckBox
            // 
            this.cameraLightCheckBox.AutoSize = true;
            this.cameraLightCheckBox.Location = new System.Drawing.Point(123, 635);
            this.cameraLightCheckBox.Name = "cameraLightCheckBox";
            this.cameraLightCheckBox.Size = new System.Drawing.Size(88, 17);
            this.cameraLightCheckBox.TabIndex = 2;
            this.cameraLightCheckBox.Text = "Camera Light";
            this.cameraLightCheckBox.UseVisualStyleBackColor = true;
            this.cameraLightCheckBox.CheckedChanged += new System.EventHandler(this.cameraLightCheckBox_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 832);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(226, 23);
            this.progressBar.TabIndex = 1;
            // 
            // drillingLineCheckBox
            // 
            this.drillingLineCheckBox.AutoSize = true;
            this.drillingLineCheckBox.Location = new System.Drawing.Point(123, 602);
            this.drillingLineCheckBox.Name = "drillingLineCheckBox";
            this.drillingLineCheckBox.Size = new System.Drawing.Size(85, 17);
            this.drillingLineCheckBox.TabIndex = 0;
            this.drillingLineCheckBox.Text = "Drilling Lines";
            this.drillingLineCheckBox.UseVisualStyleBackColor = true;
            this.drillingLineCheckBox.CheckedChanged += new System.EventHandler(this.drillingLineCheckBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1562, 932);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.menuStrip2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.ToolStripMenuItem loadModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox drillingLineCheckBox;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.CheckBox cameraLightCheckBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label divisions_X_Label;
        private System.Windows.Forms.Label altitudeLabel;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton parallelRadioButton;
        private System.Windows.Forms.RadioButton quickRadioButton;
        private System.Windows.Forms.RadioButton normalRadioButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton flatRadioButton;
        private System.Windows.Forms.RadioButton sphericalRadioButton;
        private System.Windows.Forms.Label divisionsYLabelValue;
        private System.Windows.Forms.Label divisionsXLabelValue;
        private System.Windows.Forms.Label altitudeLabelValue;
        private System.Windows.Forms.Label heightLabelValue;
        private System.Windows.Forms.Label widthLabelValue;
        private System.Windows.Forms.Label divisions_Y_Label;
        private System.Windows.Forms.Label drillTypeLabel;
        private System.Windows.Forms.Label cutterLabel;
    }
}

