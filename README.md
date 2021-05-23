# TaleSpire-TextToSpeechPlugin
Provides keyboard initiated situational character specific quotues via displayed and TTS.

## Installation

The plugin ZIP file contains a folder called "TaleSpire_CustomData". This folder needs to be, manually, moved to the Talespire directory.
This folder does not exists as part of the base TaleSpire game. It is used to LordAshes plugins to keep custom configurations and support files
separate from the base game files. However, if you have other Lord Ashes plugins installed then this folder may already exits. In such a case
copy the contents into that folder. The end result should be something like:

\Steam\steamapps\common\TaleSpire\TaleSpire_CustomData\GetSpeechInstalledVoices.exe
\Steam\steamapps\common\TaleSpire\TaleSpire_CustomData\Newtonsoft.Json.dll
\Steam\steamapps\common\TaleSpire\TaleSpire_CustomData\TextToSpeech.json
\Steam\steamapps\common\TaleSpire\TaleSpire_CustomData\TTSCommandLine.exe

Due to issues integrating the Text To Speech functionality directly into the plugin, the plugin executes an external command (TTSCommandLine)
to perform the TTS functionality. The TTSCommandLine command takes two parameters, the character name and text to be spoken. If the text contains
spaces (which it likely will) it needs to be surrounded by quotes. For example:

TTSCommandLine Jon "Hello everyone, my name is Jon."

Feel free to use the command line application in other projects.

## Initial Setup

Edit the TextToSpeech.json found in the TaleSpire_CustomData folder. The first section of this file has a triggers section which relates situations
to a numeric value. The numeric value indicate which keyboard key triggers that particular situation. These values are integer values of the
corresponding Unity.KeyCode. The user can define as many situations with corresponding triggers as desired. Each "Situation" should have a corresponding
list of quotes in each of the character definitions. When the triggering key is pressed, the system will look up the name of the currently selected
mini and then choose a random quote from the quote list for that particular situation.

The next section of the file contains the definitions for each supported character, selecting a TTS voice, a TTS speak rate and listing one or more
quotes for each situation. This section of the file should be set up with a character for each mini that is going to make use of this feature
(typically for each player mini) and the list of quotes for each situation appropriate for that particular character.

The GetSpeechInstalledVoices application can be run to list all of the installed TTS voices. Use the name that appears between square brackets.

## Usage

When in TaleSpire, select the desired mini and then press the trigger key for one of the situations (e.g. the default configruation minds the "Ready"
situation to the 5 key and the "Attack" situation to the 6 key). The characters will display an appropriate quote in its speech bubble and make a
request, using the Internet Server Plugin, to all clients to play the corresponding Text-To-Speech message.
