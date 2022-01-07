using MyOA.URIResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhengnanLib;

namespace ConvertTool
{
    public class RequestHelper
    {
        /// <summary>
        /// 请求服务器
        /// </summary>
        public static JsonData RequestServer(string url, JsonData jd)
        {
#warning 这里要根据发布方式改下
            //domain调用 （本机调用）
            //Prefix配置："rpc/converttool.service": "domain/ConvertTool.Service"
            url = url.Replace("http://139.224.107.91:7011", "rpc");
            var jdResult = RpcHelper_Ext.Rpc(jd, url);

            //直接写死网址
            //var jdResult = RpcHelper_Ext.Rpc(jd, url);

            //如果服务器变动了，那么修改IoC配置添加Rpc服务即可
            if (jdResult["status"].ToInt32() == 500)
            {
                //rpc网址调用（通过IoC配置网址） 
                //KV配置："rpc-converttool.service": "http://139.224.107.91:7011/ConvertTool.Service"

                url = url.Replace("http://139.224.107.91:7011", "rpc");
                jdResult = RpcHelper_Ext.Rpc(jd, url);
            }

            return jdResult;
        }

    }
}
