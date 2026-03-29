using Vortice.Multimedia;

namespace AFract.SurroundingSound.Helpers;

public class Speaker
{
    public Speaker(Speakers name, Vector3 position)
    {
        Name = name;
        Position = position;
    }

    public Speakers Name { get; }
    public Vector3 Position { get; }
}