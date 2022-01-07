using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertTool
{
    public enum  TypeEnum
    {
        /// <summary>
        /// 转get
        /// </summary>
        ToGet = 1,
        /// <summary>
        /// 转form
        /// </summary>
        ToForm = 2,
        /// <summary>
        /// 转json
        /// </summary>
        ToJson = 3,
        /// <summary>
        /// 转义
        /// </summary>
        ToEscapeChars = 4
    }
}
