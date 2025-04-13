using System.Collections.Generic;
using System.IO;
public class LargeFilePartReader 
{
    public class FilePart
    {
        public long ID;
        public long offset;
        public int length;

        public FilePart(long iD, long offset, int length)
        {
            ID = iD;
            this.offset = offset;
            this.length = length;
        }

        public byte[] ReadData(BinaryReader reader)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            return reader.ReadBytes(length);
        }
    }
    string fileName;
    Dictionary<long,FilePart> partList;
    BinaryReader persistentStrem;
    FileStream persistentFileStrem;
    public LargeFilePartReader(string fileName)
    {
        this.fileName = fileName;
        persistentFileStrem = File.Open(fileName, FileMode.Open);
        persistentStrem = new BinaryReader(persistentFileStrem);
        int listLen = persistentStrem.ReadInt32();
        partList = new Dictionary<long, FilePart>(listLen);
        for (int i = 0; i < listLen; i++)
        {
            FilePart part = new FilePart(
                persistentStrem.ReadInt64(), 
                persistentStrem.ReadInt64(), 
                persistentStrem.ReadInt32());
            partList.Add(part.ID, part);
        }
        persistentStrem.Close();//?????
        persistentFileStrem.Close();//????
    }
    public byte[] LoadPartData(long ID)
    {
        persistentFileStrem = File.Open(fileName, FileMode.Open);//?????
        persistentStrem = new BinaryReader(persistentFileStrem);//?????
        byte[] data = null;
        if (partList.TryGetValue(ID, out FilePart part))
        {
            data = part.ReadData(persistentStrem);
        }
        persistentStrem.Close();//?????
        persistentFileStrem.Close();//????
        return data;
    }
}
