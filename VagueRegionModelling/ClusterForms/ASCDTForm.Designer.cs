namespace VagueRegionModelling.ClusterForms
{
    partial class ASCDTForm
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
            this.inputAndOutput = new VagueRegionModelling.Widgets.InputAndOutput();
            this.buttonCancle1 = new VagueRegionModelling.Widgets.ButtonCancle();
            this.checkBoxLocal = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // inputAndOutput
            // 
            this.inputAndOutput.Location = new System.Drawing.Point(26, 21);
            this.inputAndOutput.Name = "inputAndOutput";
            this.inputAndOutput.Size = new System.Drawing.Size(391, 98);
            this.inputAndOutput.TabIndex = 0;
            // 
            // buttonCancle1
            // 
            this.buttonCancle1.Location = new System.Drawing.Point(251, 174);
            this.buttonCancle1.Name = "buttonCancle1";
            this.buttonCancle1.Size = new System.Drawing.Size(81, 29);
            this.buttonCancle1.TabIndex = 1;
            // 
            // checkBoxLocal
            // 
            this.checkBoxLocal.AutoSize = true;
            this.checkBoxLocal.Location = new System.Drawing.Point(26, 137);
            this.checkBoxLocal.Name = "checkBoxLocal";
            this.checkBoxLocal.Size = new System.Drawing.Size(72, 16);
            this.checkBoxLocal.TabIndex = 15;
            this.checkBoxLocal.Text = "局部聚类";
            this.checkBoxLocal.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(112, 177);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 16;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // ASCDTForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 220);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxLocal);
            this.Controls.Add(this.buttonCancle1);
            this.Controls.Add(this.inputAndOutput);
            this.Name = "ASCDTForm";
            this.Text = "ASCDT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Widgets.InputAndOutput inputAndOutput;
        private Widgets.ButtonCancle buttonCancle1;
        private System.Windows.Forms.CheckBox checkBoxLocal;
        private System.Windows.Forms.Button buttonOK;
    }
}