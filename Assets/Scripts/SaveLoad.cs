using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoad{

	public static StuffToSave saveStuff = new StuffToSave();

	public static void Save ()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream stream = File.Create(Application.persistentDataPath+"/save.snp");
		bf.Serialize (stream, SaveLoad.saveStuff);
		stream.Close ();
		Debug.Log("Saved! Location: "+Application.persistentDataPath+"/save.snp");
	}

	public static StuffToSave Load()
	{
		if (File.Exists (Application.persistentDataPath + "/save.snp")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream stream = File.Open(Application.persistentDataPath+"/save.snp", FileMode.Open);
			SaveLoad.saveStuff = (StuffToSave)bf.Deserialize(stream);
			stream.Close ();
			Debug.Log("Loaded! Location: "+Application.persistentDataPath+"/save.snp");
			return SaveLoad.saveStuff;
		}
		Save();
		return Load();
	}
}
