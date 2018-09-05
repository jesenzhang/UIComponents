#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：RichText
// 创建者：蔡亿飞
// 修改者列表：
// 创建日期：5/3/2016
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class EmojiExtension : MonoBehaviour
{
    private List<GameObject> mItems = new List<GameObject>();

    public UIAtlas atlas;
    private BetterList<string> mListOfSprites;
    private GameObject mLabelTemplate;

    private int mWidgetDepth;
    private int mFontSize;
    private float mLineWidth;
    private float mLineHeight;
    private float mSpacingX;
    private float mSpacingY;

    private float mCurrentHeight = 0.0f;
    private float mfontScale = 1.0f;
    void Awake()
    {
        UILabel uiLabel = GetComponent<UILabel>();
        mWidgetDepth = uiLabel.depth + 1;
        mFontSize = uiLabel.fontSize;
        mLineWidth = uiLabel.width;
        mLineHeight = uiLabel.fontSize + uiLabel.effectiveSpacingY;
        mSpacingX = uiLabel.effectiveSpacingX;
        //mSpacingY = uiLabel.effectiveSpacingY;

        mListOfSprites = atlas.GetListOfSprites();
    }

    void Start()
    {
        mLabelTemplate = Instantiate(gameObject) as GameObject;
        Destroy(mLabelTemplate.GetComponent<EmojiExtension>());
        mLabelTemplate.name = "LabelTemplate";
        mLabelTemplate.transform.parent = transform.parent;
        mLabelTemplate.SetActive(false);
    }

    public void Add(string header, string text, bool isRefresh = false)
    {
        if (isRefresh)
            Refresh();

        ProcessedText(header, text);
    }

    public void Refresh()
    {
        transform.DetachChildren();
        for (int i = mItems.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(mItems[i]);
        }
        mCurrentHeight = 0.0f;
        mItems.Clear();
    }
    StringBuilder sb = new StringBuilder();
    private void ProcessedText(string header, string text)
    {
        GetComponent<UILabel>().UpdateNGUIText();

        GameObject go = NGUITools.AddChild(gameObject);
        go.name = "Item";
        go.transform.localPosition = new Vector3(0, -mCurrentHeight, 0);
        mItems.Add(go);

        sb.AppendLine(header);
        int numOfLine = 0;
        bool isUseColor = false;
        float regionWidth = mLineWidth;
        for (int offset = 0; offset < text.Length; offset++)
        {
            char ch = text[offset];
            if (ch == '#')
            {
                if (ParseColor(ref text, ref isUseColor, ref offset, ref sb))
                    continue;

                if (ParseEmoji(ref text, ref go, ref regionWidth, ref numOfLine, ref offset, ref sb))
                    continue;
            }

            float w = NGUIText.GetGlyphWidth(ch, 0, mfontScale) + mSpacingX;
            if (regionWidth - w < 0)
            {
                sb.Append('\n');
                ++numOfLine;
                regionWidth = mLineWidth;
            }
            sb.Append(ch);
            regionWidth -= w;
        }

        if (isUseColor)
        {
            sb.Append("[-]");
        }

        UILabel uiLabel = AddEmojiText(go, sb.ToString());
        mCurrentHeight += uiLabel.height;
    }

    private bool ParseColor(ref string text, ref bool isUseColor, ref int offset, ref StringBuilder sb)
    {
        char color = text[offset + 1];
        if (color == 'R' || color == 'G' || color == 'B')
        {
            if (isUseColor)
                sb.Append("[-]");

            switch (color)
            {
                case 'R':
                    sb.Append("[ff0000]");
                    break;
                case 'G':
                    sb.Append("[00ff00]");
                    break;
                case 'B':
                    sb.Append("[0000ff]");
                    break;
            }
            isUseColor = true;
            offset += 1;
            return true;
        }
        return false;
    }

    private bool ParseEmoji(ref string text, ref GameObject go, ref float regionWidth,
        ref int numOfLine, ref int offset, ref StringBuilder sb)
    {
        int start = offset;
        int end = GetLastNumberIndex(text, start);
        if (end >= 0)
        {
            string spriteName = text.Substring(start, end - start + 1);
            if (IsEmojiImage(spriteName))
            {
                int num = CalcuNumOfPlaceholder();
                float width = NGUIText.GetGlyphWidth(' ', 0, mfontScale) + mSpacingX;
                float totalWidth = num * width;
                if (regionWidth - totalWidth < 0)
                {
                    sb.Append('\n');
                    ++numOfLine;
                    regionWidth = mLineWidth;
                }
                Vector3 pos = CalculateEmojiPos(mLineWidth - regionWidth, numOfLine);
                AddEmojiImage(go, pos, spriteName);
                sb.Append(' ', num);
                regionWidth -= totalWidth;

                // offset += end - start + 1 - 1 ===> offset += end - start
                offset += end - start;
                return true;
            }
        }
        return false;
    }

    private int GetLastNumberIndex(string str, int start)
    {
        int end = -1;
        for (int i = start + 1; i < str.Length; i++)
        {
            if (char.IsDigit(str[i]))
                end = i;
            else
                break;
        }
        return end;
    }

    private int CalcuNumOfPlaceholder()
    {
        UISpriteData data = atlas.GetSprite(mListOfSprites[0]);
        float width = NGUIText.GetGlyphWidth(' ', 0, mfontScale) + mSpacingX;
        int num = Mathf.CeilToInt(data.width / width);
        return num;
    }

    private Vector3 CalculateEmojiPos(float x, float y)
    {
        float yPos = (y + 1) * mLineHeight;
        return new Vector3(x, -yPos, 0);
    }

    private bool IsEmojiImage(string spriteName)
    {
        if (mListOfSprites.Contains(spriteName))
            return true;
        return false;
    }

    private UILabel AddEmojiText(GameObject parent, string text)
    {
        GameObject go = NGUITools.AddChild(parent, mLabelTemplate);
        UILabel uiLabel = go.GetComponent<UILabel>();
        uiLabel.name = "Label";
        uiLabel.depth = mWidgetDepth;
        uiLabel.overflowMethod = UILabel.Overflow.ResizeHeight;
        uiLabel.text = text;
        go.SetActive(true);
        return uiLabel;
    }

    private UISprite AddEmojiImage(GameObject parent, Vector3 pos, string spriteName)
    {
        UISprite uiSprite = NGUITools.AddChild<UISprite>(parent);
        uiSprite.name = "Sprite";
        uiSprite.depth = mWidgetDepth;
        uiSprite.pivot = UIWidget.Pivot.TopLeft;
        uiSprite.atlas = atlas;
        uiSprite.spriteName = spriteName;
        uiSprite.transform.localPosition = pos;
        //uiSprite.MakePixelPerfect();
        uiSprite.width = mFontSize;
        uiSprite.height = mFontSize;
        return uiSprite;
    }
}
