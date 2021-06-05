namespace RenameX.Tests.Infrastructure
{
    public class MemoryDriveData
    {
        public static MemoryDrive FillWithDefaultData(MemoryDrive drive)
        {
            return drive
                .AddDirectory(new MemoryDirectory("/tmp"))
                .AddDirectory(new MemoryDirectory("/home/documents")
                    .AddFile(new MemoryFile("/home/documents/text file.txt"))
                    .AddFile(new MemoryFile("/home/documents/Math Homework.pdf"))
                    .AddFile(new MemoryFile("/home/documents/My Last Offer.docx"))
                    .AddFile(new MemoryFile("/home/documents/logfile"))
                )
                .AddDirectory(new MemoryDirectory("/home/pictures/Fishing Trip")
                    .AddFile(new MemoryFile("/home/pictures/Fishing Trip/photo 1.jpg"))
                    .AddFile(new MemoryFile("/home/pictures/Fishing Trip/photo 2.jpg"))
                    .AddFile(new MemoryFile("/home/pictures/Fishing Trip/photo 3.jpg"))
                    .AddFile(new MemoryFile("/home/pictures/Fishing Trip/photo4.jpg"))
                    .AddFile(new MemoryFile("/home/pictures/Fishing Trip/photo-5.jpg"))
                )
            ;
        }
    }
}
