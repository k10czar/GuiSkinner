//Author: Melang http://forum.unity3d.com/members/melang.593409/
using System;
using System.Collections.Generic;
namespace UnityEngine.UI
{
	//An outline that looks a bit nicer than the default one. It has less "holes" in the outline by drawing more copies of the effect
	[AddComponentMenu( "UI/Effects/NicerOutline", 15 )]

#if UNITY_5_2 || UNITY_5_3_OR_NEWER
	public class NicerOutline : BaseMeshEffect
#else
	public class NicerOutline : BaseVertexEffect
#endif
	{
		const float MAX = 600;
		const int MAX_INTERACTIONS = 360;

		[SerializeField] private Color _effectColor = new Color( 0f, 0f, 0f, 0.5f );
		[SerializeField] private float _size = 2;
		[SerializeField] private int _interactions = 8;
		[SerializeField] private bool m_UseGraphicAlpha = true;

		public Color EffectColor
		{
			get { return _effectColor; }
			set
			{
				this._effectColor = value;
				if( base.graphic != null )
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		public int Interactions
		{
			get { return _interactions; }
			set
			{
				value = ClampInteractions( value );

				if( _interactions == value ) return;
				_interactions = value;

				if( base.graphic != null ) base.graphic.SetVerticesDirty();
			}
		}

		int ClampInteractions( int val ) { return Mathf.Clamp( val, 1, MAX_INTERACTIONS ); }

		public float Size
		{
			get { return _interactions; }
			set
			{
				value = Mathf.Clamp( value, -MAX, MAX );

				if( this._size == value ) return;
				this._size = value;

				if( base.graphic != null ) base.graphic.SetVerticesDirty();
			}
		}

		public bool useGraphicAlpha
		{
			get { return m_UseGraphicAlpha; }
			set
			{
				this.m_UseGraphicAlpha = value;
				if( base.graphic != null )
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		
		protected void ApplyShadow( List<UIVertex> verts, Color32 color, int start, int end, float x, float y )
		{
			//int num = verts.Count * 2;
			if( verts.Capacity < end )
			{
				verts.Capacity = end;
			}

			for( int i = start; i < end; i++ )
			{
				UIVertex uIVertex = verts[i];
				verts.Add( uIVertex );

				Vector3 position = uIVertex.position;
				//Debug.Log("vertex pos: "+position);
				position.x += x;
				position.y += y;
				uIVertex.position = position;
				Color32 color2 = color;
				if( this.m_UseGraphicAlpha )
				{
					color2.a = (byte)( color2.a * verts[i].color.a / 255 );
				}
				uIVertex.color = color2;
				//uIVertex.color = (Color32)Color.blue;
				verts[i] = uIVertex;
			}
		}


#if UNITY_5_2 || UNITY_5_3_OR_NEWER
#if UNITY_5_2_2 || UNITY_5_3_OR_NEWER
		public override void ModifyMesh( VertexHelper vh )
#elif UNITY_5_2
		public override void ModifyMesh( Mesh mesh )
#endif
		{
			if( !IsActive() )
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

#if UNITY_5_2 || UNITY_5_3_OR_NEWER
		public void ModifyVertices( List<UIVertex> verts )
		{ 
#else
		public override void ModifyVertices( List<UIVertex> verts )
		{
			if( !this.IsActive() )
			{
				return;
			}
#endif
			Text foundtext = GetComponent<Text>();

			float best_fit_adjustment = 1f;

			if( foundtext && foundtext.resizeTextForBestFit )
			{
				best_fit_adjustment = (float)foundtext.cachedTextGenerator.fontSizeUsedForBestFit / ( foundtext.resizeTextMaxSize - 1 ); //max size seems to be exclusive 

			}
			
			float distance = _size * best_fit_adjustment;
			
			int count = verts.Count;

			_interactions = ClampInteractions( _interactions );
			float step = (2*Mathf.PI) / _interactions;
			for( float i = 0; i < _interactions; i++ )
			{
				float ang = step * i;
                this.ApplyShadow( verts, this.EffectColor, verts.Count - count, verts.Count, Mathf.Cos( ang ) * distance, Mathf.Sin( ang ) * distance );
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			this.Size = this._size;
			base.OnValidate();
		}
#endif
	}
}
