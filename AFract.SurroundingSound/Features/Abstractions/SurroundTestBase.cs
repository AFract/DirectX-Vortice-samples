using AFract.SurroundingSound.Data;
using Vortice.Multimedia;

namespace AFract.SurroundingSound.Features.Abstractions;

public abstract class SurroundTestBase
{
    protected const int SourceAudioChannels = 1;
    protected const int DestinationAudioChannels = 6;

    public abstract void PlayTest();

    /// <summary>
    /// Listener au centre, orienté vers Z+
    /// </summary>
    /// <returns></returns>
    protected Listener CreateCenteredListener()
    {
        return new Listener
        {
            Position = Vector3.Zero,
            Velocity = Vector3.Zero,
            OrientFront = new Vector3(0f, 0f, 1f),   // regarde vers Z+
            OrientTop = new Vector3(0f, 1f, 0f),   // haut = Y+
        };
    }
}