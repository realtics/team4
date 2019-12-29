﻿using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;

public class BundleManager : Singleton<BundleManager>
{

	// 번들 다운 받을 서버의 주소(필자는 임시방편으로 로컬 파일 경로 쓸 것임)
	public const string FarmBundleURI = "https://ucbac4676e7c404ac2908e1137be.dl.dropboxusercontent.com/cd/0/get/AvIf5vBp5tf6ccHbpLIXByv7fZUVWSd56UrFBtGAR8AI8aZad-oI7OmQwIuKaGpKRjJl4B70VRjDyakFtuV6ylI4DlVSAqE61n752TovvroKGb8fsWFLkJIodS0dOXFh02Q/file#";
	public const string CharEquipBundleURI = "https://uc98052e7e743de2b49716d8895c.dl.dropboxusercontent.com/cd/0/get/AvKyc5-8uc6kr9smntCISaItdYHuBafcNbiE-OBsnxJkHlDX2QfrhRtmb-cCP2-yMLwiBkvLvvcainLcBcAOprtMPhGf2SFkS4VWiKp_xqxV30NNVx_ESOprBc_w5bgLEws/file#";


	AssetBundle _farmAssetBundle;
	AssetBundle _charEquipBundle;

	bool _farmLoadComplete = false;
	bool _charEquipLoadComplete = false;

	#region Property
	public bool IsLoadComplete { get; private set; } = false;
	#endregion

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(this);
		}

		DontDestroyOnLoad(this);
	}

	public void Init()
	{
		StartCoroutine(DownloadAndCache());
	}

	IEnumerator DownloadAndCache()
	{
		// cache 폴더에 AssetBundle을 담을 것이므로 캐싱시스템이 준비 될 때까지 기다림 
		while (!Caching.ready)
		{
			yield return null;
		}

		// 에셋번들을 캐시에 있으면 로드하고, 없으면 다운로드하여 캐시폴더에 저장합니다.
		using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(FarmBundleURI))
		{
			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError(uwr.error);
			}
			else
			{
				// Get downloaded asset bundle
				_farmAssetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
				_farmLoadComplete = true;
			}
		}
		// using문은 File 및 Font 처럼 컴퓨터 에서 관리되는 리소스들을 쓰고 나서 쉽게 자원을 되돌려줄수 있도록 기능을 제공

		using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(CharEquipBundleURI))
		{
			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError(uwr.error);
			}
			else
			{
				// Get downloaded asset bundle
				_charEquipBundle = DownloadHandlerAssetBundle.GetContent(uwr);
				_charEquipLoadComplete = true;
			}
		}

		if(_farmLoadComplete && _charEquipLoadComplete)
		{
			IsLoadComplete = true;
		}
	}

	public Object GetFarmBundleObject(string objectName)
	{
		return _farmAssetBundle.LoadAsset(objectName);
	}

	public Object GetCharEquipBundleObject(string objectName)
	{
		return _charEquipBundle.LoadAsset(objectName);
	}
}
