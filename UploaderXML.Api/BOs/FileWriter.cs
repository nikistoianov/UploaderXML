namespace UploaderXML.Api.BOs
{
    public class FileWriter : IFileWriter
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void FileDelete(string path)
        {
            File.Delete(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public async Task WriteAsync(string path, byte[] data)
        {
            using var fileStream = File.Create(path);
            await fileStream.WriteAsync(data);
        }
    }
}
