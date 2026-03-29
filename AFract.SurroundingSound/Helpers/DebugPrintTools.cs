using Vortice.MediaFoundation;

namespace AFract.SurroundingSound.Helpers;

public static class DebugPrintTools
{
    public static void PrintDspSettings(DspSettings dspSettings)
    {
        var matrix = dspSettings.MatrixCoefficients;

        // Affichage des coefficients FL FR C LFE BL BR
        string[] chNames = { "FL", "FR", "C ", "LF", "BL", "BR" };
        for (var c = 0; c < 6; c++)
        {
            Console.Write($"  {chNames[c]}={matrix[c]:F3}");
        }
        Console.Write(", Doppler:" + dspSettings.DopplerFactor);

        Console.WriteLine();
    }

    public static void PrintMasterVoice(IXAudio2MasteringVoice masterVoice)
    {
        Console.WriteLine($"Channel mask: 0x{masterVoice.ChannelMask:X8}, " +
                          $"Voice details: ActiveFlags={masterVoice.VoiceDetails.ActiveFlags} / CreationFlags={masterVoice.VoiceDetails.CreationFlags} / InputChannels={masterVoice.VoiceDetails.InputChannels}  / InputSampleRate={masterVoice.VoiceDetails.InputSampleRate}");
    }

    #region SourceReader

    public static void SourceReaderDisplayMediaTypes(IMFSourceReader reader)
    {
        // DEBUG : lister tous les types disponibles
        Console.WriteLine($"Types list");

        int typeIndex = 0;
        while (true)
        {
            try
            {
                IMFMediaType? type = reader.GetNativeMediaType(SourceReaderIndex.FirstAudioStream, typeIndex);

                if (type == null)
                {
                    break;
                }

                type.GetGUID(MediaTypeAttributeKeys.Subtype, out Guid subtype);

                string name = GetAudioSubtypeName(subtype);

                Console.WriteLine($"\t- Type {typeIndex}: {subtype} ({name})");

                type.Dispose();

                typeIndex++;
            }
            catch (Exception ex) when (ex.HResult == unchecked((int)0x800704C7)) // MF_E_NO_ACCEPTABLE_TYPES
            {
                Console.WriteLine($"Fin des types à index {typeIndex}");
                break;
            }
            catch (Exception ex) when (ex.HResult == unchecked((int)0xC00D36B9)) // MF_E_NO_MORE_TYPES
            {
                Console.WriteLine($"Fin des types à index {typeIndex}");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                break;
            }
        }
    }

    public static string GetAudioSubtypeName(Guid audioFormatSubtype)
    {
        return audioFormatSubtype switch
        {
            var g when g == AudioFormatGuids.Pcm => nameof(AudioFormatGuids.Pcm),
            var g when g == AudioFormatGuids.Flac => nameof(AudioFormatGuids.Flac),
            var g when g == AudioFormatGuids.Mp3 => nameof(AudioFormatGuids.Mp3),
            var g when g == AudioFormatGuids.Aac => nameof(AudioFormatGuids.Aac),
            _ => "unknown (" + audioFormatSubtype.ToString() + ")"
        };
    }

    #endregion 
}