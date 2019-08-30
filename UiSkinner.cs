using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu( "GUISkinner/Skinner" )]
public class UiSkinner : AttachedComponent<MaskableGraphic>
{
	[SerializeField] string _skinCode;
    UiSkinDefinition _skin;

    public string SkinCode { get { return _skinCode; } set { _skinCode = value; } }
	
	public void Update()
	{
		var skin = UiSkinManager.GetDefinition( _skinCode );

        if( !( skin == null && _skin == null ) && ( ( skin == null ^ _skin == null ) || skin != _skin ) )
        {
            Unregister();
            _skin = skin;
            if( _skin != null )
            {
                _skin.Apply( Attached );
                _skin.OnChange.Register( OnChange );
            }
        }
	}

    public void ApplyCachedSkin()
    {
        if( _skin != null )
            _skin.Apply( Attached );
    }

    void OnChange( UiSkinDefinition skin )
    {
        skin.Apply( Attached );
    }

    void Unregister()
    {
        if( _skin != null )
        {
            _skin.OnChange.Unregister( OnChange );
        }
    }

    void OnDestroy() { Unregister(); }
}