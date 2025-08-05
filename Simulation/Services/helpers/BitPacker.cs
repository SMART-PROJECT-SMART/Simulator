using System.Collections;
using Simulation.Common.constants;

namespace Simulation.Services.Helpers;

public static class BitPacker
{
    public static byte[] ToByteArray(this BitArray bits)
    {
        int numBytes = (bits.Length + SimulationConstants.Networking.BYTE_SIZE -1) / SimulationConstants.Networking.BYTE_SIZE;
        byte[] bytes = new byte[numBytes];
        bits.CopyTo(bytes, 0);
        return bytes;
    }

    public static BitArray FromByteArray(this byte[] bytes, int originalBitCount)
    {
        var bits = new BitArray(bytes);
        if (bits.Length != originalBitCount)
        {
            var trimmed = new BitArray(originalBitCount);
            for (int i = 0; i < originalBitCount; i++)
                trimmed[i] = bits[i];
            return trimmed;
        }
        return bits;
    }
}