using AFract.SurroundingSound.Data;
using Microsoft.VisualBasic;

namespace AFract.SurroundingSound.Helpers;

public static class SoundGenerator
{
    /// <summary>
    /// Generate a PCM sinus mono sample
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="durationSec"></param>
    /// <param name="sampleRate"></param>
    /// <returns></returns>
    public static short[] GenerateSine(float frequency, float durationSec, int sampleRate = SoundFormatConstants.DefaultSampleRate)
    {
        var count = (int)(durationSec * sampleRate);
        var samples = new short[count];
        for (var i = 0; i < count; i++)
        {
            var angle = 2.0 * Math.PI * frequency * i / sampleRate;
            samples[i] = (short)(Math.Sin(angle) * short.MaxValue * 0.6);
        }
        return samples;
    }

    /// <summary>
    /// Generate a PCM sinus mono sample
    /// </summary>
    /// <param name="startFrequency"></param>
    /// <param name="endFrequency"></param>
    /// <param name="durationSec"></param>
    /// <param name="sampleRate"></param>
    /// <returns></returns>
    public static short[] GenerateSineVariableFrequency(float startFrequency, float endFrequency, float durationSec, int sampleRate = SoundFormatConstants.DefaultSampleRate)
    {
        var count = (int)(durationSec * sampleRate);
        var samples = new short[count];
        for (var i = 0; i < count; i++)
        {
            var frequency = startFrequency + (endFrequency - startFrequency) * i / count;
            var angle = 2.0 * Math.PI * frequency * i / sampleRate;
            samples[i] = (short)(Math.Sin(angle) * short.MaxValue * 0.6);
        }
        return samples;
    }
}