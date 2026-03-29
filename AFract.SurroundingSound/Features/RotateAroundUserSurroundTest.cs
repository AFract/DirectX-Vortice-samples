using AFract.SurroundingSound.Data;
using AFract.SurroundingSound.Features.Abstractions;
using AFract.SurroundingSound.Helpers;
using Vortice.Multimedia;

namespace AFract.SurroundingSound.Features;

public class RotateAroundUserSurroundTest : SurroundTestBase
{
    private readonly double _rotationSpeedSeconds  = 3;

    public override void PlayTest()
    {
        using var xaudio = XAudio2.XAudio2Create();

        using var masterVoice = xaudio.CreateMasteringVoice(
            inputChannels: 6,
            inputSampleRate: SoundFormatConstants.DefaultSampleRate
        );

        DebugPrintTools.PrintMasterVoice(masterVoice);

        var x3dAudio = new X3DAudio(SpeakersDefinition.FivePointOneSpeakers);

        var wfx = new WaveFormat(SoundFormatConstants.DefaultSampleRate, SoundFormatConstants.DefaultBitsPerSample, SourceAudioChannels);

        using var sourceVoice = xaudio.CreateSourceVoice(wfx);

        short[] samples = SoundGenerator.GenerateSine(440, 60);

        var audioBuffer = AudioBufferTools.ToAudioBuffer(samples);

        var listener = CreateCenteredListener();

        // Slighly move the listener on back, to allow doppler to work
        listener.Position = new Vector3(0, 0f, -1f); 

        var emitter = new Emitter
        {
            Position = new Vector3(1, 0, 0), // start at front right
            Velocity = Vector3.Zero,
            OrientFront = new Vector3(0, 0, 1),
            OrientTop = new Vector3(0, 1, 0),
            ChannelCount = 1,
            CurveDistanceScaler = 1.0f,
            DopplerScaler = 1.0f
        };

        var dspSettings = new DspSettings(sourceChannelCount: 1, destinationChannelCount: 6);

        // rayon = 3m ; vitesse angulaire ~ 1 tour toutes les 3 secondes
        float radius = 3.0f;
        float angularSpeed = (float)(2.0 * Math.PI / _rotationSpeedSeconds); // rad/s

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Envoyer le buffer, démarrer la lecture
        sourceVoice.SubmitSourceBuffer(audioBuffer);
        sourceVoice.SetOutputMatrix(masterVoice, SourceAudioChannels, DestinationAudioChannels, dspSettings.MatrixCoefficients);

        sourceVoice.Start();

        while (sourceVoice.State.BuffersQueued > 0)
        {
            double t = stopwatch.Elapsed.TotalSeconds;

            // Angle courant
            float angle = (float)(angularSpeed * t);

            // Position circulaire (x,z)
            float x = radius * (float)Math.Cos(angle);
            float z = radius * (float)Math.Sin(angle);

            emitter.Position = new Vector3(x, 0, z);

            // Optionnel : speed for Doppler effect
            float vx = -radius * angularSpeed * (float)Math.Sin(angle);
            float vz = radius * angularSpeed * (float)Math.Cos(angle);
            emitter.Velocity = new Vector3(vx, 0, vz);
            //emitter.OrientFront = Vector3.Normalize(new Vector3(-vx, 0, -vz)); // tangente → pointe vers le centre
            emitter.OrientFront = Vector3.Normalize(listener.Position - emitter.Position);

            x3dAudio.Calculate(
                listener,
                emitter,
                CalculateFlags.Matrix | CalculateFlags.Doppler,
                dspSettings
            );

            DebugPrintTools.PrintDspSettings(dspSettings);

            sourceVoice.SetOutputMatrix(masterVoice, SourceAudioChannels, DestinationAudioChannels, dspSettings.MatrixCoefficients);
            
            // Apply doppler effect. Note: it won't do anything audible if the listener is centered.
            sourceVoice.SetFrequencyRatio(dspSettings.DopplerFactor, 0);

            // Sleep to avoid useless CPU loading (16ms -> 60 Hz)
            Thread.Sleep(16);
        }

        // Nettoyage
        sourceVoice.Stop();
    }
}