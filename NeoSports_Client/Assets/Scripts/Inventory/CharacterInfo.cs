using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public struct Status
{
	public int strength;
	public int endurance;
	public int agility;
	public int luck;

	public Status(int strength, int endurance, int agility, int luck)
	{
		this.strength = strength;
		this.endurance = endurance;
		this.agility = agility;
		this.luck = luck;
	}

	public static void Add(out Status result, Status lValue, Status rValue)
	{
		result.strength = lValue.strength + rValue.strength;
		result.endurance = lValue.endurance + rValue.endurance;
		result.agility = lValue.agility + rValue.agility;
		result.luck = lValue.luck + rValue.luck;
		return;
	}
}

public class CharacterInfo
{

	public enum EType
	{
		Unknown = 100,
		PpiYaGi,
		TurkeyJelly,
		End
	}

	public struct JsonData
	{
		public EType charType;
		public string charName;
		public Status charStat;
		public string iconName;
	}

	EType _charType;
	string _charName;
	Status _charStat;
	Sprite _iconSprite;

	#region Property
	public EType Type {
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

	}

}
