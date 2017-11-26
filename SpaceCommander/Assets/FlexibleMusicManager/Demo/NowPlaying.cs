using UnityEngine;
using UnityEngine.UI;
using SpaceCommander.Service;
namespace SpaceCommander.Test
{
    public class NowPlaying : MonoBehaviour
    {

        public Text nowPlayingText;
        void Update()
        {

            if (MusicManager.instance.CurrentTrackNumber() >= 0)
            {
                string timeText = SecondsToMS(MusicManager.instance.TimeInSeconds());
                string lengthText = SecondsToMS(MusicManager.instance.LengthInSeconds());

                nowPlayingText.text = "" + (MusicManager.instance.CurrentTrackNumber() + 1) + ".  " +
                    MusicManager.instance.NowPlaying().name
                    + " (" + timeText + "/" + lengthText + ")";
            }
            else
            {
                nowPlayingText.text = "-----------------";
            }
        }

        string SecondsToMS(float seconds)
        {
            return string.Format("{0:D3}:{1:D2}", ((int)seconds) / 60, ((int)seconds) % 60);
        }
    }
}