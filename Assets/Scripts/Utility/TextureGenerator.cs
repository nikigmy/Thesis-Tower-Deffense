using System;
using System.Drawing;
using UnityEngine;

public class TextureGenerator
{
    public static Sprite GetTextureForLevel(Declarations.LevelData levelData)
    {
        int HexHeight = 10;
        var width = (int)HexToPoints(HexHeight, 1, levelData.MapSize.x - 1)[4].X;
        var height = (int)HexToPoints(HexHeight, levelData.MapSize.y - 1, 0)[0].Y;
        var pointForHexes = FillPointForHexes(levelData, HexHeight);
        Bitmap myBitmap = new Bitmap(width, height);
        var graphics = System.Drawing.Graphics.FromImage(myBitmap);
        graphics.Clear(System.Drawing.Color.Transparent);
        DrawHexGrid(graphics, pointForHexes, levelData);

        Texture2D texture = new Texture2D(width, height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var pixel = myBitmap.GetPixel(i, height - j - 1);
                texture.SetPixel(i, j, new UnityEngine.Color((float)pixel.R / 255, (float)pixel.G / 255, (float)pixel.B / 255, (float)pixel.A / 255));
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero);
    }

    private static PointF[,][] FillPointForHexes(Declarations.LevelData levelData, int HexHeight)
    {
        var pointForHexes = new PointF[levelData.MapSize.y, levelData.MapSize.x][];
        // Loop until a hexagon won't fit.
        for (int row = 0; row < levelData.MapSize.y; row++)
        {
            // Draw the row.
            for (int col = 0; col < levelData.MapSize.x; col++)
            {
                var points = HexToPoints(HexHeight, row, col);
                pointForHexes[row, col] = points;
            }
        }
        return pointForHexes;
    }

    private static void DrawHexGrid(System.Drawing.Graphics gr, PointF[,][] pointForHexes, Declarations.LevelData levelData)
    {
        for (int row = 0; row < levelData.MapSize.y; row++)
        {
            for (int col = 0; col < levelData.MapSize.x; col++)
            {
                var points = pointForHexes[row , col];

                var brush = Brushes.White;
                switch (levelData.Map[row, col])
                {
                    case Declarations.TileType.Spawn:
                        brush = Brushes.Gray;
                        break;
                    case Declarations.TileType.Objective:
                        brush = Brushes.Yellow;
                        break;
                    case Declarations.TileType.Grass:
                        brush = Brushes.Green;
                        break;
                    case Declarations.TileType.Path:
                        brush = Brushes.Brown;
                        break;
                    case Declarations.TileType.Environment:
                        brush = Brushes.DarkGreen;
                        break;
                    default:
                        break;
                }
                gr.FillPolygon(brush, points);
            }
        }

    }
    
    private static PointF[] HexToPoints(int HexHeight, float row, float col)
    {
        var yoffset = HexHeight * 2f;
        float h = ((float)HexHeight / 2) * (float)Math.Sqrt(3);
        // Start with the leftmost corner of the upper left hexagon.
        float y = row * (HexHeight * 1.5f) + yoffset;//upmost point
        float offset = 0;
        if (row % 2 == 1)
        {
            offset = h;
        }
        float x = col * h * 2 + offset;//leftmost point

        // Generate the points.
        return new PointF[]
            {
                    new PointF(x + h, y),//up
                    new PointF(x, y - (HexHeight / 2)),//up left
                    new PointF(x, y - (HexHeight * 1.5f)),//down left
                    new PointF(x + h, y - HexHeight * 2),//down
                    new PointF(x + (h * 2), y - (HexHeight * 1.5f)),//down right
                    new PointF(x + (h * 2), y - (HexHeight / 2)),//up right
            };
    }
}
