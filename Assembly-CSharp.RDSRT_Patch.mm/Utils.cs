using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;

namespace RDSRT
{
    public static class Utils
    {
        public const float DONTE_CALIBRATION = 0.05f;

        public static string RECORD_FILE => Path.Combine(Application.persistentDataPath, "record.csv");

        public static int bool2int(bool b)
        {
            return b ? 1 : 0;
        }

        public static string GetCurrentSongName()
        {
            return (string)typeof(scrConductor).GetField("currentSongString", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(scrConductor.instance);
        }

        public static int InputBitfield_P1
        {
            get
            {
                return
                   (RDInput.p1Press ? (1 << 0) : 0) | // bit 0: p1Press
                   (RDInput.p1IsPressed ? (1 << 1) : 0) | // bit 1: p1IsPressed
                   (RDInput.p1Release ? (1 << 2) : 0); // bit 2: p1Release
                // further bits reserved (just in case arrow keys start becoming gameplay important or smth)
            }
        }

        public static int InputBitfield_P2
        {
            get
            {
                return
                   (RDInput.p2Press ? (1 << 0) : 0) | // bit 3: p2Press
                   (RDInput.p2IsPressed ? (1 << 1) : 0) | // bit 4: p2IsPressed
                   (RDInput.p2Release ? (1 << 2) : 0);  // bit 5: p2Release
            }
        }

        public static double CurrentDonteTime(bool isPlayerTwo)
        {
            float currentCalibration = isPlayerTwo ? RDCalibration.calibration_i_P2 : RDCalibration.calibration_i;
            float calibrationDifference = currentCalibration - DONTE_CALIBRATION;
            return scrConductor.instance.audioPos - scrConductor.instance.startOfSong - calibrationDifference;
        }

        // time: the timestamp -- MUST BE IN DONTETIME
        // player: 0 - player 1, 1 - player 2
        // inputs: input bitfield
        public static void WriteEvent(string currentSong, double time, int player, int inputs)
        {
            File.AppendAllText(RECORD_FILE, String.Format("{0},{1},{2},{3}\n", currentSong, time.ToString("R"), player, inputs));
        }

        public static List<InputRecord> loadInputRecordsFromFile()
        {
            List <InputRecord> final = new List<InputRecord>();
            string line;

            // Read the file and load in input events line-by-line.
            System.IO.StreamReader file =
                new System.IO.StreamReader(RECORD_FILE);
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
    }
}