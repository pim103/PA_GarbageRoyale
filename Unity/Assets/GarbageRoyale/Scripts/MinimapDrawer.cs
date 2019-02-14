using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapDrawer : MonoBehaviour
{
    
    private GUIStyle currentStyle = null;

    private int pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        InitStyles();
        GUI.Box(new Rect(pos,pos,20,20),"",currentStyle);
        pos++;
    }
    
    private void InitStyles()
    {
        if( currentStyle == null )
        {
            currentStyle = new GUIStyle( GUI.skin.box );
            currentStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 1f, 0f, 0.5f ) );
        }
    }
 
    private Texture2D MakeTex( int width, int height, Color col )
    {
        Color[] pix = new Color[width * height];
        for( int i = 0; i < pix.Length; ++i )
        {
            pix[ i ] = col;
        }
        Texture2D result = new Texture2D( width, height );
        result.SetPixels( pix );
        result.Apply();
        return result;
    }
}
