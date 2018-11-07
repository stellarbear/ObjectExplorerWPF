using System.Collections.Generic;

namespace ObjectExplorerWPF
{
    public class ViewModel
    {
        public Explorer Explorer { get; set; }

        public ViewModel(IEnumerable<string> dirs, ESelectionRestrictions type, string filter = "*")
        {
            Explorer = new Explorer(dirs, type, filter);
        }

        public void AppendNode(string node)
        {
            Explorer.AppendNode(node);
        }
        public IEnumerable<string> GetCheckedNodes()
        {
            return Explorer.GetCheckedNodes();
        }
    }
}
