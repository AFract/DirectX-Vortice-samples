using System.Runtime.InteropServices;

namespace AFract.SurroundingSound.Helpers;

public static class AudioBufferTools
{
    public static AudioBuffer ToAudioBuffer(short[] pcmData, BufferFlags flags = BufferFlags.EndOfStream)
    {
        // Code initial :
        /*unsafe
    {
        fixed (short* ptr = pcmData)
        {
            var buffer = new AudioBuffer
            {
                AudioBytes = (uint)(pcmData.Length * sizeof(short)),
                AudioDataPointer = new IntPtr(ptr),
                Flags = BufferFlags.EndOfStream,
            };
            sourceVoice.SubmitSourceBuffer(buffer);
        }
    }*/

        return new AudioBuffer
        {
            AudioBytes = (uint)(pcmData.Length * sizeof(short)),
            AudioDataPointer = Marshal.UnsafeAddrOfPinnedArrayElement(pcmData, 0),
            Flags = flags,
        };
    }
}