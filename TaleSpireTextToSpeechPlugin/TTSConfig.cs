using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TTS
{
    [Serializable]
    public class TTSMessage
    {
        public string character { get; set; }
        public string quote { get; set; }
    }

    [Serializable]
    public class Character
    {
        public string name { get; set; }
        public string voice { get; set; }
        public int rate { get; set; } = 1;
        public Dictionary<string, List<string>> speeches = new Dictionary<string, List<string>>();

        public TTSMessage ChooseSpeech(string situation, int quotation = -1)
        {
            if (speeches.ContainsKey(situation))
            {
                List<string> quotes = speeches[situation];
                System.Random rnd = new System.Random();
                if (quotation == -1) { quotation = rnd.Next(0, quotes.Count); }
                return new TTSMessage()
                {
                    character = this.name,
                    quote = quotes.ElementAt(quotation)
                };
            }
            return new TTSMessage()
            {
                character = this.name,
                quote = "I am speechless in this situation."
            };
        }
    }

    [Serializable]
    public class TTSConfig
    {
        public Dictionary<string, Character> characters { get; set; } = new Dictionary<string, Character>();
        public Dictionary<string, KeyCode> triggers { get; set; } = new Dictionary<string, KeyCode>();
    }

    public class TTSConfiguration
    {
        public TTSConfig configuration { get; set; } = new TTSConfig();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(configuration, Formatting.Indented);
        }

        public void Deserialize(string json)
        {
            this.configuration = JsonConvert.DeserializeObject<TTSConfig>(json);
        }
    }
}
