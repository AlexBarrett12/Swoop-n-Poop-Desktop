using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static bool paused;
	public static bool allowUnpause = true;

	public void pause()
	{
		if ((allowUnpause)) {
			paused = !paused;
		}
	}

	public void endLevel()
	{
		paused = true;
		allowUnpause = false;
	}

	void OnLevelWasLoaded()
	{
		paused = false;
		allowUnpause = true;
	}
}
