using System.Diagnostics;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class OnlineTimer : NetworkBehaviour
{
    private Stopwatch stopwatch;

    // Start is called before the first frame update
    private void Awake()
    {
        stopwatch = new Stopwatch();
    }

    public override void OnStartClient()
    {
        stopwatch.Reset();
        stopwatch.Start();

        Debug.Log("Stopwatch started!");

        base.OnStartClient();
    }

    public void OnDisable()
    {
        if (stopwatch.IsRunning)
        {
            var ts = stopwatch.Elapsed;
            stopwatch.Stop();

            Debug.Log("Stopwatch stopped: duration " + string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10));
        }
    }

    private void OnGUI()
    {
        if (!stopwatch.IsRunning) return;

        GUI.Box(new Rect(new Vector2(2, Screen.height - 36), new Vector2(320, 32)), "ONLINE TIME: " + string.Format(
            "{0:00}:{1:00}:{2:00}.{3:00}",
            stopwatch.Elapsed.Hours,
            stopwatch.Elapsed.Minutes,
            stopwatch.Elapsed.Seconds,
            stopwatch.Elapsed.Milliseconds /
            10));
    }
}