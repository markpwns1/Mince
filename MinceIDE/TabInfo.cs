using FastColoredTextBoxNS;
using System.IO;
using System.Windows.Forms;

namespace MinceIDE
{
    public class TabInfo
    {
        public int originalHashCode;
        public string saveLocation = null;
        public TabPage tab;

        public string fileName
        {
            get { return saveLocation == null ? "Untitled" : saveLocation.Substring(saveLocation.LastIndexOf("\\") + 1); }
        }

        public FastColoredTextBox textBox
        {
            get { return (FastColoredTextBox)tab.Controls[0]; }
        }

        public bool isSaved
        {
            get { return originalHashCode == textBox.Text.GetHashCode(); }
        }

        public TabInfo(TabPage tab, string saveLocation = null)
        {
            this.tab = tab;
            this.saveLocation = saveLocation;

            originalHashCode = saveLocation == null ? "".GetHashCode() : File.ReadAllText(saveLocation).GetHashCode();
        }

        public static TabInfo GetInfo(TabPage page)
        {
            return (TabInfo)page.Tag;
        }
    }
}
