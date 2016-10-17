using UnityEngine;
using System.Collections;
[System.Serializable]
public class Game : MonoBehaviour {

	public static Game current;
	public Character Player;

	public Game () {
		Player = new Character();
	}

}
