using AFract.SurroundingSound.Data;
using AFract.SurroundingSound.Helpers;
using Vortice.Multimedia;

namespace AFract.SurroundingSound.Features.Abstractions;

public abstract class SurroundDiscreteSourcesTestBase 
    : SurroundTestBase
{
    public override void PlayTest()
    {
        // ─── Initialisation XAudio2 ──────────────────────────────────────────────────
        using var xaudio = XAudio2.XAudio2Create();
        xaudio.StartEngine();

        // Mastering voice en 6 canaux (5.1) à 48 kHz
        using var masterVoice = xaudio.CreateMasteringVoice(
            inputChannels: 6,
            inputSampleRate: SoundFormatConstants.DefaultSampleRate
        );

        // Print XAudio details
        DebugPrintTools.PrintMasterVoice(masterVoice);

        var x3dAudio = new X3DAudio(SpeakersDefinition.FivePointOneSpeakers);

        var listener = CreateCenteredListener();

        // ─── Format source (mono 16-bit, 48kHz) ─────────────────────────────────────────────
        var wfx = new WaveFormat(SoundFormatConstants.DefaultSampleRate, SoundFormatConstants.DefaultBitsPerSample, SourceAudioChannels);

        BuildSamples();

        // ─── Jouer sur chaque haut-parleur via X3DAudio ──────────────────────────────
        foreach (var speaker in SpeakersDefinition.SpeakerPositions)
        {
            Console.WriteLine($"- {speaker.Name}  @ position {speaker.Position}");

            // Emitter positionné sur le haut-parleur cible
            var emitter = new Emitter
            {
                Position = speaker.Position,
                Velocity = Vector3.Zero,
                OrientFront = Vector3.Normalize(-speaker.Position), // pointe vers l'auditeur
                OrientTop = new Vector3(0f, 1f, 0f),
                ChannelCount = 1,
                CurveDistanceScaler = 1.0f,
                DopplerScaler = 1.0f,
                InnerRadius = 0f,
                InnerRadiusAngle = 0f,
            };

            var dspSettings = new DspSettings(sourceChannelCount: 1, destinationChannelCount: 6);

            x3dAudio.Calculate(
                listener,
                emitter,
                CalculateFlags.Matrix,
                dspSettings
            );

            DebugPrintTools.PrintDspSettings(dspSettings);

            using var sourceVoice = xaudio.CreateSourceVoice(wfx);

            // Appliquer la matrice X3DAudio à la source voice
            sourceVoice.SetOutputMatrix(masterVoice, 1, 6, dspSettings.MatrixCoefficients);

            // Soumettre le buffer PCM correspondant (LFE ou non selon le haut-parleur)
            var buffer = GetSampleForSpeaker(speaker.Name);
            sourceVoice.SubmitSourceBuffer(buffer);

            sourceVoice.Start();

            // Attendre la fin de la lecture
            while (true)
            {
                var state = sourceVoice.State;

                if (state.BuffersQueued == 0)
                {
                    break;
                }

                Thread.Sleep(50);
            }

            sourceVoice.Stop();
            sourceVoice.DestroyVoice();

            Thread.Sleep(100); // pause between each speaker
        }

        Console.WriteLine("End");
    }

    protected abstract AudioBuffer GetSampleForSpeaker(Speakers speakerName);

    protected abstract void BuildSamples();
}