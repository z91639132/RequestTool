using MyOA.URIResource;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZhengnanLib;

namespace ConvertTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnToGet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            var jdResult = this.ProcessData(TypeEnum.ToGet, textBox.Text);
            if (jdResult["status"].ToInt32() != 200)
            {
                this.ShowException(jdResult["message"].ToString());
                return;
            }

            var txt = jdResult["data"]["txt"].ToString();
            if (!string.IsNullOrEmpty(txt))
            {
                textBox.Text = txt;
            }
            btnCompressEscapeChars.Visible = false;
        }

        private void btnToForm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            var jdResult = this.ProcessData(TypeEnum.ToForm, textBox.Text);
            if (jdResult["status"].ToInt32() != 200)
            {
                this.ShowException(jdResult["message"].ToString());
                return;
            }

            var txt = jdResult["data"]["txt"].ToString();
            if (!string.IsNullOrEmpty(txt))
            {
                textBox.Text = txt;
            }
            btnCompressEscapeChars.Visible = false;
        }

        private void btnToJson_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }

            var jdResult = this.ProcessData(TypeEnum.ToJson, textBox.Text);
            if (jdResult["status"].ToInt32() != 200)
            {
                this.ShowException(jdResult["message"].ToString());
                return;
            }

            var txt = jdResult["data"]["txt"].ToString();
            if (!string.IsNullOrEmpty(txt))
            {
                textBox.Text = txt;
                btnCompressEscapeChars.Visible = true;
            }
            else
            {
                btnCompressEscapeChars.Visible = false;
            }
        }

        private void btnCompressEscapeChars_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }

            var jdResult = this.ProcessData(TypeEnum.ToEscapeChars, textBox.Text);
            if (jdResult["status"].ToInt32() != 200)
            {
                this.ShowException(jdResult["message"].ToString());
                return;
            }

            var txt = jdResult["data"]["txt"].ToString();
            if (!string.IsNullOrEmpty(txt))
            {
                textBox.Text = txt;
                btnCompressEscapeChars.Visible = false;
            }
        }

        #region 私有方法

        private JsonData ProcessData(TypeEnum type, string txt)
        {
            var url = string.Empty;

            switch (type)
            {
                case TypeEnum.ToGet:
                    url = "http://139.224.107.91:7011/ConvertTool.Service/ToolService/ToGet";
                    break;
                case TypeEnum.ToForm:
                    url = "http://139.224.107.91:7011/ConvertTool.Service/ToolService/ToForm";
                    break;
                case TypeEnum.ToJson:
                    url = "http://139.224.107.91:7011/ConvertTool.Service/ToolService/ToJson";
                    break;
                case TypeEnum.ToEscapeChars:
                    url = "http://139.224.107.91:7011/ConvertTool.Service/ToolService/ToEscapeChars";
                    break;
                default:
                    break;
            }

            var jd = new JsonData(JsonType.Object);
            jd["txt"] = txt;

            var jdResult = RequestHelper.RequestServer(url, jd);
            return jdResult;
        }

        /// <summary>
        /// 弹出提示
        /// </summary>
        private void ShowException(Exception e)
        {
            MessageBox.Show(this, e.Message, "参数转换工具 - 正南", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        /// <summary>
        /// 弹出提示
        /// </summary>
        private void ShowException(string eMessage)
        {
            MessageBox.Show(this, eMessage, "参数转换工具 - 正南", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        #endregion

    }
}
