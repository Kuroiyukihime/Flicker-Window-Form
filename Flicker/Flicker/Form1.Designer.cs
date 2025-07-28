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
            label1 = new Label();
            label2 = new Label();
            txtWord = new TextBox();
            txtFlickerRate = new TextBox();
            btnStart = new Button();
            btnPauseResume = new Button();
            btnBack = new Button();
            pictureBoxDisplay = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDisplay).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 28);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 2;
            label1.Text = "Word:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 76);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 3;
            label2.Text = "Flicker Rate";
            // 
            // txtWord
            // 
            txtWord.Location = new Point(84, 27);
            txtWord.Name = "txtWord";
            txtWord.Size = new Size(100, 23);
            txtWord.TabIndex = 4;
            // 
            // txtFlickerRate
            // 
            txtFlickerRate.Location = new Point(94, 68);
            txtFlickerRate.Name = "txtFlickerRate";
            txtFlickerRate.Size = new Size(100, 23);
            txtFlickerRate.TabIndex = 5;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(35, 119);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 6;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnPauseResume
            // 
            btnPauseResume.Location = new Point(56, 386);
            btnPauseResume.Name = "btnPauseResume";
            btnPauseResume.Size = new Size(75, 23);
            btnPauseResume.TabIndex = 7;
            btnPauseResume.Text = "Pause";
            btnPauseResume.UseVisualStyleBackColor = true;
            btnPauseResume.Click += btnPauseResume_Click;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(163, 386);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(75, 23);
            btnBack.TabIndex = 8;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // pictureBoxDisplay
            // 
            pictureBoxDisplay.BackColor = SystemColors.WindowText;
            pictureBoxDisplay.Location = new Point(395, 56);
            pictureBoxDisplay.Name = "pictureBoxDisplay";
            pictureBoxDisplay.Size = new Size(318, 273);
            pictureBoxDisplay.TabIndex = 9;
            pictureBoxDisplay.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBoxDisplay);
            Controls.Add(btnBack);
            Controls.Add(btnPauseResume);
            Controls.Add(btnStart);
            Controls.Add(txtFlickerRate);
            Controls.Add(txtWord);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxDisplay).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private TextBox txtWord;
        private TextBox txtFlickerRate;
        private Button btnStart;
        private Button btnPauseResume;
        private Button btnBack;
        private PictureBox pictureBoxDisplay;
    }
}
