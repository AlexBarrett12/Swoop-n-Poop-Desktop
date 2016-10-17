using UnityEngine;
using System.Collections;

[System.Serializable]
public class StuffToSave{

	public static StuffToSave somethingToSave;

	public bool[] upgradeChecklist = new bool[5];
	public int hp = 10;

	public StuffToSave()
	{
		if(Player.saveStuff != null) {
			upgradeChecklist = Player.saveStuff.upgradeChecklist;
			hp = Player.saveStuff.hp;
		}
	}
}
