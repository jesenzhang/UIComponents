#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州
//
// 模块名：    ChatWindow
// 创建者：    蔡亿飞
// 邮箱：      smallrainf@gmail.com
// 修改者列表：
// 创建日期：  2016-03-05 20:55:30
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Text;

public class ChatWindow : MonoBehaviour
{
    private EmojiExtension mEmojiExt;
    private UIScrollView mScrollView;
    private UIInput mInput;
    private GameObject mOkButton;
    private GameObject mClrButton;

    void Awake()
    {
        mEmojiExt = transform.Find("bg/scrollview/text").GetComponent<EmojiExtension>();
        mScrollView = transform.Find("bg/scrollview").GetComponent<UIScrollView>();
        mInput = transform.Find("input").GetComponent<UIInput>();
        mOkButton = transform.Find("okButton").gameObject;
        UIEventListener.Get(mOkButton).onClick = OnSubmit;
        mClrButton = transform.Find("clrButton").gameObject;
        UIEventListener.Get(mClrButton).onClick = OnClear;
    }

    private void OnSubmit(GameObject go)
    {
        mEmojiExt.Add("[00ff00]菠萝 03-26 11:55-37[-]", mInput.value);
        mInput.value = mInput.defaultText;
        mScrollView.ResetPosition();
    }

    private void OnClear(GameObject go)
    {
        mEmojiExt.Add("[00ff00]菠萝 03-26 11:55-37[-]", mInput.value, true);
        mInput.value = mInput.defaultText;
        mScrollView.ResetPosition();
    }
}
