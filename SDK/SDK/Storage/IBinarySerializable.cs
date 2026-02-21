using System.IO;

namespace FreeFramework.SDK.Storage;

/// <summary>
/// All types that should be allowed to be written or read from IStorageProvider should inherit this interface
/// </summary>
public interface IBinarySerializable {
    /// <summary>
    /// Writes the child object into a BinaryWriter.
    /// </summary>
    /// <remarks>MUST MIRROR THE READ METHOD</remarks>
    public void Write(BinaryWriter writer);
    
    /// <summary>
    /// Reads the child object from a BinaryReader
    /// </summary>
    /// <remarks>MUST MIRROR THE WRITE METHOD</remarks>
    public void Read(BinaryReader reader);
}