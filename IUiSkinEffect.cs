using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IUiSkinEffect
{
	void Apply( MaskableGraphic graphic );
}

public abstract class BaseUiSkinEffect<T> : IUiSkinEffect where T : BaseMeshEffect
{
	public void Apply( MaskableGraphic graphic )
	{
		var t = graphic.GetComponent<T>();
		if( t == null ) t = graphic.gameObject.AddComponent<T>();
		ApplyEffect( t );
	}
	
	public static void Destroy( MaskableGraphic graphic )
	{
		var s = graphic.GetComponent<T>();
		if( s != null )
		{
			Shadow.DestroyImmediate( s );
		}
	}
	
	protected abstract void ApplyEffect( T t );
}

[System.Serializable]
public class UiShadowEffect : BaseUiSkinEffect<Shadow>
{
    public static System.Type Component { get { return typeof( Shadow ); } }

	[SerializeField] Color _shadowColor = Color.black;
	[SerializeField] Vector2 _shadowDistance = new Vector2( -3, -1 );
	[SerializeField] bool _useGraphicAlpha = true;

	public Color Color  { get { return _shadowColor; } set { _shadowColor = value; } }
	public Vector2 Distance  { get { return _shadowDistance; } set { _shadowDistance = value; } }
	public bool UseGraphicAlpha { get { return _useGraphicAlpha; } set { _useGraphicAlpha = value; } }
	
	protected override void ApplyEffect( Shadow s )
	{
		s.effectDistance = _shadowDistance;
		s.effectColor = _shadowColor;
		s.useGraphicAlpha = _useGraphicAlpha;
	}
}

[System.Serializable]
public class UiOutlineEffect : BaseUiSkinEffect<NicerOutline>
{
    public static System.Type Component { get { return typeof( NicerOutline ); } }

	[SerializeField] Color _outlineColor = Color.white;
	[SerializeField] float _outlineSize = 2;
	[SerializeField] int _outlineInteractions = 8;
	[SerializeField] bool _useGraphicAlpha = true;
	
	public Color Color  { get { return _outlineColor; } set { _outlineColor = value; } }
	public float Size { get { return _outlineSize; } set { _outlineSize = value; } }
	public int Interactions { get { return _outlineInteractions; } set { _outlineInteractions = value; } }
	public bool UseGraphicAlpha { get { return _useGraphicAlpha; } set { _useGraphicAlpha = value; } }
	
	protected override void ApplyEffect( NicerOutline o )
	{
		_outlineSize = o.Size = _outlineSize;
		_outlineInteractions = o.Interactions = _outlineInteractions;
		o.EffectColor = _outlineColor;
        o.useGraphicAlpha = _useGraphicAlpha;
	}
}

[System.Serializable]
public class UiGradientEffect : BaseUiSkinEffect<Gradient>
{
    public static System.Type Component { get { return typeof( Gradient ); } }

	[SerializeField] GradientMode _mode = GradientMode.Local;
	[SerializeField] GradientDir _direction = GradientDir.Vertical;
	[SerializeField] bool _overwriteAllColors;
	[SerializeField] Color _color1 = Color.white;
	[SerializeField] Color _color2 = Color.gray;
	
	public GradientMode Mode { get { return _mode; } set { _mode = value; } }
	public GradientDir Direction { get { return _direction; } set { _direction = value; } }
	public Color Color1  { get { return _color1; } set { _color1 = value; } }
	public Color Color2  { get { return _color2; } set { _color2 = value; } }
	public bool OverwriteAllColors { get { return _overwriteAllColors; } set { _overwriteAllColors = value; } }
	
	protected override void ApplyEffect( Gradient g )
	{
		g.Mode = _mode;
		g.Direction = _direction;
		g.OverwriteAllColor = _overwriteAllColors;
		g.Color1 = _color1;
		g.Color2 = _color2;
	}
}