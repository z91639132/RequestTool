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

            var txt = RemoveEscapeChars(textBox.Text);

            var stringBuilder = new StringBuilder();

            try
            {
                var jsonDic = JsonSerializerHelper.JsonStringToKeyValuePairs(txt.Replace("\r\n", ""));

                if (jsonDic != null)//如果是json
                {
                    foreach (var item in jsonDic)
                    {
                        var key = item.Key.Trim();
                        var value = item.Value.Trim();

                        value = System.Web.HttpUtility.UrlDecode(value);//万一从json来的值是编码的
                        value = value.Replace("\\\"", "\"");
                        value = System.Web.HttpUtility.UrlEncode(value);

                        if (!string.IsNullOrEmpty(key))
                        {
                            stringBuilder.Append($"{key}={value}&");
                        }
                    }
                }
                else//可能是form
                {
                    var lines = txt.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.All(t => t.Contains(":")))
                    {
                        foreach (var item in lines)
                        {
                            var keyValues = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (keyValues.Length == 2)
                            {
                                var key = keyValues[0].Trim();
                                var value = keyValues[1].Trim();

                                value = System.Web.HttpUtility.UrlDecode(value);//万一从form来的值是编码的
                                value = value.Replace("\\\"", "\"");
                                value = System.Web.HttpUtility.UrlEncode(value);

                                if (!string.IsNullOrEmpty(key))
                                {
                                    stringBuilder.Append($"{key}={value}&");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            var str = stringBuilder.ToString().TrimEnd('&');
            if (!string.IsNullOrEmpty(str))
            {
                textBox.Text = str;
            }
            btnCompressEscapeChars.Visible = false;
        }

        private void btnToForm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }

            var txt = RemoveEscapeChars(textBox.Text);

            var stringBuilder = new StringBuilder();
            try
            {
                var jsonDic = JsonSerializerHelper.JsonStringToKeyValuePairs(txt.Replace("\r\n", ""));

                if (jsonDic != null)//如果是json
                {
                    foreach (var item in jsonDic)
                    {
                        var key = item.Key.Trim();
                        var value = item.Value.Trim();

                        value = System.Web.HttpUtility.UrlDecode(value);
                        if (!string.IsNullOrEmpty(key))
                        {
                            stringBuilder.AppendLine($"{key}:{value}");
                        }
                    }
                }
                else//可能是query string
                {
                    var items = txt.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in items)
                    {
                        var keyValues = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (keyValues.Length == 2)
                        {
                            var key = keyValues[0].Trim();
                            var value = keyValues[1].Trim();

                            value = System.Web.HttpUtility.UrlDecode(value);
                            value = value.Replace("\\\"", "\"");

                            if (!string.IsNullOrEmpty(key))
                            {
                                stringBuilder.AppendLine($"{key}:{value}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            var str = stringBuilder.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                textBox.Text = str;
            }
            btnCompressEscapeChars.Visible = false;
        }

        private void btnToJson_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }

            var txt = RemoveEscapeChars(textBox.Text);

            var strList = new List<string>();

            try
            {
                var lines = txt.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.All(t => t.Contains(":")))
                {
                    foreach (var item in lines)
                    {
                        var keyValues = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (keyValues.Length == 2)
                        {
                            var key = keyValues[0].Trim();
                            var value = keyValues[1].Trim();

                            value = System.Web.HttpUtility.UrlDecode(value);
                            value = value.Replace("\"", "\\\"");

                            if (!string.IsNullOrEmpty(key))
                            {
                                if (value == "true" || value == "false" || IsNum(value))
                                {
                                    strList.Add($"\"{key}\":{value},");
                                }
                                else
                                {
                                    strList.Add($"\"{key}\":\"{value}\",");
                                }
                            }
                        }
                    }
                }
                else
                {
                    var items = txt.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.All(t => t.Contains("=")))
                    {
                        foreach (var item in items)
                        {
                            var keyValues = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (keyValues.Length == 2)
                            {
                                var key = keyValues[0].Trim();
                                var value = keyValues[1].Trim();

                                value = System.Web.HttpUtility.UrlDecode(value);
                                value = value.Replace("\"", "\\\"");

                                if (!string.IsNullOrEmpty(key))
                                {
                                    if (value == "true" || value == "false" || IsNum(value))
                                    {
                                        strList.Add($"\"{key}\":{value},");
                                    }
                                    else
                                    {
                                        strList.Add($"\"{key}\":\"{value}\",");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            if (strList.Count > 0)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("{");

                strList[strList.Count - 1] = strList.Last().TrimEnd(',');
                foreach (var str in strList)
                {
                    stringBuilder.Append(str);
                }

                stringBuilder.Append("}");

                textBox.Text = RichTextBoxHelper.FormatJsonString(stringBuilder.ToString());

                btnCompressEscapeChars.Visible = true;
            }
            else if (JsonSerializerHelper.IsJsonString(txt))
            {
                textBox.Text = RichTextBoxHelper.FormatJsonString(txt);
                btnCompressEscapeChars.Visible = true;
            }
            else
            {
                textBox.Text = string.Empty;
                btnCompressEscapeChars.Visible = false;
            }
        }

        private void btnCompressEscapeChars_Click(object sender, EventArgs e)
        {
            try
            {
                var jsonStringData = RichTextBoxHelper.CompressJsonString(textBox.Text);

                //因为引号使用了反斜杠\ , 所以先操作反斜杠\  再操作引号
                // 如果先操作引号的话, 那引号转义后产生的反斜杠\再转义将会产生更多的反斜杠\
                string str = jsonStringData.Replace("\\", "\\\\").Replace("\"", "\\\"");
                textBox.Text = str;
                btnCompressEscapeChars.Visible = false;
            }
            catch (Exception exception)
            {
                this.ShowException(exception);
            }
        }

        #region 私有方法

        /// <summary>
        /// 弹出提示
        /// </summary>
        private void ShowException(Exception e)
        {
            MessageBox.Show(this, e.Message, "ConvertTool", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        /// <summary>
        /// 移除转义字符
        /// </summary>
        private string RemoveEscapeChars(string txt)
        {
            Regex regex = new Regex(@"\s*{\s*\\\""[^\\]+\\\""\s*:");
            if (regex.IsMatch(txt))
            {
                txt = txt.Replace("\\\\", "\\").Replace("\\\"", "\"");
            }

            return txt;
        }

        /// <summary>
        /// 验证字符串是否是数字
        /// </summary>
        private bool IsNum(string str)
        {
            Regex r = new Regex(@"^[+-]?\d*(,\d{3})*(\.\d+)?$");
            if (r.IsMatch(str))
            {
                return true;
            }
            return false;
        }
        #endregion

    }
}
