using System;
namespace RDSRT
{
    public class InputRecord
    {
        public string song;
        public double time;
        public int player;
        public int inputs;

        public InputRecord(string song, double time, int player, int inputs)
        {
            this.song = song;
            this.time = time;
            this.player = player;
            this.inputs = inputs;
        }

    }
}
