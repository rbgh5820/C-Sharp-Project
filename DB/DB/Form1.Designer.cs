
namespace DB
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textMovie = new System.Windows.Forms.TextBox();
            this.textDi = new System.Windows.Forms.TextBox();
            this.textType = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(185, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(476, 40);
            this.button1.TabIndex = 0;
            this.button1.Text = "DB select";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "영화이름";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 258);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "감독";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 371);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "장르";
            // 
            // textMovie
            // 
            this.textMovie.Location = new System.Drawing.Point(185, 151);
            this.textMovie.Name = "textMovie";
            this.textMovie.Size = new System.Drawing.Size(346, 31);
            this.textMovie.TabIndex = 4;
            // 
            // textDi
            // 
            this.textDi.Location = new System.Drawing.Point(185, 258);
            this.textDi.Name = "textDi";
            this.textDi.Size = new System.Drawing.Size(342, 31);
            this.textDi.TabIndex = 5;
            // 
            // textType
            // 
            this.textType.Location = new System.Drawing.Point(185, 365);
            this.textType.Name = "textType";
            this.textType.Size = new System.Drawing.Size(342, 31);
            this.textType.TabIndex = 6;
            this.textType.TextChanged += new System.EventHandler(this.textType_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textType);
            this.Controls.Add(this.textDi);
            this.Controls.Add(this.textMovie);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textMovie;
        private System.Windows.Forms.TextBox textDi;
        private System.Windows.Forms.TextBox textType;
    }
}

