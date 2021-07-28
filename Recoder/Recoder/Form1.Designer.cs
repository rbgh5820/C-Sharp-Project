
namespace Recoder
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Record_Audio = new System.Windows.Forms.Button();
            this.Save_Stop = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // Record_Audio
            // 
            this.Record_Audio.Location = new System.Drawing.Point(475, 100);
            this.Record_Audio.Name = "Record_Audio";
            this.Record_Audio.Size = new System.Drawing.Size(482, 74);
            this.Record_Audio.TabIndex = 0;
            this.Record_Audio.Text = "Record";
            this.Record_Audio.UseVisualStyleBackColor = true;
            this.Record_Audio.Click += new System.EventHandler(this.Record_Audio_Click);
            // 
            // Save_Stop
            // 
            this.Save_Stop.Location = new System.Drawing.Point(594, 354);
            this.Save_Stop.Name = "Save_Stop";
            this.Save_Stop.Size = new System.Drawing.Size(238, 63);
            this.Save_Stop.TabIndex = 1;
            this.Save_Stop.Text = "Save/Stop";
            this.Save_Stop.UseVisualStyleBackColor = true;
            this.Save_Stop.Click += new System.EventHandler(this.Save_Stop_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1412, 845);
            this.Controls.Add(this.Save_Stop);
            this.Controls.Add(this.Record_Audio);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Record_Audio;
        private System.Windows.Forms.Button Save_Stop;
        private System.Windows.Forms.Timer timer1;
    }
}

