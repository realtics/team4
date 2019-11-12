﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.U2D;

public class ResourceManager : Singleton<ResourceManager>
{

	public const string JsonDataPath = "JSons/";

	public SpriteAtlas uiAtlas;

	private void Awake()
	{
		DontDestroyOnLoad(this);
	}

	public string ReadJsonDataString(string dataPath)
	{
		string data;

		TextAsset ta = Resources.Load(string.Format("{0}", dataPath)) as TextAsset;
		data = ta.text;

		return data;
	}

	public Sprite GetUISprite(string name)
	{
		Sprite result;
		result = uiAtlas.GetSprite(name);
		return result;
	}

}
