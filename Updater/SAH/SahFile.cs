namespace Updater.SAH
{
    public class SahFile
    {
        public SahFile(string name, long offset, long size, SahFolder folder)
        {
            Name = name;
            Offset = offset;
            Size = size;
            Folder = folder;
        }
        public SahFile(string name, long size, SahFolder folder)
        {
            Name = name;
            Size = size;
            Folder = folder;
        }

        public string Name;
        public long Offset;
        public long Size;
        public SahFolder Folder;
    }
}
