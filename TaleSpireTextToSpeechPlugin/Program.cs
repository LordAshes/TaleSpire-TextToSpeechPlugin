using BepInEx;
using System;
using TaleSpireUnofficalPlugins;

using TTS;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

using System.IO;
using System.Reflection;

namespace TextToSpeechPlugin
{
    [BepInPlugin(Guid, "Text To Speech Plug-In", Version)]
    [BepInDependency(TaleSpireUnofficalPlugins.InternetServerPlugin.Guid)]
    public class TextToSpeechPlugin : BaseUnityPlugin
    {
        private const string Guid = "org.lordashes.plugins.TTS";
        private const string Version = "1.0.0.0";

        // Content directory
        private static string dir = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.LastIndexOf("/")) + "/TaleSpire_CustomData/";

        // Internet Server client
        TaleSpireUnofficalPlugins.InternetServerPlugin.MessageClient client = new InternetServerPlugin.MessageClient();

        // Text to speech configuration
        private TTSConfiguration config = new TTSConfiguration();

        // ISP initialized
        private bool initializized = false;

        // Last picked up mini
        CreatureBoardAsset mini = null;

        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            UnityEngine.Debug.Log("Text To Speech Plugin");

            if(!System.IO.File.Exists(dir+"TTSCommandLine.exe"))
            {
                UnityEngine.Debug.Log("TextToSpeechPlugin: Missing mandatory 'TTSCommandLine.exe' in '" + dir + "'");
            }
            config.Deserialize(System.IO.File.ReadAllText(dir+"TextToSpeech.json"));
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
            // Ensure that there is a camera controller instance
            if (CameraController.HasInstance)
            {
                // Ensure that there is a board session manager instance
                if (BoardSessionManager.HasInstance)
                {
                    // Ensure that there is a board
                    if (BoardSessionManager.HasBoardAndIsInNominalState)
                    {
                        // Ensure that the board is not loading
                        if (!BoardSessionManager.IsLoading)
                        {
                            if(!initializized)
                            {
                                initializized = true;
                                string url = "http://talespire.mygamesonline.org/MessageServerSQL.php";
                                string session = CampaignSessionManager.Id + "." + BoardSessionManager.CurrentBoardInfo.Id + ".TTSPlugin";
                                string user = CampaignSessionManager.GetPlayerName(LocalPlayer.Id);
                                client.diagnosticMode = InternetServerPlugin.MessageClient.DiagnosticModes.off;
                                client.Connect(url, session, user, ReceivedMessages, true, false);
                            }

                            client.TryGetMessage();

                            CreatureMoveBoardTool moveBoard = SingletonBehaviour<BoardToolManager>.Instance.GetTool<CreatureMoveBoardTool>();
                            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
                            CreatureBoardAsset cba = (CreatureBoardAsset)typeof(CreatureMoveBoardTool).GetField("_pickupObject", flags).GetValue(moveBoard);
                            if (cba != null)
                            {
                                mini = cba;
                            }

                            foreach (KeyValuePair<string, KeyCode> trigger in config.configuration.triggers)
                            {
                                if (Input.GetKeyDown((KeyCode)trigger.Value))
                                {
                                    if (mini != null)
                                    {
                                        UnityEngine.Debug.Log("Creature '" + mini.Creature.Name + "' says quote for situation '" + trigger.Key + "'");
                                        SpeakTTS(mini, mini.Creature.Name, trigger.Key);
                                    }
                                    else
                                    {
                                        SystemMessage.DisplayInfoText("TTS Plugin Requires A Selected Mini");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to cause an asset to speak
        /// </summary>
        /// <param name="character">Name of the character speaking (used to look up voice and quotes in config)</param>
        /// <param name="situation">String situation used to look up appropriate quotes</param>
        /// <param name="quotation">Optional index indicating which quote to use. Default is -1 which means random choice</param>
        public void SpeakTTS(CreatureBoardAsset mini, string character, string situation, int quotation = -1)
        {
            if (config.configuration.characters.ContainsKey(character))
            {
                UnityEngine.Debug.Log("Found Character '"+character+"'. Getting Siutiational Quote...");
                if (config.configuration.characters[character].speeches.ContainsKey(situation))
                {
                    UnityEngine.Debug.Log("Found Situational Quote. Sending Queue Request...");
                    TTSMessage ttsm = config.configuration.characters[character].ChooseSpeech(situation, quotation);
                    client.QueueMessage(new InternetServerPlugin.NetworkMessage()
                    {
                        author = CampaignSessionManager.GetPlayerName(LocalPlayer.Id),
                        content = JsonConvert.SerializeObject(ttsm)
                    });
                }
                else
                {
                    UnityEngine.Debug.Log("Situaltion '" + situation + "' not found in configuration.");
                    foreach (string name in config.configuration.characters[character].speeches.Keys)
                    {
                        UnityEngine.Debug.Log("I know quotes for situation '" + name + "'.");
                    }
                }
            }
            else
            {
                UnityEngine.Debug.Log("Character '"+character+"' not found in configuration.");
                foreach(string name in config.configuration.characters.Keys)
                {
                    UnityEngine.Debug.Log("I know '" + name + "'.");
                }
            }
        }

        /// <summary>
        /// Method for receiving messages and initiating TTS
        /// </summary>
        /// <param name="messages"></param>
        private void ReceivedMessages(InternetServerPlugin.NetworkMessage[] messages)
        {
            foreach (InternetServerPlugin.NetworkMessage msg in messages)
            {
                // Speak in the specified voice
                TTSMessage tts = JsonConvert.DeserializeObject<TTSMessage>(msg.content);

                UnityEngine.Debug.Log("Request for "+tts.character+" saying '"+tts.quote+"'");

                using (System.Diagnostics.Process pSpeak = new System.Diagnostics.Process())
                {
                    pSpeak.StartInfo.FileName = dir+"TTSCommandLine.exe";
                    pSpeak.StartInfo.Arguments = "\"" + tts.character + "\" \"" + tts.quote + "\"";
                    pSpeak.StartInfo.WorkingDirectory = dir;
                    pSpeak.StartInfo.UseShellExecute = false;
                    pSpeak.StartInfo.RedirectStandardOutput = true;
                    pSpeak.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    pSpeak.StartInfo.CreateNoWindow = true;
                    UnityEngine.Debug.Log("Executing " + pSpeak.StartInfo.FileName + " " + pSpeak.StartInfo.Arguments);
                    pSpeak.Start();
                }

                foreach(CreatureBoardAsset asset in CreaturePresenter.AllCreatureAssets)
                {
                    if(asset.Creature.Name==tts.character)
                    {
                        asset.Creature.Speak(tts.quote);
                        break;
                    }
                }
            }
        }
    }
}
