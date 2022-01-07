namespace ConvertTool.Service
{
    public class Node : MyOA.URIResource.Node
    {
        public override void Initialization(string root)
        {
            base.Initialization(root,false);
        }
    }
}