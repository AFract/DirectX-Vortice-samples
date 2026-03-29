using AFract.SurroundingSound.Features.Abstractions;
using AFract.SurroundingSound.Helpers;
using Vortice.Multimedia;

namespace AFract.SurroundingSound.Features;

public class SineWaveSurroundTest(bool fixedFrequency) 
    : SurroundDiscreteSourcesTestBase
{
    private const float FixedFrequency = 440f;
    private const float FixedFrequencyLfe = 80f;
    private readonly (float start, float end) _variableFrequencyRange = (200f, 1000f);
    private readonly (float start, float end) _variableFrequencyRangeLfe = (20f, 140f);
    private const float Duration = 1f;       // secondes par haut-parleur

    private short[]? _pcmData, _pcmDataLfe;

    protected override void BuildSamples()
    {
        if (fixedFrequency)
        {
            _pcmData = SoundGenerator.GenerateSine(FixedFrequency, Duration);
            _pcmDataLfe = SoundGenerator.GenerateSine(FixedFrequencyLfe, Duration);
        }
        else
        {
            _pcmData = SoundGenerator.GenerateSineVariableFrequency(_variableFrequencyRange.start, _variableFrequencyRange.end, Duration);
            _pcmDataLfe = SoundGenerator.GenerateSineVariableFrequency(_variableFrequencyRangeLfe.start, _variableFrequencyRangeLfe.end, Duration * 2);
        }
    }

    protected override AudioBuffer GetSampleForSpeaker(Speakers speakerName)
    {
        var pcm = speakerName == Speakers.LowFrequency ? _pcmDataLfe : _pcmData;
        return AudioBufferTools.ToAudioBuffer(pcm ?? throw new Exception("Sample not initialized"));
    }
}