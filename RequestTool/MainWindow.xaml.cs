using CsharpHttpHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RequestTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //todo  切换按钮时 下方请求参数随之改变
        //todo  各环境配置文件，页面加载

        private Regex ChromeRequestRegex = new Regex(@"^(?<key>[^{}\[\]:]+?):(?<value>.+?)$", RegexOptions.Multiline);
        //private Regex TopRequestRegex = new Regex(@"^(?<key>.+?)=>(?<value>.+?)$", RegexOptions.Multiline);  //以前测试或前端会给这个, 目前暂不考虑

        private Regex cookieRegex = new Regex("^cookie:.*", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private Regex jsonRegex = new Regex("{.*}$", RegexOptions.Multiline);

        private Regex Red_BlueRegex = new Regex(@"(\s*"".+?"")(:)(\s"".*?"")(,?)$");
        private Regex Red_GreenRegex = new Regex(@"(\s*"".+?"")(:)(\s[^""\s\{\[,]+)(,?)");
        private Regex Red_BlackRegex = new Regex(@"(\s*"".+?"")(:)(\s[\[\{\}\]]+)");
        private Regex RedBlackRegex = new Regex(@"(^\s*""[^""]*"")(,?)$");
        private Regex GreenBlackRegex = new Regex(@"(^\s*[-.\d]+)(,?)$");

        private void btnAnalysis_Click(object sender, RoutedEventArgs e)
        {
            var requestContent = tbRequestContent.Text;

            Dictionary<string, string> paramsDic = null;

            string url = null;

            if (requestContent.Contains("Request URL"))
            {
                rbNone.IsChecked = true;

                if (!this.ChromeRequestRegex.IsMatch(requestContent))
                {
                    MessageBox.Show("请求内容的格式不正确！");
                    return;
                }
                paramsDic = AnalysisChromeRequest(ChromeRequestRegex, requestContent, out url);
                this.tbOriginalURL.Text = url;
            }

            #region 以前测试或前端会给这个, 目前暂不考虑
            //else if (requestContent.Contains("url: =>"))
            //{
            //    rbForm.IsChecked = true;
            //    cbRequestType.SelectedValue = "POST";
            //    if (!this.TopRequestRegex.IsMatch(requestContent))
            //    {
            //        MessageBox.Show("请求信息的格式不正确！");
            //        return;
            //    }
            //    paramsDic = AnalysisTopRequest(TopRequestRegex, requestContent, out url);
            //} 
            #endregion

            #region 校验URL，显示请求域名前缀、URL

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("未解析到请求URL！");
                return;
            }

            if (url.ToLower().StartsWith("http"))
            {
                var index = this.GetIndex(url, "/", 3);
                if (index != -1)
                {
                    tbOriginalPrefix.Text = url.Substring(0, index - 1);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(tbOriginalPrefix.Text))
                {
                    MessageBox.Show("请求URL未包含域名前缀，请输入后重试！");
                    return;
                }

                url = $"{tbOriginalPrefix.Text.TrimEnd('/')}/{url.TrimStart('/')}";
            }

            #endregion

            if (paramsDic.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var item in paramsDic)
                {
                    sb.AppendLine($"{item.Key}:{item.Value}");
                }
                tbRequestData.Text = sb.ToString();
            }

            tbCookies.Text = cookieRegex.Match(requestContent).Value.Split(':')[1].Trim();
        }

        private Dictionary<string, string> AnalysisChromeRequest(Regex requestRegex, string requestContent, out string url)
        {
            url = string.Empty;
            var isQuery = false;

            var matches = requestRegex.Matches(requestContent);
            var beginValueIndex = 0;
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var groups = match.Groups;
                var key = groups["key"].Value.Trim();
                var value = groups["value"].Value.Trim();

                if (key == "Request URL" && !string.IsNullOrEmpty(value))
                {
                    url = value;
                    if (value.Contains("?"))
                    {
                        isQuery = true;
                        rbQuery.IsChecked = true;
                    }
                    continue;
                }
                else if (key == "Request Method")
                {
                    cbRequestType.Text = value;
                    continue;
                }
                else if (key == "content-type")
                {
                    var lowerValue = value.ToLower();
                    if (lowerValue.Contains("application/json".ToLower()) && !isQuery)
                    {
                        rbJson.IsChecked = true;
                    }
                    else if (lowerValue.Contains("application/x-www-form-urlencoded".ToLower()) && !isQuery)
                    {
                        rbForm.IsChecked = true;
                    }
                    continue;
                }
                else if (key == "User-Agent".ToLower())
                {
                    beginValueIndex = i;
                    continue;
                }
                else if (key == "X-Requested-With".ToLower())
                {
                    beginValueIndex = i;
                    continue;
                }
            }

            var paramsDic = new Dictionary<string, string>();
            for (int i = beginValueIndex + 1; i < matches.Count; i++)
            {
                var match = matches[i];
                var groups = match.Groups;
                var key = groups["key"].Value.Trim();
                var value = groups["value"].Value.Trim();

                if (!paramsDic.ContainsKey(key))
                {
                    paramsDic.Add(key, value);
                }
            }

            if (paramsDic.Count == 0)
            {
                if (rbQuery.IsChecked == true)
                {
                    var suffixStr = tbOriginalURL.Text.Split('?')[1];
                    if (!string.IsNullOrEmpty(suffixStr))
                    {
                        var suffixArray = suffixStr.Split('&');
                        foreach (var suffixItem in suffixArray)
                        {
                            var keyValues = suffixItem.Split('=');
                            if (keyValues.Count() == 2)
                            {
                                paramsDic.Add(keyValues[0], keyValues[1]);
                            }
                        }
                    }
                }
                else if (jsonRegex.IsMatch(requestContent))
                {
                    tbRequestData.Text = jsonRegex.Match(requestContent).Value.Trim();
                }
            }

            return paramsDic;
        }

        private Dictionary<string, string> AnalysisTopRequest(Regex requestRegex, string requestContent, out string url)
        {
            url = string.Empty;

            var paramsDic = new Dictionary<string, string>();

            var matches = requestRegex.Matches(requestContent);
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var groups = match.Groups;
                var key = groups["key"].Value.Trim();
                var value = groups["value"].Value.Trim();

                if (key == "url:" && !string.IsNullOrEmpty(value))
                {
                    url = value;
                    if (value.Contains("?"))
                    {
                        rbQuery.IsChecked = true;
                    }
                }
                else if (key == "data:" && !string.IsNullOrEmpty(value))
                {
                    //cbRequestType.Text = value; 
                    var subValues = value.Split('&');
                    foreach (var subValue in subValues)
                    {
                        var keyValues = subValue.Split('=');
                        if (keyValues.Count() == 2)
                        {
                            paramsDic.Add(keyValues[0], keyValues[1]);
                        }
                    }
                }
            }

            return paramsDic;
        }

        private void btnPostOld_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbOriginalURL.Text))
            {
                return;
            }

            string htmlData = this.GetResponseData(this.tbOriginalURL.Text.Trim(), this.tbRequestData.Text.Trim(), this.tbCookies.Text.Trim());
            this.ProcessResponseData(this.rtbOriginalResponse, htmlData);
            this.tiOriginalResponse.IsSelected = true;
        }

        private void tbLostFocus(object sender, RoutedEventArgs e)
        {
            //var textBox = (TextBox)sender;
            //textBox.Text = textBox.Text.TrimEnd('/');
            //textBox.SelectionStart = textBox.Text.Length;
        }

        private void rbQuery_Checked(object sender, RoutedEventArgs e)
        {
            RequestDataToQueryString(this.tbRequestData.Text);
        }

        private void tbRequestData_TextChanged(object sender, TextChangedEventArgs e)
        {
            RequestDataToQueryString(this.tbRequestData.Text);
        }

        #region 私有方法

        private int GetIndex(string text, string keyWord, int sequence)
        {
            int index = 0;
            int count = 0;
            while ((index = text.IndexOf(keyWord, index)) != -1)
            {
                index = index + keyWord.Length;
                count++;
                if (sequence == count)
                {
                    return index;
                }
            }
            return -1;
        }

        private string GetResponseData(string url, string requestStr, string cookies)
        {
            //创建Httphelper对象
            HttpHelper http = new HttpHelper();

            //创建Httphelper参数对象
            var item = new HttpItem()
            {
                URL = url,  //URL，必需项    
                Method = this.cbRequestType.Text,
                Encoding = Encoding.UTF8,
                Cookie = cookies
            };

            //其中None和QueryString都是通过URL直接访问的，不需要增加其他参数
            if (this.rbForm.IsChecked == true)
            {
                var lines = requestStr.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    var index = line.IndexOf(':');
                    if (index > 0 && index < line.Length - 1)
                    {
                        var str1 = line.Substring(0, index).Trim();
                        var str2 = line.Substring(index + 1).Trim();
                        if (!string.IsNullOrEmpty(str1))
                        {
                            sb.Append($"{str1}={str2}&");
                        }
                    }
                }
                var requestData = sb.ToString().TrimEnd('&');
                item.PostDataType = CsharpHttpHelper.Enum.PostDataType.String;
                item.Postdata = requestData;

                item.ContentType = "application/x-www-form-urlencoded";
            }
            else if (this.rbJson.IsChecked == true)
            {
                item.PostDataType = CsharpHttpHelper.Enum.PostDataType.String;
                if (!requestStr.StartsWith("{"))
                {
                    var lines = requestStr.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var sb = new StringBuilder();
                    foreach (var line in lines)
                    {
                        var index = line.IndexOf(':');
                        if (index > 0 && index < line.Length - 1)
                        {
                            var str1 = line.Substring(0, index).Trim();
                            var str2 = line.Substring(index + 1).Trim();
                            if (!string.IsNullOrEmpty(str1))
                            {
                                sb.Append($"\"{str1}\":{str2},");
                            }
                        }
                    }
                    item.Postdata = "{" + sb.ToString().TrimEnd(',') + "}";
                }
                else
                {
                    item.Postdata = requestStr.Replace("\r\n", " ");
                }

                item.ContentType = "application/json";
            }

            //请求的返回值对象
            HttpResult result = http.GetHtml(item);

            //获取当前页数据
            return result.Html;
        }

        private void ProcessResponseData(RichTextBox rtbResponse, string data)
        {
            var str = ConvertJsonString(data);

            rtbResponse.Document.Blocks.Clear();

            var lines = str.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                Paragraph paragraph = new Paragraph();

                if (this.Red_BlueRegex.IsMatch(line))
                {
                    var match = this.Red_BlueRegex.Match(line);

                    var run = new Run(match.Groups[1].Value);
                    run.Foreground = new SolidColorBrush(Color.FromRgb(163, 21, 21));//红色
                    paragraph.Inlines.Add(run);

                    run = new Run(match.Groups[2].Value);
                    //run.Foreground = Brushes.Black;
                    paragraph.Inlines.Add(run);

                    run = new Run(match.Groups[3].Value);
                    run.Foreground = new SolidColorBrush(Color.FromRgb(4, 81, 173));//蓝色
                    paragraph.Inlines.Add(run);

                    if (match.Groups.Count == 5)
                    {
                        run = new Run(match.Groups[4].Value);
                        //run.Foreground = Brushes.Black;
                        paragraph.Inlines.Add(run);
                    }
                }
                else if (this.Red_GreenRegex.IsMatch(line))
                {
                    var match = this.Red_GreenRegex.Match(line);

                    var run = new Run(match.Groups[1].Value);
                    run.Foreground = new SolidColorBrush(Color.FromRgb(163, 21, 21));//红色
                    paragraph.Inlines.Add(run);

                    run = new Run(match.Groups[2].Value);
                    //run.Foreground = Brushes.Black;
                    paragraph.Inlines.Add(run);

                    run = new Run(match.Groups[3].Value);
                    run.Foreground = new SolidColorBrush(Color.FromRgb(9, 136, 90));//绿色
                    paragraph.Inlines.Add(run);

                    if (match.Groups.Count == 5)
                    {
                        run = new Run(match.Groups[4].Value);
                        //run.Foreground = Brushes.Black;
                        paragraph.Inlines.Add(run);
                    }
                }
                else if (this.Red_BlackRegex.IsMatch(line))
                {
                    var match = this.Red_BlackRegex.Match(line);

                    var run = new Run(match.Groups[1].Value);
                    run.Foreground = new SolidColorBrush(Color.FromRgb(163, 21, 21));//红色
                    paragraph.Inlines.Add(run);

                    run = new Run($"{match.Groups[2].Value}{match.Groups[3].Value}");
                    //run.Foreground = Brushes.Black;
                    paragraph.Inlines.Add(run);
                }
                else if (this.RedBlackRegex.IsMatch(line))
                {
                    var match = this.RedBlackRegex.Match(line);

                    var run = new Run(match.Groups[1].Value);
                    run.Foreground = new SolidColorBrush(Color.FromRgb(163, 21, 21));//红色
                    paragraph.Inlines.Add(run);

                    if (match.Groups.Count == 3)
                    {
                        run = new Run(match.Groups[2].Value);
                        //run.Foreground = Brushes.Black;
                        paragraph.Inlines.Add(run);
                    }
                }
                else if (this.GreenBlackRegex.IsMatch(line))
                {
                    var match = this.GreenBlackRegex.Match(line);

                    var run = new Run(match.Groups[1].Value);
                    run.Foreground = new SolidColorBrush(Color.FromRgb(9, 136, 90));//绿色
                    paragraph.Inlines.Add(run);

                    if (match.Groups.Count == 3)
                    {
                        run = new Run(match.Groups[2].Value);
                        //run.Foreground = Brushes.Black;
                        paragraph.Inlines.Add(run);
                    }
                }
                else
                {
                    var run = new Run(line);
                    //run.Foreground = Brushes.Black;
                    paragraph.Inlines.Add(run);
                }

                rtbResponse.Document.Blocks.Add(paragraph);
            }
        }

        private string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }


        private void RequestDataToQueryString(object sender, TextChangedEventArgs e)
        {
            //如果是querystring，那么改变后，URL地址也要改变
            if (rbQuery.IsChecked == true)
            {
                var textBox = (TextBox)sender;
                var lines = textBox.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    var index = line.IndexOf(':');
                    if (index > 0 && index < line.Length)
                    {
                        var str1 = line.Substring(0, index).Trim();
                        var str2 = line.Substring(index).Trim();
                        if (!string.IsNullOrEmpty(str1))
                        {
                            sb.Append($"{str1}={str2}&");
                        }
                    }
                }
                var urlParaStr = sb.ToString().TrimEnd('&');

                var regex = new Regex("[^=&]*=[^=&]*");
                if (regex.IsMatch(urlParaStr) && regex.Matches(urlParaStr).Count > 0)
                {
                    if (tbOriginalURL.Text.Contains("?"))
                    {
                        tbOriginalURL.Text = $"{ tbOriginalURL.Text.Split('?')[0]}?{urlParaStr}";
                    }
                    else
                    {
                        tbOriginalURL.Text = $"{ tbOriginalURL.Text}?{urlParaStr}";
                    }
                }
            }
        }

        private void RequestDataToQueryString(string requestStr)
        {
            //如果是querystring，那么改变后，URL地址也要改变
            if (rbQuery.IsChecked == true)
            {
                var lines = requestStr.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    var index = line.IndexOf(':');
                    if (index > 0 && index < line.Length)
                    {
                        var str1 = line.Substring(0, index).Trim();
                        var str2 = line.Substring(index + 1).Trim();
                        if (!string.IsNullOrEmpty(str1))
                        {
                            sb.Append($"{str1}={str2}&");
                        }
                    }
                }
                var urlParaStr = sb.ToString().TrimEnd('&');

                var regex = new Regex("[^=&]*=[^=&]*");
                if (regex.IsMatch(urlParaStr) && regex.Matches(urlParaStr).Count > 0)
                {
                    if (tbOriginalURL.Text.Contains("?"))
                    {
                        tbOriginalURL.Text = $"{ tbOriginalURL.Text.Split('?')[0]}?{urlParaStr}";
                    }
                    else
                    {
                        tbOriginalURL.Text = $"{ tbOriginalURL.Text}?{urlParaStr}";
                    }
                }
            }
        }

        #endregion

        private void splitterLeft_MouseEnter(object sender, MouseEventArgs e)
        {
            var splitter = (GridSplitter)sender;
            splitter.Width = 2;
            splitter.Background = new SolidColorBrush(Color.FromRgb(145, 187, 230));//浅蓝色
        }

        private void splitterLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            var splitter = (GridSplitter)sender;
            splitter.Width = 2;
            splitter.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));//灰色  FFF0F0F0
        }

    }
}
