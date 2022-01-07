using MyOA.URIResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZhengnanLib;

namespace ConvertTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //这行代码，可以让该程序访问公司框架写的服务层代码
            MyOA.URIResource.IoC.config($"{System.AppDomain.CurrentDomain.BaseDirectory}/IoC.txt");

            //这行代码，可以让该程序访问数据库
            MyOA.URIResource.IoC.BuildUp<MyOA.URIResource.Node>().Initialization($"{ System.AppDomain.CurrentDomain.BaseDirectory}".TrimEnd('/', '\\'), false);

            var url = "http://139.224.107.91:7011/ConvertTool.Service/ToolService/Power";
            var jd = new JsonData(JsonType.Object);

            var jdResult = RequestHelper.RequestServer(url, jd);
            if (jdResult["status"].ToInt32() != 200)
            {
                MessageBox.Show(jdResult["message"].ToString(), "参数转换工具 - 正南", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }



    }
}
