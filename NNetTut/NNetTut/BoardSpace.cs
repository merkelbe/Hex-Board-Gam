using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NNetTut
{
    class BoardSpace : GameObject
    {
        internal bool Selected;
        Sprite selectedSprite;
        internal int RowIndex;
        internal int ColIndex;

        internal Tuple<int, int> Center
        {
            get { return new Tuple<int, int>((destinationRectangle.X + destinationRectangle.Width / 2), (destinationRectangle.Y + destinationRectangle.Height / 2)); }
        }

        internal BoardSpace(int _x, int _y, Sprite _hexSprite,Sprite _selectedSprite, int _windowWidth, int _windowHeight, int _colIndex, int _rowIndex)
        {
            this.active = true;
            this.Selected = false;
            this.X = _x;
            this.Y = _y;
            this.sprite = _hexSprite;
            this.selectedSprite = _selectedSprite;
            this.RowIndex = _rowIndex;
            this.ColIndex = _colIndex;
            this.WINDOW_WIDTH = _windowWidth;
            this.WINDOW_HEIGHT = _windowHeight;
            this.destinationRectangle = new Rectangle(this.X, this.Y, this.sprite.Width, this.sprite.Height);
        }

        internal void Draw(SpriteBatch _spriteBatch)
        {
            if (this.active)
            {
                _spriteBatch.Draw(this.sprite.EntireImage, this.destinationRectangle, Color.GhostWhite);
            }
            if (this.Selected)
            {
                _spriteBatch.Draw(this.selectedSprite.EntireImage, this.destinationRectangle, Color.White);
            }
        }

        internal void UpdateXYCoords(int _x, int _y)
        {
            this.X = _x;
            this.Y = _y;
        }

        internal void UpdateZoom(double _zoom)
        {
            this.destinationRectangle = new Rectangle(this.X, this.Y, (int)(this.sprite.Width * _zoom), (int)(this.sprite.Height * _zoom));
        }
    }
}
