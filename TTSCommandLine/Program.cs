using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TTSCommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Count()!=2)
            {
                Console.WriteLine("Error: Incorrect Number Of Parameters\r\n");
                Console.WriteLine("\r\n");
                Console.WriteLine("Syntax:\r\n");
                Console.WriteLine("\r\n");
                Console.WriteLine(" TTSCommandLine \"Character\" \"Quote\"\r\n");
            }
            else
            {
                TTS.TTSConfiguration config = new TTS.TTSConfiguration();
                config.Deserialize(System.IO.File.ReadAllText("TextToSpeech.json"));

                if (config.configuration.characters.ContainsKey(args[0]))
                {
                    System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer();
                    try
                    {
                        synth.SelectVoice(config.configuration.characters[args[0]].voice);
                    }
                    catch (Exception)
                    {
                        synth.SelectVoice(synth.GetInstalledVoices()[0].VoiceInfo.Name);
                    }
                    synth.Speak(args[1]);
                }
            }
        }
    }
}
