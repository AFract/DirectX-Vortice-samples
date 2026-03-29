using AFract.SurroundingSound.Features;
using AFract.SurroundingSound.Features.Abstractions;

namespace AFract.SurroundingSound;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Press '1' to start the 5.1 speaker test with pure sine at fixed frequency...");
        Console.WriteLine("Press '2' to start the 5.1 speaker test with pure sine at variable frequency...");
        Console.WriteLine("Press '3' to start the 5.1 speaker test with several voice files...");
        Console.WriteLine("Press '4' to start the 5.1 speaker test with one single splitted voice file...");
        Console.WriteLine("Press '5' to start the 5.1 speaker test with pure sine rotating around listener...");

        char[] allowed = ['1', '2', '3', '4', '5', 'Q', 'q'];

        ConsoleKeyInfo key;
        while ((key = Console.ReadKey()).Key != ConsoleKey.Q)
        {
            Console.WriteLine($"--User input: {key.Key}--");

            if (!allowed.Contains(key.KeyChar))
            {
                Console.WriteLine("Bad input");
                continue;
            }

            SurroundTestBase? surroundTest = null;

            if (key.KeyChar == '1')
            {
                surroundTest = new SineWaveSurroundTest(true);
            }
            else if (key.KeyChar == '2')
            {
                surroundTest = new SineWaveSurroundTest(false);
            }
            else if (key.KeyChar == '3')
            {
                surroundTest = new VoiceSeveralFilesSurroundTest();
            }
            else if (key.KeyChar == '4')
            {
                surroundTest = new VoiceSingleFileSurroundTest();
            }
            else if (key.KeyChar == '5')
            {
                surroundTest = new RotateAroundUserSurroundTest();
            }

            if (surroundTest == null)
            {
                throw new InvalidOperationException("Unexpected status");
            }

            surroundTest.PlayTest();
        }
    }
}