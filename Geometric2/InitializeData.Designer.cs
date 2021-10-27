
namespace Geometric2
{
    partial class InitializeData
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
            this.initializeButton = new System.Windows.Forms.Button();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.divisions_Y_textbox = new System.Windows.Forms.TextBox();
            this.divisions_X_textbox = new System.Windows.Forms.TextBox();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.Divisions_X_Label = new System.Windows.Forms.Label();
            this.Divisions_Y_Label = new System.Windows.Forms.Label();
            this.AltitudeLabel = new System.Windows.Forms.Label();
            this.altitudeTextBox = new System.Windows.Forms.TextBox();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(44, 217);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(184, 35);
            this.initializeButton.TabIndex = 0;
            this.initializeButton.Text = "Initialize";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // widthTextBox
            // 
            this.widthTextBox.Location = new System.Drawing.Point(107, 27);
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(121, 20);
            this.widthTextBox.TabIndex = 1;
            // 
            // divisions_Y_textbox
            // 
            this.divisions_Y_textbox.Location = new System.Drawing.Point(107, 155);
            this.divisions_Y_textbox.Name = "divisions_Y_textbox";
            this.divisions_Y_textbox.Size = new System.Drawing.Size(121, 20);
            this.divisions_Y_textbox.TabIndex = 4;
            // 
            // divisions_X_textbox
            // 
            this.divisions_X_textbox.Location = new System.Drawing.Point(107, 129);
            this.divisions_X_textbox.Name = "divisions_X_textbox";
            this.divisions_X_textbox.Size = new System.Drawing.Size(121, 20);
            this.divisions_X_textbox.TabIndex = 3;
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(41, 30);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(60, 13);
            this.WidthLabel.TabIndex = 5;
            this.WidthLabel.Text = "Width (mm)";
            // 
            // Divisions_X_Label
            // 
            this.Divisions_X_Label.AutoSize = true;
            this.Divisions_X_Label.Location = new System.Drawing.Point(41, 132);
            this.Divisions_X_Label.Name = "Divisions_X_Label";
            this.Divisions_X_Label.Size = new System.Drawing.Size(65, 13);
            this.Divisions_X_Label.TabIndex = 7;
            this.Divisions_X_Label.Text = "Divisions (X)";
            // 
            // Divisions_Y_Label
            // 
            this.Divisions_Y_Label.AutoSize = true;
            this.Divisions_Y_Label.Location = new System.Drawing.Point(41, 158);
            this.Divisions_Y_Label.Name = "Divisions_Y_Label";
            this.Divisions_Y_Label.Size = new System.Drawing.Size(65, 13);
            this.Divisions_Y_Label.TabIndex = 8;
            this.Divisions_Y_Label.Text = "Divisions (Y)";
            // 
            // AltitudeLabel
            // 
            this.AltitudeLabel.AutoSize = true;
            this.AltitudeLabel.Location = new System.Drawing.Point(38, 83);
            this.AltitudeLabel.Name = "AltitudeLabel";
            this.AltitudeLabel.Size = new System.Drawing.Size(67, 13);
            this.AltitudeLabel.TabIndex = 10;
            this.AltitudeLabel.Text = "Altitude (mm)";
            // 
            // altitudeTextBox
            // 
            this.altitudeTextBox.Location = new System.Drawing.Point(107, 80);
            this.altitudeTextBox.Name = "altitudeTextBox";
            this.altitudeTextBox.Size = new System.Drawing.Size(121, 20);
            this.altitudeTextBox.TabIndex = 9;
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(38, 57);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(63, 13);
            this.HeightLabel.TabIndex = 12;
            this.HeightLabel.Text = "Height (mm)";
            // 
            // heightTextBox
            // 
            this.heightTextBox.Location = new System.Drawing.Point(107, 53);
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(121, 20);
            this.heightTextBox.TabIndex = 11;
            // 
            // InitializeData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 290);
            this.Controls.Add(this.HeightLabel);
            this.Controls.Add(this.heightTextBox);
            this.Controls.Add(this.AltitudeLabel);
            this.Controls.Add(this.altitudeTextBox);
            this.Controls.Add(this.Divisions_Y_Label);
            this.Controls.Add(this.Divisions_X_Label);
            this.Controls.Add(this.WidthLabel);
            this.Controls.Add(this.divisions_Y_textbox);
            this.Controls.Add(this.divisions_X_textbox);
            this.Controls.Add(this.widthTextBox);
            this.Controls.Add(this.initializeButton);
            this.Name = "InitializeData";
            this.Text = "InitializeData";
            this.Load += new System.EventHandler(this.InitializeData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button initializeButton;
        private System.Windows.Forms.TextBox widthTextBox;
        private System.Windows.Forms.TextBox divisions_Y_textbox;
        private System.Windows.Forms.TextBox divisions_X_textbox;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label Divisions_X_Label;
        private System.Windows.Forms.Label Divisions_Y_Label;
        private System.Windows.Forms.Label AltitudeLabel;
        private System.Windows.Forms.TextBox altitudeTextBox;
        private System.Windows.Forms.Label HeightLabel;
        private System.Windows.Forms.TextBox heightTextBox;
    }
}