using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
#if WINDOWS
using System.Runtime.Serialization.Formatters.Binary;
#elif XBOX
using System.Xml;
#endif

namespace Tile_Engine
{
    public class TileMap
    {
        #region Declarations
        public const int TileWidth = 48;
        public const int TileHeight = 48;
        public const int MapWidth = 160;
        public const int MapHeight = 12;
        public const int MapLayers = 3;
        private const int skyTile = 2;
        private static float rotation = 0f;
        private static float r;

        private MapSquare[,] mapCopy;

        static private MapSquare[,] mapCells =
            new MapSquare[MapWidth, MapHeight];

        public static bool EditorMode = false;

        public static SpriteFont spriteFont;
        static private Texture2D tileSheet;
        #endregion

        #region Initialization
        public static void Initialize(Texture2D tileTexture)
        {
            tileSheet = tileTexture;

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    for (int z = 0; z < MapLayers; z++)
                    {
                        mapCells[x, y] = new MapSquare(skyTile, 0, 0, "", true);
                    }
                }
            }
        }
        #endregion

        #region Tile and Tile Sheet Handling
        public static int TilesPerRow
        {
            get { return tileSheet.Width / TileWidth; }
        }

        public static Rectangle TileSourceRectangle(int tileIndex)
        {
            return new Rectangle(
                (tileIndex % TilesPerRow) * TileWidth,
                (tileIndex / TilesPerRow) * TileHeight,
                TileWidth,
                TileHeight);
        }
        #endregion

        #region Information about Map Cells
        static public int GetCellByPixelX(int pixelX)
        {
            return pixelX / TileWidth;
        }

        static public int GetCellByPixelY(int pixelY)
        {
            return pixelY / TileHeight;
        }

        static public Vector2 GetCellByPixel(Vector2 pixelLocation)
        {
            return new Vector2(
                GetCellByPixelX((int)pixelLocation.X),
                GetCellByPixelY((int)pixelLocation.Y));
        }

        static public Vector2 GetCellCenter(int cellX, int cellY)
        {
            return new Vector2(
                (cellX * TileWidth) + (TileWidth / 2),
                (cellY * TileHeight) + (TileHeight / 2));
        }

        static public Vector2 GetCellCenter(Vector2 cell)
        {
            return GetCellCenter(
                (int)cell.X,
                (int)cell.Y);
        }

        static public Rectangle CellWorldRectangle(int cellX, int cellY)
        {
            return new Rectangle(
                cellX * TileWidth,
                cellY * TileHeight,
                TileWidth,
                TileHeight);
        }

        static public Rectangle CellWorldRectangle(Vector2 cell)
        {
            return CellWorldRectangle(
                (int)cell.X,
                (int)cell.Y);
        }

        static public Rectangle CellScreenRectangle(int cellX, int cellY)
        {
            return Camera.WorldToScreen(CellWorldRectangle(cellX, cellY));
        }

        static public Rectangle CellSreenRectangle(Vector2 cell)
        {
            return CellScreenRectangle((int)cell.X, (int)cell.Y);
        }

        static public bool CellIsPassable(int cellX, int cellY)
        {
            MapSquare square = GetMapSquareAtCell(cellX, cellY);

            if (square == null)
                return false;
            else
                return square.Passable;
        }

        static public bool CellIsPassable(Vector2 cell)
        {
            return CellIsPassable((int)cell.X, (int)cell.Y);
        }

        static public bool CellIsPassableByPixel(Vector2 pixelLocation)
        {
            return CellIsPassable(
                GetCellByPixelX((int)pixelLocation.X),
                GetCellByPixelY((int)pixelLocation.Y));
        }

        static public string CellCodeValue(int cellX, int cellY)
        {
            MapSquare square = GetMapSquareAtCell(cellX, cellY);

            if (square == null)
                return "";
            else
                return square.CodeValue;
        }

        static public string CellCodeValue(Vector2 cell)
        {
            return CellCodeValue((int)cell.X, (int)cell.Y);
        }
        #endregion

        #region Information about MapSquare objects
        static public MapSquare GetMapSquareAtCell(int tileX, int tileY)
        {
            if ((tileX >= 0) && (tileX < MapWidth) &&
                (tileY >= 0) && (tileY < MapHeight))
            {
                return mapCells[tileX, tileY];
            }
            else
            {
                return null;
            }
        }

        static public void SetMapSquareAtCell(
            int tileX,
            int tileY,
            MapSquare tile)
        {
            if ((tileX >= 0) && (tileX < MapWidth) &&
                (tileY >= 0) && (tileY < MapHeight))
            {
                mapCells[tileX, tileY] = tile;
            }
        }

        static public void SetTileAtCell(
            int tileX,
            int tileY,
            int layer,
            int tileIndex)
        {
            if ((tileX >= 0) && (tileX < MapWidth) &&
                (tileY >= 0) && (tileY < MapHeight))
            {
                mapCells[tileX, tileY].LayerTiles[layer] = tileIndex;
            }
        }

        static public MapSquare GetMapSquareAtPixel(int pixelX, int pixelY)
        {
            return GetMapSquareAtCell(
                GetCellByPixelX(pixelX),
                GetCellByPixelY(pixelY));
        }

        static public MapSquare GetMapSquareAtPixel(Vector2 pixelLocation)
        {
            return GetMapSquareAtPixel(
                (int)pixelLocation.X,
                (int)pixelLocation.Y);
        }

        #endregion

        #region Loading and Saving Maps
        public void SaveMap(FileStream fileStream)
        {
//#if WINDOWS
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(fileStream, mapCells);
            //fileStream.Close();
//#endif
            int k = 0;
            MapSquare[] lala = new MapSquare[MapWidth * MapHeight];
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {// Grab each value one at a time, and place it inside the SD array  
                    lala[k] = mapCells[i, j];
                    k++;
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(MapSquare[]));
            serializer.Serialize(fileStream, lala);
            fileStream.Close();
        }

        public static void LoadMap(FileStream fileStream)
        {
            try
            {
//#if WINDOWS
//                BinaryFormatter formatter = new BinaryFormatter();
//                mapCells = (MapSquare[,])formatter.Deserialize(fileStream);
//                fileStream.Close();
//#elif XBOX

                XmlSerializer serializer = new XmlSerializer(typeof(MapSquare[]));
                MapSquare[] lala = new MapSquare[MapWidth * MapHeight];
                lala = (MapSquare[])serializer.Deserialize(fileStream);
                int k = 0;
                for (int i = 0; i < MapWidth; i++)
                {
                    for (int j = 0; j < MapHeight; j++)
                    {// Going backwards now  
                        mapCells[i, j] = lala[k];
                        k++;
                    }
                }  
                fileStream.Close();

//#endif
            }
            catch
            {
                ClearMap();
            }
        }

        public static void ClearMap()
        {
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                    for (int z = 0; z < MapLayers; z++)
                    {
                        mapCells[x, y] = new MapSquare(2, 0, 0, "", true);
                    }
        }
        #endregion

        #region Drawing
        static public void Draw(SpriteBatch spriteBatch)
        {
            int startX = GetCellByPixelX((int)Camera.Position.X);
            int endX = GetCellByPixelX((int)Camera.Position.X +
                  Camera.ViewPortWidth);

            int startY = GetCellByPixelY((int)Camera.Position.Y);
            int endY = GetCellByPixelY((int)Camera.Position.Y +
                      Camera.ViewPortHeight);

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                {
                    for (int z = 0; z < MapLayers; z++)
                    {
                        if ((x >= 0) && (y >= 0) &&
                            (x < MapWidth) && (y < MapHeight))
                        {
                            rotation += 0.01f;
                            float circle = MathHelper.Pi * 2;
                            rotation = rotation % circle;
                            

                            if (mapCells[x, y].Rotating)
                                r = rotation;
                            else r = 0;


                            spriteBatch.Draw(
                              tileSheet,
                              CellScreenRectangle(x, y),
                              TileSourceRectangle(mapCells[x, y].LayerTiles[z]),
                              Color.White,
                              r,
                              Vector2.Zero,
                              SpriteEffects.None,
                              1f - ((float)z * 0.1f));
                        }
                    }

                    if (EditorMode)
                    {
                        DrawEditModeItems(spriteBatch, x, y);
                    }

                }
        }

        public static void DrawEditModeItems(
            SpriteBatch spriteBatch,
            int x,
            int y)
        {
            if ((x < 0) || (x >= MapWidth) ||
                (y < 0) || (y >= MapHeight))
                return;

            if (!CellIsPassable(x, y))
            {
                spriteBatch.Draw(
                                tileSheet,
                                CellScreenRectangle(x, y),
                                TileSourceRectangle(1),
                                new Color(255, 0, 0, 80),
                                0.0f,
                                Vector2.Zero,
                                SpriteEffects.None,
                                0.0f);
            }

            if (mapCells[x, y].CodeValue != "")
            {
                Rectangle screenRect = CellScreenRectangle(x, y);

                spriteBatch.DrawString(
                    spriteFont,
                    mapCells[x, y].CodeValue,
                    new Vector2(screenRect.X, screenRect.Y),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0.0f);
            }
        }

        public MapSquare[,] getMapCells()
        {
            return mapCells;
        }
        #endregion

    }
}
