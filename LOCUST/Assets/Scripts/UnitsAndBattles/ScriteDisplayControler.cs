using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriteDisplayControler : MonoBehaviour
{
    public RawImage i;

    private Texture2D tex;
    private Material m;
    public Material source;

    public int squareSize = 50;

    public Color triangleColor;
    public Color pointColor;
    public Color secondPointColor;


    private float side;
    private float height;
    private float vpadding;

    private Vector2Int top;
    private Vector2Int left;
    private Vector2Int right;

    private Vector2 midPointH;

    private float lineVhM;
    private float lineVhB;

    private float perpendicularM;

    private void cleanTexture(Texture2D tex)
    {
        Color background = triangleColor;
        background.a = 0;
        Color[] colors = getColorArray(background, squareSize * squareSize);
        tex.SetPixels(colors);
    }

    private void initComponent()
    {
        m = new Material(source);
        i.material = m;

        tex = new Texture2D(squareSize, squareSize);
        m.SetTexture("_MainTex", tex);

        //calculating everything not related to scores beforehand
        side = 0.9f * squareSize;
        height = side * Mathf.Sqrt(3) / 2;
        vpadding = (squareSize - height) / 2;

        top = new Vector2Int(Mathf.RoundToInt(squareSize / 2), Mathf.RoundToInt(squareSize - vpadding));
        left = new Vector2Int(Mathf.RoundToInt((squareSize - side) / 2), Mathf.RoundToInt(vpadding));
        right = new Vector2Int(Mathf.RoundToInt((squareSize + side) / 2), Mathf.RoundToInt(vpadding));

        midPointH = new Vector2((top.x + right.x) / 2, (top.y + right.y) / 2);

        lineVhM = (midPointH.y - left.y) / (midPointH.x - left.x);
        lineVhB = midPointH.y - lineVhM * midPointH.x; // line is y = lineVhM * x + lineVhB

        perpendicularM = -1 / lineVhM;
    }

    public void rebuildFor(float vI, float vH)
    {
        rebuildFor(vI, vH, -1, -1);
    }

    public void rebuildFor(float vI, float vH, float i2, float h2)
    {
        if (m == null)
        {
            initComponent();
        }

        cleanTexture(tex);

        drawTriangleDisplay(tex, triangleColor, pointColor, vI, vH);

        if(i2 != -1)
        {
            drawTriangleDisplay(tex, triangleColor, secondPointColor, i2, h2, false);
        }
        tex.Apply();
    }

    private void drawTriangleDisplay(Texture2D tex, Color triangleColor, Color pointColor, float vI, float vH, bool drawTriangle = true)
    {
        int w = 10;

        if(drawTriangle)
        {
            drawRoundedSegment(tex, top, left, triangleColor, w);
            drawRoundedSegment(tex, top, right, triangleColor, w);
            drawRoundedSegment(tex, right, left, triangleColor, w);
        }

        //finding pointScore
        float tvI = vI / 3 * height + vpadding; //line is y=tvI or y-tvI=0
        Vector2 inter = Vector2.Lerp(midPointH, left, vH / 3);

        float perpendicularB = inter.y - perpendicularM * inter.x; // line is y = perpendicularM * x + perpendicularB or perpendicularM * x + perpendicularB - y = 0

        //intersection between perpendicular and tvI
        float xp = (tvI - perpendicularB) / perpendicularM;
        float yp = tvI;
        Vector2Int point = new Vector2Int(Mathf.RoundToInt(xp), Mathf.RoundToInt(yp));

        drawCircle(tex, point, w*2, pointColor);
    }

    private void drawRoundedSegment(Texture2D tex, Vector2Int pointA, Vector2Int pointB, Color c, int width = 1)
    {
        drawSegment(tex, pointA, pointB, c, width, true);
        drawCircle(tex, pointA, width, c);
        drawCircle(tex, pointB, width, c);
    }

    private void drawCircle(Texture2D tex, Vector2Int center, int radius, Color c)
    {
        for (int j = center.y - radius; j <= center.y + radius; ++j)
        {
            for (int i = center.x - radius; i < center.x + radius; ++i)
            {
                float sqrMag = (i - center.x) * (i - center.x) + (j - center.y) * (j - center.y);

                if ( sqrMag < radius*radius) tex.SetPixel(i, j, c);
            }
        }
    }

    private void drawSegment(Texture2D tex, Vector2Int pointA, Vector2Int pointB, Color c, int width = 1, bool pointyEnds = false)
    {
        float d = Vector2Int.Distance(pointA, pointB);

        float pointyEndLength = 1/d;

        float t = 0;

        while (t < 1)
        {
            Vector2 x = Vector2.Lerp(pointA, pointB, t);

            float wcoef = 1;

            if(pointyEnds)
            {
                if(t < pointyEndLength)
                {
                    wcoef = t / pointyEndLength;
                }
                else if(t > 1 - pointyEndLength)
                {
                    wcoef = (1-t) / pointyEndLength;
                }
            }

            int usedWidth = Mathf.RoundToInt(width * wcoef);
            if (usedWidth == 0) usedWidth = 1;

            Vector2Int point = new Vector2Int(Mathf.RoundToInt(x.x), Mathf.RoundToInt(x.y));

            Vector2Int squareStart = new Vector2Int(point.x + 1 - usedWidth, point.y + 1 - usedWidth);

            Color[] colors = getColorArray(c, (usedWidth * 2 - 1) * (usedWidth * 2 - 1));

            tex.SetPixels(squareStart.x, squareStart.y, usedWidth * 2 - 1, usedWidth * 2 - 1, colors);
            //tex.SetPixel(point.x, point.y,Color.black);
            t += usedWidth / d;
        }
    }

    private Color[] getColorArray(Color c, int size)
    {
        Color[] colors = new Color[size];

        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = c;
        }

        return colors;
    }
}
