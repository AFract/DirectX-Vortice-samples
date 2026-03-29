using AFract.SurroundingSound.Helpers;
using Vortice.Multimedia;

namespace AFract.SurroundingSound.Data;

public class SpeakersDefinition
{
    public const Speakers FivePointOneSpeakers = (Speakers)0x0000003F;

    // ─── Positions 5.1 des haut-parleurs (repère auditeur au centre) ──────────────
    // Convention : X = droite, Y = haut, Z = avant (vers l'auditeur)
    // Distance arbitraire de 3 mètres

    public static readonly Speaker[] SpeakerPositions =
    [
        new (Speakers.FrontLeft,   position: new Vector3(-2.1f,  0f,  2.1f)),   // FL  — avant gauche 45°
        new(Speakers.FrontRight,   position: new Vector3( 2.1f,  0f,  2.1f)),   // FR  — avant droit 45°
        new(Speakers.FrontCenter,  position: new Vector3( 0f,    0f,  3.0f)),   // FC  — avant centre
        new(Speakers.LowFrequency, position: new Vector3( 0f,   -0.5f, 1.0f)),  // LFE — sol/centre
        new(Speakers.BackLeft,     position: new Vector3(-2.1f,  0f, -2.1f)),   // BL  — arrière gauche
        new(Speakers.BackRight,    position: new Vector3( 2.1f,  0f, -2.1f)) // BR  — arrière droit
    ];
}