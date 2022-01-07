using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace ConvertTool
{
    public class Node : MyOA.URIResource.Node
    {
        public override void Initialization(string root, bool loadNodeList = true)
        {
            base.Initialization(root, loadNodeList);
        }

        public override void Finalization()
        {
            base.Finalization();
        }
    }
}