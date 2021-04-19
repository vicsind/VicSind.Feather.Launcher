using System.Collections.Generic;

namespace Updater.SAH
{
    public class SahFolder
    {
        public SahFolder(string name, SahFolder parent)
        {
            Name = name;
            Parent = parent;
        }

        public string Name;
        public SahFolder Parent;

        public List<SahFolder> Folders = new List<SahFolder>();
        public List<SahFile> Files = new List<SahFile>();
    }
}
