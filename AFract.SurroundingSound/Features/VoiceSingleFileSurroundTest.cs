using AFract.SurroundingSound.Data;
using AFract.SurroundingSound.Features.Abstractions;
using AFract.SurroundingSound.Helpers;
using Vortice;
using Vortice.MediaFoundation;
using Vortice.Multimedia;

namespace AFract.SurroundingSound.Features;

public class VoiceSingleFileSurroundTest()
    : SurroundDiscreteSourcesTestBase
{
    private readonly Dictionary<Speakers, DataStream>? _speakerDataStreams = new();

    protected override void BuildSamples()
    {
        if (_speakerDataStreams == null)
            throw new NullReferenceException(nameof(_speakerDataStreams));

        MediaFactory.MFStartup();

        using var stream = System.IO.File.OpenRead("Assets\\EnumerateSpeakers.wav");
        IMFSourceReader reader = MediaFactory.MFCreateSourceReaderFromByteStream(stream);

        DebugPrintTools.SourceReaderDisplayMediaTypes(reader);

        using IMFMediaType pcmType = MediaFactory.MFCreateMediaType();
        pcmType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Audio);
        pcmType.Set(MediaTypeAttributeKeys.Subtype, AudioFormatGuids.Pcm); // Sortie en PCM, même si le format d'entrée est en MP3
        pcmType.Set(MediaTypeAttributeKeys.AudioBitsPerSample, 16);
        pcmType.Set(MediaTypeAttributeKeys.AudioSamplesPerSecond, 48000);
        pcmType.Set(MediaTypeAttributeKeys.AudioNumChannels, 1);

        reader.SetCurrentMediaType(SourceReaderIndex.FirstAudioStream, pcmType);
        reader.SetStreamSelection(SourceReaderIndex.FirstAudioStream, true);

        const long hnsPerSecond = 10_000_000L; // Hundred of ns per second
        const float segmentDurationSeconds = 1.2f;
        const float segmentStep = 2f;
        long segmentStart = 0;

        foreach (var speaker in SpeakersDefinition.SpeakerPositions)
        {
            var segmentDataStream = new DataStream((int)(48000 * 2 * segmentDurationSeconds), true, true);
            segmentDataStream.Position = 0;

            reader.SetCurrentPosition(segmentStart);

            while (true)
            {
                IMFSample? sample = reader.ReadSample(SourceReaderIndex.FirstAudioStream, SourceReaderControlFlag.None,
                    out _, out SourceReaderFlag flags, out long timestamp);

                if (sample == null || flags.HasFlag(SourceReaderFlag.EndOfStream))
                    break;

                if (timestamp >= segmentStart + (segmentDurationSeconds * hnsPerSecond))
                {
                    sample.Dispose();
                    break;
                }

                var buffer = sample.ConvertToContiguousBuffer();
                buffer.Lock(out nint ptr, out _, out int length);
                segmentDataStream.Write(ptr, 0, length);
                buffer.Unlock();

                sample.Dispose();
            }

            segmentDataStream.Position = 0; // Reset pour AudioBuffer

            _speakerDataStreams.Add(speaker.Name, segmentDataStream);

            // Jump to the next segment for the next speaker
            segmentStart += (long)(segmentStep * hnsPerSecond);
        }

        pcmType.Dispose();
        //nativeType.Dispose();

        MediaFactory.MFShutdown();
    }

    protected override AudioBuffer GetSampleForSpeaker(Speakers speakerName)
    {
        if (_speakerDataStreams == null)
            throw new NullReferenceException(nameof(_speakerDataStreams));

        var dataStream = _speakerDataStreams[speakerName];
        return new(dataStream);
    }
}