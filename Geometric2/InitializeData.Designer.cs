
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
            this.SuspendLayout();
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(74, 289);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(121, 35);
            this.initializeButton.TabIndex = 0;
            this.initializeButton.Text = "Initialize";
            this.initializeButton.UseVisualStyleBackColor = true;
            // 
            // InitializeData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 336);
            this.Controls.Add(this.initializeButton);
            this.Name = "InitializeData";
            this.Text = "InitializeData";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button initializeButton;
    }
}