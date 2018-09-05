#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州
//
// 模块名：    AutoCommentScript
// 创建者：    蔡亿飞
// 邮箱：      smallrainf@gmail.com
// 修改者列表：
// 创建日期：  2016-01-05 22:21:38
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using System.IO;
using System.Text;

public class AutoCommentScript : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset( string path )
    {
        string[] segments = path.Split( '.' );
        if ( segments[1] != "cs" )
            return;

        string filename = segments[0] + "." + segments[1];
        string content = File.ReadAllText( filename, Encoding.UTF8 );
        content = content.Replace( "#SCRIPTDATE#", System.DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) );
        File.WriteAllText( filename, content, Encoding.UTF8 );
    }
}
