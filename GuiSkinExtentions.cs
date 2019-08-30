using UnityEngine;
using System.Collections;

public static class GuiSkinExtentions
{
    public static void SetSkin( this UnityEngine.UI.MaskableGraphic graphic, string skinCode )
    {
        var comp = graphic.GetComponent<UiSkinner>();
        if( comp == null ) comp = graphic.gameObject.AddComponent<UiSkinner>();

        comp.SkinCode = skinCode;
        comp.Update();
    }
}

