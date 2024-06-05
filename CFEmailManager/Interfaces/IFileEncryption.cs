namespace CFEmailManager.Interfaces
{
    /// <summary>
    /// Interface for storing file in encrypted format
    /// </summary>
    public interface IFileEncryption
    {
        string Name { get; }

        void WriteFile(string file, byte[] content);

        byte[] ReadFile(string file);

        //Stream ReadFile(string file);
    }
}
