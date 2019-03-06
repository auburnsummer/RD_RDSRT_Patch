using System;
using System.Collections;
using System.Collections.Generic;
using MonoMod;
using UnityEngine;

#pragma warning disable CS0626
namespace RDSRT
{
    [MonoModPatch("global::scrWrldGame")]
    public class patch_scrWrldGame : scrWrldGame
    {
        public RDSRT_Role CurrentRole;

        public List<InputRecord> inputRecords;
        public int currentRecordIndex;

        public string SongLastFrame;

        int P1BitfieldThisFrame;
        int P2BitfieldThisFrame;

        public extern void orig_Awake();
        public void Awake()
        {
            Application.runInBackground = true;
            inputRecords = new List<InputRecord>();
            CurrentRole = RDSRT_Role.Sender;
            orig_Awake();
        }

        public extern void orig_Update();
        public new void Update()
        {
            if (this.gameState == GameState.PreStart && Input.GetKeyDown(KeyCode.R))
            {
                CurrentRole = CurrentRole == RDSRT_Role.Sender ? RDSRT_Role.Receiver : RDSRT_Role.Sender;
                HUD.status = String.Format("Current Role: {0}", CurrentRole.ToString());
            }
            if (CurrentRole == RDSRT_Role.Sender)
            {
                Utils.CurrentTimePassed(); // to set the time at the start of the frame.
                // Compare last inputs to these inputs, if they've changed write to the file.
                P1BitfieldThisFrame = Utils.InputBitfield_P1;
                P2BitfieldThisFrame = Utils.InputBitfield_P2;

                // pretty sure this is how we avoid reading cutscene inputs, etc.
                if (P1BitfieldThisFrame > 0 && this.receptive && !this.paused)
                {
                    Utils.WriteEvent(Utils.GetCurrentSongName(), Utils.CurrentTimePassed(), 0, P1BitfieldThisFrame);
                }
            }

            orig_Update();
        }

        public void LateUpdate()
        {

            Utils.FakeNews = false;
            if (CurrentRole == RDSRT_Role.Receiver)
            {
                // if there are input records in the first place...
                if (inputRecords.Count > 0 && currentRecordIndex < inputRecords.Count)
                {
                    // if we should play this record...
                    if (currentRecordIndex < inputRecords.Count && Utils.ShouldWePlayThisInputRecordNow(inputRecords[currentRecordIndex], SongLastFrame))
                    {
                        InputRecord currentRecord = inputRecords[currentRecordIndex];
                        // set the fake news
                        Utils.FakeNewsValue = currentRecord.time + scrConductor.instance.startOfSong;
                        Debug.Log(String.Format("[FakeNews] The time is {0}, instead of {1}. frame {2}", Utils.FakeNewsValue - scrConductor.instance.startOfSong, Utils.CurrentTimePassed(), Time.frameCount));
                        Utils.FakeNews = true;
                        // play the record
                        Utils.PlayInputRecord(currentRecord);
                        Debug.Log(String.Format(
                            "[InputRecord] Playing {0} InputRecord {1} at time {2}, OT {3}, error {4}, DT {5}, frame {6}",
                            currentRecord.player == 1 ? "P2" : "P1",
                            currentRecord.inputs,
                            Utils.CurrentTimePassed(),
                            currentRecord.time,
                            Math.Abs(Utils.CurrentTimePassed() - currentRecord.time),
                            Time.deltaTime,
                            Utils.DeltaTimeNoScale
                            ));
                        currentRecordIndex++;
                    }
                }
                SongLastFrame = Utils.GetCurrentSongName();

            }
        }

        public extern IEnumerator orig_StartTheGame(float speed = 1f, string customMessage = "");
        public new IEnumerator StartTheGame(float speed = 1f, string customMessage = "")
        {
            Debug.Log("[RDSRT] LET US START THE GAME");
            Debug.Log(String.Format("The Role is {0}", CurrentRole.ToString()));
            if (CurrentRole == RDSRT_Role.Sender)
            {
                Debug.Log("[RDSRT] We're in the role of a Sender");
                Utils.ClearInputFile();
            }
            else if (CurrentRole == RDSRT_Role.Receiver)
            {
                Debug.Log("[RDSRT] We're in the role of a Receiver");
                inputRecords = Utils.loadInputRecordsFromFile();
                currentRecordIndex = 0;
            }
            return orig_StartTheGame();
        }
    }
}
