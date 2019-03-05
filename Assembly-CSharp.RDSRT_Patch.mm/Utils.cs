using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;

namespace RDSRT
{
    public static class Utils
    {
        public static string RECORD_FILE => Path.Combine(Application.persistentDataPath, "record.csv");

        public static bool FakeNews = false;
        public static double FakeNewsValue;

        public static int bool2int(bool b)
        {
            return b ? 1 : 0;
        }

        public static string GetCurrentSongName()
        {
            //
            return (string)typeof(scrConductor).GetField("currentSongString", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(scrConductor.instance);
        }

        public static int InputBitfield_P1
        {
            get
            {
                return
                   (RDInput.p1Press ? (1 << 0) : 0) | // bit 0: p1Press
                   (RDInput.p1Release ? (1 << 1) : 0); // bit 1: p1Release
                // further bits reserved (just in case arrow keys start becoming gameplay important or smth)
            }
        }

        public static int InputBitfield_P2
        {
            get
            {
                return
                   (RDInput.p2Press ? (1 << 0) : 0) | // bit 0: p2Press
                   (RDInput.p2Release ? (1 << 1) : 0);  // bit 1: p2Release
            }
        }

        public static double CurrentTimePassed()
        {
            return scrConductor.instance.audioPos - scrConductor.instance.startOfSong;
        }

        // time: the timestamp
        // player: 0 - player 1, 1 - player 2
        // inputs: input bitfield
        public static void WriteEvent(string currentSong, double time, int player, int inputs)
        {
            File.AppendAllText(RECORD_FILE, String.Format("{0},{1},{2},{3}\n", currentSong, time.ToString("R"), player, inputs));
        }

        public static void ClearInputFile()
        {
            System.IO.File.WriteAllText(RECORD_FILE, string.Empty);
        }

        public static List<InputRecord> loadInputRecordsFromFile()
        {
            List <InputRecord> final = new List<InputRecord>();
            string line;

            // Read the file and load in input events line-by-line.
            System.IO.StreamReader file = new System.IO.StreamReader(RECORD_FILE);
            while ((line = file.ReadLine()) != null)
            {
                string[] tokens = line.Split(',');
                InputRecord newRecord = new InputRecord(
                    tokens[0],
                    Double.Parse(tokens[1]), 
                    int.Parse(tokens[2]),
                    int.Parse(tokens[3])
                );
                final.Add(newRecord);

            }

            file.Close();


            return final;
        }

        public static void PlayInputRecord(InputRecord record)
        {
            RDInput.PlayerEmu keyToPress = ((record.inputs >> 1 & 1) == 1) ? RDInput.PlayerEmu.Up : RDInput.PlayerEmu.Down;
            // player 1
            if (record.player == 0)
            {
                RDInput.SetP1Emu(keyToPress);
            }
            // player 2
            else
            {
                RDInput.SetP2Emu(keyToPress);
            }
        }

        public static bool ShouldWePlayThisInputRecordNow(InputRecord record, string songLastFrame)
        {
            string songThisFrame = GetCurrentSongName();

            // Case: the song in the frame we're currently in isn't the song that the record states!
            if (songThisFrame != record.song)
            {
                // there's two different scenarios here:
                // Case 1: the previous frame's song is different from this frame. this means the song change occured
                // at some point between this frame and the previous frame, so the hit can only ever be less than one deltaTime
                // away from our current position. therefore, we should just hit it now.
                if (songLastFrame != songThisFrame)
                {
                    return true;
                }
                // OTHERWISE: the previous frame's song is the same as this one, so only the record has changed songs
                // but we're still in the previous song, it's just the user didn't do any input until the song change.
                return false;
            }
            // Otherwise, we're in the same song as the record, so we can rely on the time values.
            // realistically, the different songs case shouldn't occur too often - i don't think there's many levels that have
            // gameplay-important segmenets next to a song change.
            else
            {
                // Something like: hit the beat if it's close to us, or if we've already past it
                if (record.time < (CurrentTimePassed() + Time.deltaTime))
                {
                    return true;
                }
                return false;
            }
        }
    }
}