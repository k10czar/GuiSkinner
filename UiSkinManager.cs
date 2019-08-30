using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu( "GUISkinner/SkinManager" )]
public class UiSkinManager : MonoBehaviour
{
	static UiSkinManager Instance { get { return Guaranteed<UiSkinManager>.Instance; } }

	[SerializeField] List<UiSkinDefinition> _colorDefinitions = new List<UiSkinDefinition>();

	public List<UiSkinDefinition> SkinDefinitions { get { return _colorDefinitions; } }

	UiSkinDefinition GetUiDefinition( string code )
	{
		for( int i = 0; i < _colorDefinitions.Count; i++ )
		{
			var def = _colorDefinitions[i];
			if( def.Code == code ) { return def; }
		}

		return null;
	}
	
	public static UiSkinDefinition GetDefinition( string code ) { return Instance.GetUiDefinition( code ); }
}
