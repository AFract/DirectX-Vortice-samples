using System.IO;
using AFract.SurroundingSound.Features.Abstractions;
using Vortice;
using Vortice.Multimedia;

namespace AFract.SurroundingSound.Features;

public class VoiceSeveralFilesSurroundTest()
    : SurroundDiscreteSourcesTestBase
{
    private readonly (Speakers speaker, string filename)[]? _speakerFiles =
        [
            (Speakers.FrontLeft, filename: "Assets\\FrontLeft.wav"),
            (Speakers.FrontRight, filename: "Assets\\FrontRight.wav"),
            (Speakers.FrontCenter, filename: "Assets\\Center.wav"),
            (Speakers.LowFrequency, filename: "Assets\\LFE_lowpass.wav"),
            (Speakers.BackLeft, filename: "Assets\\BackLeft.wav"),
            (Speakers.BackRight, filename: "Assets\\BackRight.wav"),
        ];
    private readonly Dictionary<Speakers, DataStream>? _speakerDataStreams = new();

    protected override void BuildSamples()
    {
        if (_speakerFiles == null)
            throw new NullReferenceException(nameof(_speakerFiles));
        if (_speakerDataStreams == null)
            throw new NullReferenceException(nameof(_speakerDataStreams));

        foreach (var speakerSample in _speakerFiles)
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes(speakerSample.filename));
            using SoundStream soundStream = new(stream);
            _speakerDataStreams.Add(speakerSample.speaker, soundStream.ToDataStream());
        }
    }

    protected override AudioBuffer GetSampleForSpeaker(Speakers speakerName)
    {
        if (_speakerFiles == null)
            throw new NullReferenceException(nameof(_speakerFiles));
        if (_speakerDataStreams == null)
            throw new NullReferenceException(nameof(_speakerDataStreams));

        return new(_speakerDataStreams[speakerName]);
    }
}