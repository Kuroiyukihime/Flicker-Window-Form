namespace Flicker
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            label2 = new Label();
            txtWord = new TextBox();
            txtMonitorRate = new TextBox();
            btnStart = new Button();
            btnPauseResume = new Button();
            btnCancel = new Button();
            pictureBoxDisplay = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBoxDisplay).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 47);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(60, 25);
            label1.TabIndex = 2;
            label1.Text = "Word:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 127);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(120, 25);
            label2.TabIndex = 3;
            label2.Text = "Monitor Rate:";
            // 
            // txtWord
            // 
            txtWord.Location = new Point(120, 45);
            txtWord.Margin = new Padding(4, 5, 4, 5);
            txtWord.Name = "txtWord";
            txtWord.Size = new Size(141, 31);
            txtWord.TabIndex = 4;
            // 
            // txtMonitorRate
            // 
            txtMonitorRate.Location = new Point(134, 113);
            txtMonitorRate.Margin = new Padding(4, 5, 4, 5);
            txtMonitorRate.Name = "txtMonitorRate";
            txtMonitorRate.Size = new Size(141, 31);
            txtMonitorRate.TabIndex = 5;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(50, 198);
            btnStart.Margin = new Padding(4, 5, 4, 5);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(107, 38);
            btnStart.TabIndex = 6;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnPauseResume
            // 
            btnPauseResume.Location = new Point(80, 643);
            btnPauseResume.Margin = new Padding(4, 5, 4, 5);
            btnPauseResume.Name = "btnPauseResume";
            btnPauseResume.Size = new Size(107, 38);
            btnPauseResume.TabIndex = 7;
            btnPauseResume.Text = "Pause";
            btnPauseResume.UseVisualStyleBackColor = true;
            btnPauseResume.Click += btnPauseResume_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(233, 643);
            btnCancel.Margin = new Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(107, 38);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // pictureBoxDisplay
            // 
            pictureBoxDisplay.BackColor = Color.White;
            pictureBoxDisplay.Location = new Point(377, 14);
            pictureBoxDisplay.Margin = new Padding(4, 5, 4, 5);
            pictureBoxDisplay.Name = "pictureBoxDisplay";
            pictureBoxDisplay.Size = new Size(730, 700);
            pictureBoxDisplay.TabIndex = 9;
            pictureBoxDisplay.TabStop = false;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 750);
            Controls.Add(pictureBoxDisplay);
            Controls.Add(btnCancel);
            Controls.Add(btnPauseResume);
            Controls.Add(btnStart);
            Controls.Add(txtMonitorRate);
            Controls.Add(txtWord);
            Controls.Add(label2);
            Controls.Add(label1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Flicker Window Form";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxDisplay).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private TextBox txtWord;
        private TextBox txtMonitorRate;
        private Button btnStart;
        private Button btnPauseResume;
        private Button btnCancel;
        private PictureBox pictureBoxDisplay;
        private System.Windows.Forms.Timer timer1;
    }
}
