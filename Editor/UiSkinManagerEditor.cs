using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using K10.EditorGUIExtention;

[CustomEditor(typeof(UiSkinManager))]
public class UiSkinManagerEditor : Editor 
{
	const int ICON_SIZE = 16;

	List<bool> _folds = new List<bool>();
	UiSkinManager _target = null;
	
	protected void OnEnable()
	{
		_target = (UiSkinManager)target;
	}

	void ButtonAddEffect( SerializedProperty list, int id, string name )
	{
		var listId = -1;

		for( int i = 0; i < list.arraySize && listId == -1; i++ )
			if( list.GetArrayElementAtIndex( i ).intValue == id )
				listId = i;

		EditorGUILayout.BeginHorizontal();
		var has = ( listId != -1 );
        if( EditorGUILayout.Toggle( has, GUILayout.MaxWidth( 15 ) ) != has )
		{
			if( !has )
			{
				listId = list.arraySize;
				list.arraySize++;
				list.GetArrayElementAtIndex( listId ).intValue = id;
			}
			else
			{
				K10EditorGUIUtils.RemoveItemFromArray( list, listId );
            }
		}

		var icon = IconCache.Get( name.ToLower() ).Texture;
		if( icon != null ) GUILayout.Label( icon, K10GuiStyles.basicCenterStyle, GUILayout.Width( ICON_SIZE ) );
		
		GUILayout.Label( name, K10GuiStyles.smallboldStyle );
		EditorGUILayout.EndHorizontal();
	}

	void EditShadow( SerializedProperty fxProp, SerializedProperty fxOrderProp, int id )
	{
		EditorGUILayout.BeginHorizontal();
		EditEffect( fxOrderProp, id );

		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal();
		DrawFxIcon<UiShadowEffect>();
		GUILayout.Label( "Shadow", K10GuiStyles.boldStyle, GUILayout.MaxWidth( 70 ) );
		GUILayout.Label( "Offset", GUILayout.MaxWidth( 38 ) );

		var distProp = fxProp.FindPropertyRelative( "_shadowDistance" );
		distProp.vector2Value = EditorGUILayout.Vector2Field( "", distProp.vector2Value, GUILayout.MinWidth( 70 ), GUILayout.MaxWidth( 130 ) );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		
		ColorPicker.Layout( fxProp.FindPropertyRelative( "_shadowColor" ) );

		GUILayout.Label( "Use Graphic Alpha", GUILayout.MinWidth( 50 ), GUILayout.MaxWidth( 110 ) );
		var alphaProp = fxProp.FindPropertyRelative( "_useGraphicAlpha" );
		alphaProp.boolValue = EditorGUILayout.Toggle( alphaProp.boolValue, GUILayout.MaxWidth( 15 ) );

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}

	void EditOutline( SerializedProperty fxProp, SerializedProperty fxOrderProp, int id )
	{
		EditorGUILayout.BeginHorizontal();
		EditEffect( fxOrderProp, id );

		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();

		DrawFxIcon<UiOutlineEffect>();
		GUILayout.Label( "Outline", K10GuiStyles.boldStyle, GUILayout.MaxWidth( 65 ) );

		var initialValue = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 30;

		var sizeProp = fxProp.FindPropertyRelative( "_outlineSize" );
		sizeProp.floatValue = EditorGUILayout.FloatField( "Size", sizeProp.floatValue, GUILayout.MaxWidth( 60 ) );
		//GUILayout.Label( "Interactions", GUILayout.MinWidth( 30 ), GUILayout.MaxWidth( 73 ) );
		EditorGUIUtility.labelWidth = 73;

		var interactionsProp = fxProp.FindPropertyRelative( "_outlineInteractions" );
		interactionsProp.intValue = EditorGUILayout.IntField( "Interactions", interactionsProp.intValue, GUILayout.MaxWidth( 103 ) );
		interactionsProp.intValue = Mathf.Max( interactionsProp.intValue, 0 );
        EditorGUIUtility.labelWidth = initialValue;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		ColorPicker.Layout( fxProp.FindPropertyRelative( "_outlineColor" ) );
		GUILayout.Label( "Use Graphic Alpha", GUILayout.MinWidth( 50 ), GUILayout.MaxWidth( 110 ) );
		var alphaProp = fxProp.FindPropertyRelative( "_useGraphicAlpha" );
		alphaProp.boolValue = EditorGUILayout.Toggle( alphaProp.boolValue, GUILayout.MaxWidth( 15 ) );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}

	void EditGradient( SerializedProperty fxProp, SerializedProperty fxOrderProp, int id )
	{
		EditorGUILayout.BeginHorizontal();
		EditEffect( fxOrderProp, id );

		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		DrawFxIcon<UiGradientEffect>();
		GUILayout.Label( "Gradient", K10GuiStyles.boldStyle, GUILayout.MaxWidth( 80 ) );

		var modeProp = fxProp.FindPropertyRelative( "_mode" );
		modeProp.enumValueIndex = (int)(GradientMode)EditorGUILayout.EnumPopup( (GradientMode)modeProp.enumValueIndex );

		var dirProp = fxProp.FindPropertyRelative( "_direction" );
		dirProp.enumValueIndex = (int)(GradientDir)EditorGUILayout.EnumPopup( (GradientDir)dirProp.enumValueIndex );

		var overProp = fxProp.FindPropertyRelative( "_overwriteAllColors" );
		overProp.boolValue = EditorGUILayout.Toggle( overProp.boolValue, GUILayout.MaxWidth( 15 ) );

		GUILayout.Label( "Overwrite All Colors", GUILayout.MaxWidth( 120 ) );

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		var color1Prop = fxProp.FindPropertyRelative( "_color1" );
		var color2Prop = fxProp.FindPropertyRelative( "_color2" );
		ColorPicker.Layout( color1Prop );
		if( GUILayout.Button( "↔", GUILayout.MaxWidth( 25 ) ) ) { var c = color1Prop.colorValue; color1Prop.colorValue = color2Prop.colorValue; color2Prop.colorValue = c; }
		ColorPicker.Layout( color2Prop );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}

	void EditEffect( SerializedProperty fxOrderProp, int id )
	{
		var canUp = id > 0;
		var canDown = id < fxOrderProp.arraySize - 1;

		EditorGUILayout.BeginVertical( GUILayout.MaxWidth( 20 ) );
		//GUILayout.Space( 5);
		GuiColorManager.New( ( canUp ) ? Color.white : Color.gray );
		if( GUILayout.Button( "↑", GUILayout.MaxWidth( 15 ) ) && canUp )
		{
			var val = fxOrderProp.GetArrayElementAtIndex( id ).intValue;
			fxOrderProp.GetArrayElementAtIndex( id ).intValue = fxOrderProp.GetArrayElementAtIndex( id - 1 ).intValue;
			fxOrderProp.GetArrayElementAtIndex( id - 1 ).intValue = val;
		}
		GuiColorManager.Revert();

		//SeparationLine.Horizontal();

		GuiColorManager.New( ( canDown ) ? Color.white : Color.gray );
		if( GUILayout.Button( "↓", GUILayout.MaxWidth( 15 ) ) && canDown )
		{
			var val = fxOrderProp.GetArrayElementAtIndex( id ).intValue;
			fxOrderProp.GetArrayElementAtIndex( id ).intValue = fxOrderProp.GetArrayElementAtIndex( id + 1 ).intValue;
			fxOrderProp.GetArrayElementAtIndex( id + 1 ).intValue = val;
		}
		GuiColorManager.Revert();
		EditorGUILayout.EndVertical();
	}

    public void UpdateInstances()
    {
        var list = FindObjectsOfType<UiSkinner>();
        for( int i = 0; i < list.Length; i++ )
        {
            list[ i ].ApplyCachedSkin();
            EditorUtility.SetDirty( list[ i ].gameObject );
        }
    }

    public void SwapSkins( int a, int b )
    {
        var defs = _target.SkinDefinitions;

        var temp = defs[ a ]; 
        defs[ a ] = defs[ b ]; 
        defs[ b ] = temp;

        var tempB = _folds[ a ];
        _folds[ a ] = _folds[ b ];
        _folds[ b ] = tempB;
    }

	void DrawFxIcon<T>( T t ) where T : IUiSkinEffect
	{
		string icnCode = null;

		if( t is UiShadowEffect ) icnCode = "shadow";
		else if( t is UiOutlineEffect ) icnCode = "outline";
		else if( t is UiGradientEffect ) icnCode = "gradient";

		if( icnCode != null )
		{
			var icon = IconCache.Get( icnCode ).Texture;
			if( icon != null ) GUILayout.Label( icon, K10GuiStyles.basicCenterStyle, GUILayout.Width( ICON_SIZE ) );
		}
	}
    void DrawFxIcon<T>() where T : IUiSkinEffect
	{
		string icnCode = null;

		if( typeof(T) == typeof(UiShadowEffect) ) icnCode = "shadow";
		else if( typeof(T) == typeof(UiOutlineEffect) ) icnCode = "outline";
		else if( typeof(T) == typeof(UiGradientEffect) ) icnCode = "gradient";

		if( icnCode != null )
		{
			var icon = IconCache.Get( icnCode ).Texture;
			if( icon != null ) GUILayout.Label( icon, K10GuiStyles.basicCenterStyle, GUILayout.Width( ICON_SIZE ) );
		}
	}
	
	public override void OnInspectorGUI()
	{
        bool changed = false;

		serializedObject.Update();
		_target = (UiSkinManager)target;
		Event ev = Event.current;

		HashSet<string> _codes = new HashSet<string>();

		var defsProp = serializedObject.FindProperty( "_colorDefinitions" );
		//var defs = _target.SkinDefinitions;
		//if( defsProp.arraySize == 0 )
		//	return;

		if( _folds == null ) _folds = new List<bool>();
		while( _folds.Count > defsProp.arraySize ) { _folds.RemoveAt( _folds.Count - 1 ); }
		while( _folds.Count < defsProp.arraySize ) { _folds.Add( false ); }

		for( int i = 0; i < defsProp.arraySize; i++ )
		{
			SeparationLine.Horizontal();
			var def = defsProp.GetArrayElementAtIndex( i );

			var codeName = def.FindPropertyRelative( "_code" );
            var valid = !_codes.Contains( codeName.stringValue );
            var color = valid ? Color.white : Color.red;
			GuiColorManager.New( color );

			EditorGUILayout.BeginHorizontal();
			_folds[ i ] = EditorGUILayout.Foldout( _folds[ i ], ( valid ? "" : "!UNUSED! " ) + codeName.stringValue, K10GuiStyles.bigFoldStyle );
			

			var fxOrderProp = def.FindPropertyRelative( "_effectOrder" );
			var instance = _target.SkinDefinitions[i];
            for( int e = 0; e < instance.FxCount; e++ )
			{
				var fx = instance.GetFxAt( e );
				DrawFxIcon( fx );
            }

			bool canUp = i > 0;
			GuiColorManager.New( ( canUp ) ? color : Color.gray );
            if( GUILayout.Button( "↑", GUILayout.MaxWidth( 15 ) ) && canUp ) { SwapSkins( i, i - 1 ); }
			GuiColorManager.Revert();

			bool canDown = i < defsProp.arraySize - 1;
			GuiColorManager.New(  ( canDown ) ? color : Color.gray );
            if( GUILayout.Button( "↓", GUILayout.MaxWidth( 15 ) ) && canDown ) { SwapSkins( i, i + 1 ); }
			GuiColorManager.Revert();

			GuiColorManager.New(  new Color( .6f,.1f,.1f,1 ) );
			if( GUILayout.Button( "X", GUILayout.MaxWidth( 20 ) ) )
			{
				K10EditorGUIUtils.RemoveItemFromArray( defsProp, i );				
				i--;
				GuiColorManager.Revert();
				GuiColorManager.Revert();
				EditorGUILayout.EndHorizontal();
				continue;
			}

			GuiColorManager.Revert();
			EditorGUILayout.EndHorizontal();

			if( _folds[i] )
			{
				SeparationLine.Horizontal();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( "Codename", K10GuiStyles.smallboldStyle, GUILayout.Width( 75 ) );
				codeName.stringValue = EditorGUILayout.TextField( codeName.stringValue, K10GuiStyles.smalltextFieldStyle, GUILayout.Height( 18 ) );
				_codes.Add( codeName.stringValue );
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				//GUILayout.Label( "Color", K10EditorGUIUtils._smallboldStyle, GUILayout.Width( 40 ) );
				ColorPicker.Layout( def.FindPropertyRelative( "_color" ) );
				//def.Color = EditorGUILayout.ColorField( def.Color );
				//GUILayout.Label( "Font", K10EditorGUIUtils._smallboldStyle, GUILayout.Width( 35 ) );
				var fontProp = def.FindPropertyRelative( "_font" );
				fontProp.objectReferenceValue = (Font)EditorGUILayout.ObjectField( fontProp.objectReferenceValue, typeof( Font ), false );
				EditorGUILayout.EndHorizontal();

				
				SeparationLine.Horizontal();
				GUILayout.Label( "Effects", K10GuiStyles.titleStyle );
				SeparationLine.Horizontal();

				var shadowProp = def.FindPropertyRelative( "_shadow" );
				var outlineProp = def.FindPropertyRelative( "_outline" );
				var gradientProp = def.FindPropertyRelative( "_gradient" );

				var sid = UiSkinDefinition.GetID<UiShadowEffect>();
				var oid = UiSkinDefinition.GetID<UiOutlineEffect>();
				var gid = UiSkinDefinition.GetID<UiGradientEffect>();

				EditorGUILayout.BeginHorizontal();
				//GUILayout.FlexibleSpace();
				//EditorGUILayout.BeginVertical();
				ButtonAddEffect( fxOrderProp, sid, "Shadow" );
				ButtonAddEffect( fxOrderProp, oid, "Outline" );
				ButtonAddEffect( fxOrderProp, gid, "Gradient" );
				//EditorGUILayout.EndVertical();
				//GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

				if( fxOrderProp.arraySize > 0 )
				{
					SeparationLine.Horizontal();
					GUILayout.Label( "Edit Effects", K10GuiStyles.titleStyle );
					for( int e = 0; e < fxOrderProp.arraySize; e++ )
					{
						SeparationLine.Horizontal();
						if( sid == fxOrderProp.GetArrayElementAtIndex( e ).intValue ) EditShadow( shadowProp, fxOrderProp, e );
						else if( oid == fxOrderProp.GetArrayElementAtIndex( e ).intValue ) EditOutline( outlineProp, fxOrderProp, e );
						else if( gid == fxOrderProp.GetArrayElementAtIndex( e ).intValue ) EditGradient( gradientProp, fxOrderProp, e );
					}
				}
				
			}
			
			SeparationLine.Horizontal();
			EditorGUILayout.Space();
			GuiColorManager.Revert();
		}

		if( GUILayout.Button( "Add New Skin Definition", K10GuiStyles.buttonStyle, GUILayout.Height( 30 ) ) )
		{
			defsProp.arraySize++;
		}

		serializedObject.ApplyModifiedProperties();

		changed = true;
        if( changed )
            UpdateInstances();
	}
}
