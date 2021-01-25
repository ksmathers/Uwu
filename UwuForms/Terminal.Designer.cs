
namespace UwuForms
{
    partial class Terminal
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.screenBuffer = new System.Windows.Forms.PictureBox();
            this.screenRefreshTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.screenBuffer)).BeginInit();
            this.SuspendLayout();
            // 
            // screenBuffer
            // 
            this.screenBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenBuffer.Location = new System.Drawing.Point(0, 0);
            this.screenBuffer.Name = "screenBuffer";
            this.screenBuffer.Size = new System.Drawing.Size(1425, 947);
            this.screenBuffer.TabIndex = 0;
            this.screenBuffer.TabStop = false;
            // 
            // timer1
            // 
            this.screenRefreshTimer.Tick += new System.EventHandler(this.screenRefreshTimer_Tick);
            // 
            // Terminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.screenBuffer);
            this.Name = "Terminal";
            this.Size = new System.Drawing.Size(1425, 947);
            ((System.ComponentModel.ISupportInitialize)(this.screenBuffer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox screenBuffer;
        private System.Windows.Forms.Timer screenRefreshTimer;
    }
}
