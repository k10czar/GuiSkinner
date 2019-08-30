using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum GradientMode { Global, Local }
public enum GradientDir { Vertical, Horizontal, DiagonalLeftToRight, DiagonalRightToLeft }

[AddComponentMenu( "UI/Effects/Gradient" )]
public class Gradient
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
	: BaseMeshEffect
#else
    : BaseVertexEffect
#endif
{
	[SerializeField] GradientMode _gradientMode = GradientMode.Global;
	[SerializeField] GradientDir _gradientDir = GradientDir.Vertical;
	[SerializeField] bool _overwriteAllColor = false;
	[SerializeField] Color _color1 = Color.white;
	[SerializeField] Color _color2 = new Color( .5f, .5f, .5f, 1 );

	public GradientMode Mode { get { return _gradientMode; } set { _gradientMode = value; } }
	public GradientDir Direction { get { return _gradientDir; } set { _gradientDir = value; } }
	public bool OverwriteAllColor { get { return _overwriteAllColor; } set { _overwriteAllColor = value; } }
	public Color Color1 { get { return _color1; } set { _color1 = value; } }
	public Color Color2 { get { return _color2; } set { _color2 = value; } }

	private Graphic targetGraphic;

	protected override void Start()
	{
		targetGraphic = GetComponent<Graphic>();
	}

#if UNITY_5_2 || UNITY_5_3_OR_NEWER
#if UNITY_5_2_2 || UNITY_5_3_OR_NEWER
	public override void ModifyMesh( VertexHelper vh )
#elif UNITY_5_2
		public override void ModifyMesh( Mesh mesh )
#endif
	{
		if( !this.IsActive() )
			return;

		List<UIVertex> list = new List<UIVertex>();
#if UNITY_5_2_2 || UNITY_5_3_OR_NEWER
		vh.GetUIVertexStream( list );
#elif UNITY_5_2
			using( VertexHelper vertexHelper = new VertexHelper( mesh ) )
			{
				vertexHelper.GetUIVertexStream( list );
			}
#endif

		ModifyVertices( list );  // calls the old ModifyVertices which was used on pre 5.2

#if UNITY_5_2_2 || UNITY_5_3_OR_NEWER
		vh.Clear();
		vh.AddUIVertexTriangleStream( list );
#else
			using( VertexHelper vertexHelper2 = new VertexHelper() )
			{
				vertexHelper2.AddUIVertexTriangleStream( list );
				vertexHelper2.FillMesh( mesh );
			}
#endif
	}
#endif

#if UNITY_5_2  || UNITY_5_3_OR_NEWER
	public void ModifyVertices( List<UIVertex> vertexList )
#else
	public override void ModifyVertices( List<UIVertex> vertexList )
#endif
	{
		if( !IsActive() || vertexList.Count == 0 ) return;
		int count = vertexList.Count;
		UIVertex uiVertex = vertexList[0];
		if( _gradientMode == GradientMode.Global )
		{
			if( _gradientDir == GradientDir.DiagonalLeftToRight || _gradientDir == GradientDir.DiagonalRightToLeft )
			{
#if UNITY_EDITOR
				Debug.LogError( "Diagonal dir is not supported in Global mode" );
#endif
				_gradientDir = GradientDir.Vertical;
			}
			float bottomY = _gradientDir == GradientDir.Vertical ? vertexList[vertexList.Count - 1].position.y : vertexList[vertexList.Count - 1].position.x;
			float topY = _gradientDir == GradientDir.Vertical ? vertexList[0].position.y : vertexList[0].position.x;

			float uiElementHeight = topY - bottomY;

			for( int i = 0; i < count; i++ )
			{
				uiVertex = vertexList[i];
				if( !_overwriteAllColor && uiVertex.color != targetGraphic.color ) continue;
				uiVertex.color *= Color.Lerp( _color2, _color1, ( ( _gradientDir == GradientDir.Vertical ? uiVertex.position.y : uiVertex.position.x ) - bottomY ) / uiElementHeight );
				vertexList[i] = uiVertex;
			}
		}
		else
		{
			for( int i = 0; i < count; i++ )
			{
				uiVertex = vertexList[i];
				if( !_overwriteAllColor && !EqualColor( uiVertex.color, targetGraphic.color ) ) continue;
				switch( _gradientDir )
				{
					case GradientDir.Vertical: uiVertex.color *= ( i % 4 == 0 || ( i - 1 ) % 4 == 0 ) ? _color1 : _color2; break;
					case GradientDir.Horizontal: uiVertex.color *= ( i % 4 == 0 || ( i - 3 ) % 4 == 0 ) ? _color1 : _color2; break;
					case GradientDir.DiagonalLeftToRight: uiVertex.color *= ( i % 4 == 0 ) ? _color1 : ( ( i - 2 ) % 4 == 0 ? _color2 : Color.Lerp( _color2, _color1, 0.5f ) ); break;
					case GradientDir.DiagonalRightToLeft: uiVertex.color *= ( ( i - 1 ) % 4 == 0 ) ? _color1 : ( ( i - 3 ) % 4 == 0 ? _color2 : Color.Lerp( _color2, _color1, 0.5f ) ); break;
				}
				vertexList[i] = uiVertex;
			}
		}
	}

 	private bool EqualColor( Color a, Color b ) { return Mathf.Approximately( a.r, b.r ) &&( Mathf.Approximately( a.g, b.g ) && Mathf.Approximately( a.b, b.b ) && Mathf.Approximately( a.a, b.a ) ); }
}