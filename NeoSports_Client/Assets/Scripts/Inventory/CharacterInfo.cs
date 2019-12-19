using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Status
{
	public int strength;
	public int endurance;
	public int agility;
	public int luck;

	public Status()
	{

	}

	public Status(int strength, int endurance, int agility, int luck)
	{
		this.strength = strength;
		this.endurance = endurance;
		this.agility = agility;
		this.luck = luck;
	}

	public static void Add(out Status result, Status lValue, Status rValue)
	{
		result = new Status
		{
			strength = lValue.strength + rValue.strength,
			endurance = lValue.endurance + rValue.endurance,
			agility = lValue.agility + rValue.agility,
			luck = lValue.luck + rValue.luck
		};
		return;
	}
}

public class CharacterInfo
{

	public struct JsonData
	{
		public int charType;
		public string charName;
		public Status charStat;
		public string iconName;
		public string prefName;
	}

	const string CharacterPrefabPath = "Prefab/Character/";

	readonly int _charType;
	readonly string _charName;
	readonly Status _charStat;
	readonly Sprite _iconSprite;
	GameObject _prefObject;

	#region Property
	public int Type {
		get { return _charType; }
	}

	public string Name {
		get { return _charName; }
	}

	public Status Stat {
		get { return _charStat; }
	}

	public Sprite IconSprite {
		get { return _iconSprite; }
	}
	#endregion

	public CharacterInfo(JsonData data)
	{
		_charType = data.charType;
		_charName = data.charName;
		_charStat = data.charStat;
		_iconSprite = Singleton<ResourceManager>.Instance.GetUISprite(data.iconName);

		string prefabPath = CharacterPrefabPath + data.prefName;
		_prefObject = Resources.Load(prefabPath) as GameObject;
	}

	public GameObject GetCharacterPrefab()
	{
		return _prefObject;
	}

}
