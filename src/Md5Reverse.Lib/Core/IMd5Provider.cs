namespace Md5Reverse.Lib.Core
{
    public interface IMd5Provider
    {
        uint ComputeUIntHash(uint input);
        byte[] ComputeByteHash(uint input);
        byte[] ComputeByteHash(long input);
    }
}