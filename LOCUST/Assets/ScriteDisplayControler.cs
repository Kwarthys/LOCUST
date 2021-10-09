using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriteDisplayControler : MonoBehaviour
{
    private Image i;

    public int squareSize = 50;

    public Color triangleColor;

    // Start is called before the first frame update
    void Start()
    {
        i = GetComponent<Image>();

        i.material.mainTexture = buildTexture();
    }

    private Texture2D buildTexture()
    {
        Texture2D tx = new Texture2D(squareSize, squareSize);

        Color background = new Color(1, 1, 1, 0);
        Color[] colors = getColorArray(background, squareSize * squareSize);
        tx.SetPixels(colors);

        drawEquiTriangle(tx, triangleColor,3,0);

        tx.Apply();

        return tx;
    }

    private void drawEquiTriangle(Texture2D tex, Color c, float vI, float vH)
    {
        float s = 0.9f * squareSize;
        float h = s * Mathf.Sqrt(3) / 2;
        float vpadding = (squareSize - h) / 2;

        Vector2Int top = new Vector2Int(Mathf.RoundToInt(squareSize / 2), Mathf.RoundToInt(squareSize - vpadding));
        Vector2Int left = new Vector2Int(Mathf.RoundToInt((squareSize - s) / 2), Mathf.RoundToInt(vpadding));
        Vector2Int right = new Vector2Int(Mathf.RoundToInt((squareSize + s) / 2), Mathf.RoundToInt(vpadding));

        drawRoundedSegment(tex, top, left, c, 5);
        drawRoundedSegment(tex, top, right, c, 5);
        drawRoundedSegment(tex, right, left, c, 5);

        //finding pointScore
        float tvI = vI / 3 * h + vpadding; //line is y=tvI or y-tvI=0

        Vector2 midPointH = new Vector2((top.x + right.x) / 2, (top.y + right.y) / 2);
        Vector2 inter = Vector2.Lerp(midPointH, left, vH / 3);

        float lineVhM = (midPointH.y - left.y) / (midPointH.x - left.x);
        float lineVhB = midPointH.y - lineVhM * midPointH.x; // line is y = lineVhM * x + lineVhB

        float perpendicularM = -1 / lineVhM;
        float perpendicularB = inter.y - perpendicularM * inter.x; // line is y = perpendicularM * x + perpendicularB or perpendicularM * x + perpendicularB - y = 0

        //intersection between perpendicular and tvI
        float xp = (tvI - perpendicularB) / perpendicularM;
        float yp = tvI;
        Vector2Int point = new Vector2Int(Mathf.RoundToInt(xp), Mathf.RoundToInt(yp));

        drawCircle(tex, point, 10, Color.black);
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
            t += 1 / d;
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
