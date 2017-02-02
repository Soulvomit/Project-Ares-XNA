using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace TileEngine
{
    public static class ConversionHelper
    {
        #region Fields
        public static int TileWidth = 64;
        public static int TileHeight = 64;
        #endregion

        #region PointToCell
        /// <summary>
        /// Creates a 2D grid position from a pixel position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point PointToCell(Point position)
        {
            return new Point((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        public static Vector2 PointToCell(Vector2 position)
        {
            return new Vector2((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        public static Point VPointToCell(Vector2 position)
        {
            return new Point((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        public static Vector2 VPointToCell(Point position)
        {
            return new Vector2((int)(position.X / (float)TileWidth), (int)(position.Y / (float)TileHeight));
        }
        #endregion

        #region CellToPoint
        /// <summary>
        /// Converts a given cell to a point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 CellToPoint(Point point)
        {
            Vector2 vector = Vector2.Zero;
            vector.X = point.X * TileWidth;
            vector.Y = point.Y * TileHeight;
            return vector;
        }
        #endregion

        #region CellToCellCenterPoint
        /// <summary>
        /// Offsets the cell position to a cell center point.
        /// </summary>
        /// <param name="x">Cell X position.</param>
        /// <param name="y">Cell Y position</param>
        /// <param name="spriteSize">Further offsets with a sprite size. Use to find the precise spot for placing a sprite in a cell. Set to Vector2.Zero if not needed</param>
        /// <returns></returns>
        public static Vector2 CellToCellCenterPoint(int x, int y, Vector2 spriteSize)
        {
            if (spriteSize != Vector2.Zero)
            {
                return CellToPoint(new Point(x, y)) + 
                                                    new Vector2
                                                    (
                                                        (ConversionHelper.TileWidth / 2) - (spriteSize.X / 2),
                                                        (ConversionHelper.TileHeight / 2) - (spriteSize.Y / 2)
                                                    );
            }
            else
            {
                return CellToPoint(new Point(x, y)) + 
                                                    new Vector2
                                                    (
                                                        (ConversionHelper.TileWidth / 2),
                                                        (ConversionHelper.TileHeight / 2)
                                                    );
            }
        }
        #endregion

        #region PointToCellCenterPoint
        /// <summary>
        /// Offsets the point position to a cell center point.
        /// </summary>
        /// <param name="x">Cell X position.</param>
        /// <param name="y">Cell Y position</param>
        /// <param name="spriteSize">Further offsets with a sprite size. Use to find the precise spot for placing a sprite in a cell. Set to Vector2.Zero if not needed</param>
        /// <returns></returns>
        public static Vector2 PointToCellCenterPoint(float x, float y, Vector2 spriteSize)
        {
            Point cell = VPointToCell(new Vector2(x, y));

            if (spriteSize != Vector2.Zero)
            {
                return CellToPoint(cell) +
                                        new Vector2
                                        (
                                            (ConversionHelper.TileWidth / 2) - (spriteSize.X / 2),
                                            (ConversionHelper.TileHeight / 2) - (spriteSize.Y / 2)
                                        );
            }
            else
            {
                return CellToPoint(cell) +
                                        new Vector2
                                        (
                                            (ConversionHelper.TileWidth / 2),
                                            (ConversionHelper.TileHeight / 2)
                                        );
            }
        }
        #endregion

        #region PointArrToCellCenter
        public static Vector2[] PointArrToCellCenter(Vector2[] arr, Vector2 size)
        {
            Vector2[] newArr = new Vector2[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                newArr[i] = PointToCellCenterPoint(arr[i].X, arr[i].Y, size);
            }
            return newArr;
        }
        #endregion

        #region PointLinkedToCellCenter
        public static LinkedList<Vector2> PointLinkedToCellCenter(LinkedList<Vector2> list, Vector2 size)
        {
            LinkedList<Vector2> newList = new LinkedList<Vector2>();

            foreach (Vector2 vector in list)
            {
                newList.AddLast(PointToCellCenterPoint(vector.X, vector.Y, size));
            }

            return newList;
        }
        #endregion

        #region RectangleForCell
        public static Rectangle RectangleForCell(Point cell)
        {
            return new Rectangle(cell.X * TileWidth, cell.Y * TileHeight, TileWidth, TileHeight);
        }
        #endregion

        #region ToVector2
        public static Vector2 ToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }
        #endregion

        #region ToPoint
        public static Point ToPoint(Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
        #endregion

        #region GetIntersectionDepth
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            //calculate half sizes
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            //calculate centers
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            //calculate current and minimum-non-intersecting distances between centers
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            //if we are not intersecting at all, return (0, 0)
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            //calculate and return intersection depths
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
        #endregion
    }
}
