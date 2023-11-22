namespace UploaderXML.Api.BOs
{
    public interface IFileWriter
    {
        void CreateDirectory(string path);

        bool DirectoryExists(string path);

        void FileDelete(string path);

        bool FileExists(string path);

        Task WriteAsync(string path, byte[] data);
    }
}
