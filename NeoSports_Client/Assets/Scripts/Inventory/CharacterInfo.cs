using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public struct Status
{
	public enum EType
	{
		Strength,
		Endurance,
		Agility,
		Luck,
		End
	}

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

	public static void Add(Status lValue, Status rValue, out Status result)
	{
		result.strength = lValue.strength + rValue.strength;
		result.endurance = lValue.endurance + rValue.endurance;
		result.agility = lValue.agility + rValue.agility;
		result.luck = lValue.luck + rValue.luck;
		return;
	}


	/// <summary>
	/// Return Stat Value To Array, Using With EType(enum)
	/// </summary>
	/// <returns>Status Array</returns>
	public int[] GetValueAsArray()
	{
		int[] value = new int[(int)EType.End];
		value[(int)EType.Strength] = strength;
		value[(int)EType.Endurance] = endurance;
		value[(int)EType.Agility] = agility;
		value[(int)EType.Luck] = luck;

		return value;
	}
}

public class CharacterInfo
{

	public enum EType
	{
		Unknown,
		PpiYaGi,
		End
	}

	public struct JsonData
	{
		public EType	charType;
		public string	charName;
		public Status	charStat;
		public string	iconName;
	}

	EType	charType;
	string	charName;
	Status	charStat;
	Sprite	iconSprite;

	#region Property
	public EType Type {
		get { return charType; }
	}

	public string Name {
		get { return charName; }
	}

	public Status Stat {
		get { return charStat; }
	}

	public Sprite IconSprite {
		get { return iconSprite; }
	}
	#endregion

	public CharacterInfo(JsonData data)
	{
		charType = data.charType;
		charName = data.charName;
		charStat = data.charStat;
		iconSprite = Singleton<ResourceManager>.Instance.GetUISprite(data.iconName);

	}

}
