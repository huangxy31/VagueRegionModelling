namespace VagueRegionModelling.ClusterForms
{
    partial class DBSCANForm
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancle1 = new VagueRegionModelling.Widgets.ButtonCancle();
            this.textBoxMinPts = new VagueRegionModelling.Widgets.TextBoxNum();
            this.textBoxEps = new VagueRegionModelling.Widgets.TextBoxNum();
            this.inputAndOutput = new VagueRegionModelling.Widgets.InputAndOutput();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(126, 243);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 20;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "周围点个数(MinPts)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "区域半径(Eps)";
            // 
            // buttonCancle1
            // 
            this.buttonCancle1.Location = new System.Drawing.Point(281, 240);
            this.buttonCancle1.Name = "buttonCancle1";
            this.buttonCancle1.Size = new System.Drawing.Size(81, 29);
            this.buttonCancle1.TabIndex = 25;
            // 
            // textBoxMinPts
            // 
            this.textBoxMinPts.Location = new System.Drawing.Point(59, 200);
            this.textBoxMinPts.Name = "textBoxMinPts";
            this.textBoxMinPts.Size = new System.Drawing.Size(389, 26);
            this.textBoxMinPts.TabIndex = 24;
            // 
            // textBoxEps
            // 
            this.textBoxEps.Location = new System.Drawing.Point(59, 143);
            this.textBoxEps.Name = "textBoxEps";
            this.textBoxEps.Size = new System.Drawing.Size(389, 26);
            this.textBoxEps.TabIndex = 23;
            // 
            // inputAndOutput
            // 
            this.inputAndOutput.Location = new System.Drawing.Point(59, 12);
            this.inputAndOutput.Name = "inputAndOutput";
            this.inputAndOutput.Size = new System.Drawing.Size(391, 98);
            this.inputAndOutput.TabIndex = 22;
            // 
            // DBSCANForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 289);
            this.Controls.Add(this.buttonCancle1);
            this.Controls.Add(this.textBoxMinPts);
            this.Controls.Add(this.textBoxEps);
            this.Controls.Add(this.inputAndOutput);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Name = "DBSCANForm";
            this.Text = "DBSCAN";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Widgets.InputAndOutput inputAndOutput;
        private Widgets.TextBoxNum textBoxEps;
        private Widgets.TextBoxNum textBoxMinPts;
        private Widgets.ButtonCancle buttonCancle1;
    }
}