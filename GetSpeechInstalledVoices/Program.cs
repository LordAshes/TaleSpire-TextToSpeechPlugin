using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetSpeechInstalledVoices
{
    class Program
    {
        static void Main(string[] args)
        {
            using (System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer())
            {
                foreach (System.Speech.Synthesis.InstalledVoice voice in synth.GetInstalledVoices())
                {
                    Console.WriteLine("[" + voice.VoiceInfo.Name + "]");
                    Console.WriteLine("  Description: " + voice.VoiceInfo.Description);
                    Console.WriteLine("  Gender:      " + voice.VoiceInfo.Gender);
                    Console.WriteLine("  Age:         " + voice.VoiceInfo.Age);
                    Console.WriteLine("  Culture:     " + voice.VoiceInfo.Culture);
                    Console.WriteLine();
                }
            }
        }
    }
}
