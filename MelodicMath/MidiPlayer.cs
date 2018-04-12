using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MonoTouch;
using AudioToolbox;
using AudioUnit;
using CoreFoundation;
using CoreMidi;
using Foundation;
using UIKit;

using System.Collections;

namespace MelodicMath
{
    /// <summary>
    ///    MIDI controller class
    /// </summary>
    public class MidiPlayer
    {
        AUGraph graph;
        AudioUnit.AudioUnit samplerUnit;

        Hashtable noteValues = new Hashtable();
        Hashtable noteTimes = new Hashtable();
        Hashtable instruments = new Hashtable();


        int sendMidiStop = 0;

        int lowerOctave = 0;//some instruments like bass need lowered

        public MidiPlayer() 
        {
            //set up 12 notes around mid C
            //add/subtract 12 to go up/down to next octave
            noteValues.Add("C", 60);
            noteValues.Add("C#", 61);
            noteValues.Add("Db", 61);
            noteValues.Add("D", 62);
            noteValues.Add("D#", 63);
            noteValues.Add("Eb", 63);
            noteValues.Add("E", 64);
            noteValues.Add("F", 65);
            noteValues.Add("F#", 66);
            noteValues.Add("Gb", 66);
            noteValues.Add("G", 67);
            noteValues.Add("G#", 68);
            noteValues.Add("Ab", 68);
            noteValues.Add("A", 69);
            noteValues.Add("A#", 70);
            noteValues.Add("Bb", 70);
            noteValues.Add("B", 71);
            noteValues.Add("R", 0); //this is for rests

            //add note times, this is how long each note would last
            //in a 60bpm tempo in 4/4 time
            noteTimes.Add(".25", 250);
            noteTimes.Add(".5", 500);
            noteTimes.Add("1", 1000);
            noteTimes.Add("2", 2000);
            noteTimes.Add("4", 4000);

            instruments.Add("piano", "Rhodes EPs Plus-JN1.5");
            instruments.Add("voice", "KBH-Real-Choir-V2.5");
            instruments.Add("guitar", "Electric-Guitars-JNv4.4");
            instruments.Add("bass", "Nice-4-Bass-V1.5");


            //create this on start up, or when they switch instrument
            createAudioProcessGraph();
            loadVirtualInstrument("Rhodes EPs Plus-JN1.5",1);//default to piano
        }

        //melody is a string made up of all the notes, it also contains note length and octave information
        public async void play(string melody, int tempo)
        {

            //createAudioProcessGraph();

            //loadVirtualInstrument(1);

            await playMelody(melody, tempo);
        }

        void createAudioProcessGraph()
        {
            graph = new AUGraph();

            var sampler = graph.AddNode(AudioComponentDescription.CreateMusicDevice(AudioTypeMusicDevice.Sampler));
            var inout = graph.AddNode(AudioComponentDescription.CreateOutput(AudioTypeOutput.Remote));

            graph.Open();
            graph.ConnnectNodeInput(sampler, 0, inout, 0);

            samplerUnit = graph.GetNodeInfo(sampler);

            graph.Initialize();
            graph.Start();
        }

        public void SetInstrument(string instrument)
        {
            loadVirtualInstrument((string)instruments[instrument], 1);
            if(instrument.Equals("piano") || instrument.Equals("guitar"))
            {
                sendMidiStop = 0;
            }
            if(instrument.Equals("voice"))
            {
                sendMidiStop = 1;
            }

            if(instrument.Equals("bass"))
            {
                lowerOctave = 12;
            }
            else
            {
                lowerOctave = 0;
            }
        }

        void loadVirtualInstrument(string instrument, int preset)
        {
            //var soundFontPath = NSBundle.MainBundle.PathForResource("Rhodes EPs Plus-JN1.5", "sf2");
            //var soundFontPath = NSBundle.MainBundle.PathForResource("KBH-Real-Choir-V2.5", "sf2");
            var soundFontPath = NSBundle.MainBundle.PathForResource(instrument, "sf2");

            var soundFontUrl = CFUrl.FromFile(soundFontPath);

            samplerUnit.LoadInstrument(new SamplerInstrumentData(soundFontUrl, InstrumentType.SF2Preset)
            {
                BankLSB = SamplerInstrumentData.DefaultBankLSB,
                BankMSB = SamplerInstrumentData.DefaultMelodicBankMSB,
                PresetID = (byte)preset,
            });
        }

        async Task playMelody(string melody, int tempo)
        {
            double time = 60;
            string[] splitMelody = { "," };
            string[] melodyInfo = melody.Split(splitMelody, StringSplitOptions.RemoveEmptyEntries);

            foreach(string noteInfo in melodyInfo)
            {
                string[] splitNote = { "|" };
                string[] note = noteInfo.Split(splitNote, StringSplitOptions.RemoveEmptyEntries);
                int noteVal = (int)noteValues[note[0]] +(Convert.ToInt32(note[2]) * 12);
                int dur = Convert.ToInt32((int)noteTimes[note[1]] * (time/tempo));

                if (noteVal == 0)
                    await playRest(dur);
                else
                    await playNote(noteVal - lowerOctave, dur, 127);

            }

            //don't need to feed all notes, above seem to work fast enough
            /*await playNotes(new[] {
                tone,
            }, duration, velocity);*/

        }

        async Task playNote(int note, int duration, double velocity = 127)
        {
            var channel = 0;
            var status = (9 << 4) | channel;
            samplerUnit.MusicDeviceMIDIEvent((byte)status, (byte)note, (byte)velocity);
            await Task.Delay(duration);

            //this would be called to stop a MIDI note from playing
            if (sendMidiStop == 1)
            {
                status = 0x80 | channel;
                samplerUnit.MusicDeviceMIDIEvent((byte)status, (byte)note, (byte)velocity);
            }

        }

        async Task playRest(int duration)
        {
            await Task.Delay(duration);
        }

    }
}
