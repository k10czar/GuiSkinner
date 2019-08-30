using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class UiSkinDefinition
{
	[SerializeField] string _code;
	[SerializeField] Color _color = Color.white;
    [SerializeField] Font _font;

	[SerializeField] UiShadowEffect _shadow = new UiShadowEffect();
	[SerializeField] UiOutlineEffect _outline = new UiOutlineEffect();
	[SerializeField] UiGradientEffect _gradient = new UiGradientEffect();

	[SerializeField] List<int> _effectOrder = new List<int>{};

    EventSlot<UiSkinDefinition> _onChangeName = new EventSlot<UiSkinDefinition>();
    public IEventRegister<UiSkinDefinition> OnChangeName { get { return _onChangeName; } }

    EventSlot<UiSkinDefinition> _onChange = new EventSlot<UiSkinDefinition>();
    public IEventRegister<UiSkinDefinition> OnChange { get { return _onChange; } }

    public string Code { get { return _code; } set { if( _code != value ) { _code = value; _onChangeName.Trigger( this ); } } }
    public Color Color { get { return _color; } set { if( _color != value ) { _color = value; ChangeRelay(); } } }
    public Font Font { get { return _font; } set { if( _font != value ) { _font = value; ChangeRelay(); } } }
	
	public int FxCount { get { return _effectOrder.Count; } }
	public IUiSkinEffect GetFxAt( int id )
	{
		return GetFx( _effectOrder[id] );
	}

    void ChangeRelay()
    {
        _onChangeName.Trigger( this );
    }
	
	public UiShadowEffect Shadow { get { return _shadow; } }
	public UiOutlineEffect Outline { get { return _outline; } }
	public UiGradientEffect Gradient { get { return _gradient; } }
	
	public static int GetID<T>() where T : IUiSkinEffect, new()
	{
		if( typeof( T ) == typeof( UiShadowEffect ) ) return 0;
		if( typeof( T ) == typeof( UiOutlineEffect ) ) return 1;
		if( typeof( T ) == typeof( UiGradientEffect ) ) return 2;
		return -1;
	}

	public static int GetID( object o )
    {
        if( o == null ) return -1;

        if( o.GetType() == UiShadowEffect.Component ) return GetID<UiShadowEffect>();
        if( o.GetType() == UiOutlineEffect.Component ) return GetID<UiOutlineEffect>();
        if( o.GetType() == UiGradientEffect.Component ) return GetID<UiGradientEffect>();
        return -1;
    }
	
	public IUiSkinEffect GetFx( int id )
	{
		if( id == GetID<UiShadowEffect>() ) return _shadow;
		if( id == GetID<UiOutlineEffect>() ) return _outline;
		if( id == GetID<UiGradientEffect>() ) return _gradient;
		return null;
	}	

	public bool AddEffect<T>() where T : IUiSkinEffect, new()
	{
		if( Contains<T>() ) 
			return false;

		_effectOrder.Add( GetID<T>() );
		return true;
	}
	
	public bool Contains<T>() where T : IUiSkinEffect, new() { return _effectOrder.Contains( GetID<T>() ); }
	public void RemoveFx<T>() where T : IUiSkinEffect, new() { _effectOrder.RemoveAt( _effectOrder.IndexOf( GetID<T>() ) ); }
	public void MoveUp<T>() where T : IUiSkinEffect, new() { var id = _effectOrder.IndexOf( GetID<T>() ); MoveUp(id); }
	public void MoveDown<T>() where T : IUiSkinEffect, new() { var id = _effectOrder.IndexOf( GetID<T>() ); MoveDown(id); }

	public void MoveUp( int id ) { if( id > 0 ) Swap( id, id - 1 ); }
	public void MoveDown( int id ) { if( id < _effectOrder.Count - 1 ) Swap( id, id + 1 ); }

	public void Swap( int a, int b )
	{
		if( a >= 0 && a < _effectOrder.Count && b < _effectOrder.Count && b >= 0 )
		{
			var temp = _effectOrder[a];
			_effectOrder[a] = _effectOrder[b];
			_effectOrder[b] = temp;
		}
	}
	
	public void Apply( MaskableGraphic graphic )
	{
		graphic.color = _color;

        if( _font != null )
        {
            var t = graphic as Text;
            if( t != null )
            {
                t.font = _font;
            }
        }

        var components = graphic.gameObject.GetComponents<MonoBehaviour>();
        int it = 0;

        for( int i = 0; i < components.Length; i++ )
        {
            int id = GetID( components[ i ] );
            if( id >= 0 )
            {
                if( it >= _effectOrder.Count || _effectOrder[ it ] != id ) MonoBehaviour.DestroyImmediate( components[ i ] );
                else it++;
            }
        }

        //TODO: Check consistency of components order
        for( int i = 0; i < _effectOrder.Count; i++ )
        {
            var fx = GetFx( _effectOrder[ i ] );
            fx.Apply( graphic );
        }
		
		if( !Contains<UiOutlineEffect>() ) UiOutlineEffect.Destroy( graphic );
		if( !Contains<UiShadowEffect>() ) UiShadowEffect.Destroy( graphic );
		if( !Contains<UiGradientEffect>() ) UiGradientEffect.Destroy( graphic );
	}
}