
namespace ConvertTool
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox = new System.Windows.Forms.TextBox();
            this.btnToGet = new System.Windows.Forms.Button();
            this.btnToForm = new System.Windows.Forms.Button();
            this.btnToJson = new System.Windows.Forms.Button();
            this.btnCompressEscapeChars = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(12, 12);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(622, 329);
            this.textBox.TabIndex = 0;
            // 
            // btnToGet
            // 
            this.btnToGet.Location = new System.Drawing.Point(154, 356);
            this.btnToGet.Name = "btnToGet";
            this.btnToGet.Size = new System.Drawing.Size(90, 31);
            this.btnToGet.TabIndex = 1;
            this.btnToGet.Text = "转Get";
            this.btnToGet.UseVisualStyleBackColor = true;
            this.btnToGet.Click += new System.EventHandler(this.btnToGet_Click);
            // 
            // btnToForm
            // 
            this.btnToForm.Location = new System.Drawing.Point(267, 356);
            this.btnToForm.Name = "btnToForm";
            this.btnToForm.Size = new System.Drawing.Size(90, 31);
            this.btnToForm.TabIndex = 1;
            this.btnToForm.Text = "转Form";
            this.btnToForm.UseVisualStyleBackColor = true;
            this.btnToForm.Click += new System.EventHandler(this.btnToForm_Click);
            // 
            // btnToJson
            // 
            this.btnToJson.Location = new System.Drawing.Point(377, 356);
            this.btnToJson.Name = "btnToJson";
            this.btnToJson.Size = new System.Drawing.Size(90, 31);
            this.btnToJson.TabIndex = 1;
            this.btnToJson.Text = "转Json";
            this.btnToJson.UseVisualStyleBackColor = true;
            this.btnToJson.Click += new System.EventHandler(this.btnToJson_Click);
            // 
            // btnCompressEscapeChars
            // 
            this.btnCompressEscapeChars.Location = new System.Drawing.Point(485, 357);
            this.btnCompressEscapeChars.Name = "btnCompressEscapeChars";
            this.btnCompressEscapeChars.Size = new System.Drawing.Size(90, 31);
            this.btnCompressEscapeChars.TabIndex = 3;
            this.btnCompressEscapeChars.Text = "压缩转义";
            this.btnCompressEscapeChars.UseVisualStyleBackColor = true;
            this.btnCompressEscapeChars.Visible = false;
            this.btnCompressEscapeChars.Click += new System.EventHandler(this.btnCompressEscapeChars_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 400);
            this.Controls.Add(this.btnCompressEscapeChars);
            this.Controls.Add(this.btnToJson);
            this.Controls.Add(this.btnToForm);
            this.Controls.Add(this.btnToGet);
            this.Controls.Add(this.textBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "参数转换工具 - 正南";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button btnToGet;
        private System.Windows.Forms.Button btnToForm;
        private System.Windows.Forms.Button btnToJson;
        private System.Windows.Forms.Button btnCompressEscapeChars;
    }
}

