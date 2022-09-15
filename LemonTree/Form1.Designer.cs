
namespace LemonTree
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlList1 = new LemonTree.CustomControls.ControlList();
            this.button1 = new System.Windows.Forms.Button();
            this.monacoPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // controlList1
            // 
            this.controlList1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.controlList1.Dock = System.Windows.Forms.DockStyle.Left;
            this.controlList1.Location = new System.Drawing.Point(0, 0);
            this.controlList1.Name = "controlList1";
            this.controlList1.Size = new System.Drawing.Size(462, 1178);
            this.controlList1.TabIndex = 0;
            this.controlList1.Load += new System.EventHandler(this.controlList1_Load);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(2133, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(204, 52);
            this.button1.TabIndex = 2;
            this.button1.Text = "Compile";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // monacoPanel
            // 
            this.monacoPanel.Location = new System.Drawing.Point(465, 94);
            this.monacoPanel.Name = "monacoPanel";
            this.monacoPanel.Size = new System.Drawing.Size(1872, 1072);
            this.monacoPanel.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(2349, 1178);
            this.Controls.Add(this.monacoPanel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.controlList1);
            this.Name = "Form1";
            this.Text = "Lemon Tree";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ClientSizeChanged += new System.EventHandler(this.Form1_ClientSizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControls.ControlList controlList1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel monacoPanel;
    }
}

