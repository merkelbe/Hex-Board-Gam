using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NNetTut
{
    class GameBoard
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //// FIELDS
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // View 
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        int x;
        int y;

        List<double> zoomOptions = new List<double>(){.05,.1,.15,.2,.25,.3,.35,.4,.45,.5,.55,.6,.65,.7,.75,.8,.85,.9,.95,1,
            1.05,1.1,1.15,1.2,1.25,1.3,1.35,1.4,1.45,1.5,1.55,1.6,1.65,1.7,1.75,1.8,1.85,1.9,1.95,2};
        double currentZoomOption;

        int selectedSpaceRowIndex;
        int selectedSpaceColIndex;

        int baseHexSpaceWidth;
        int baseHexSpaceHeight;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Data
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal enum BoardSpaceOffsetType { ToggleWithUpStart, ToggleWithDownStart, Ascending, Descending }
        BoardSpaceOffsetType boardSpaceOffsetType;
               
        BoardSpace[,] gameBoard;
        int[,] gameBoardXCoords;
        int[,] gameBoardYCoords;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //// CONSTRUCTOR
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal GameBoard(BoardSpaceOffsetType _boardSpaceOffsetType, int _x, int _y, int _hexSpaceWidth, int _hexSpaceHeight)
        {

            ClearBoardOfSpaces();
            boardSpaceOffsetType = _boardSpaceOffsetType;
            x = _x;
            y = _y;
            baseHexSpaceWidth = _hexSpaceWidth;
            baseHexSpaceHeight = _hexSpaceHeight;
            currentZoomOption = .2;

            selectedSpaceRowIndex = 0;
            selectedSpaceColIndex = 0;

            updateZoomOfBoardSpaces();
            setBoardXYCoords();
            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //// METHODS
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Data Changing Methods

        internal void AddSpace(BoardSpace _boardGameSpace)
        {
            int rowIndex = _boardGameSpace.RowIndex;
            int colIndex = _boardGameSpace.ColIndex;
            if (colIndex >= gameBoard.GetLength(1))
            {
                growBoardHorizontally(ref gameBoard);
                growBoardHorizontally(ref gameBoardXCoords);
                growBoardHorizontally(ref gameBoardYCoords);
            }
            if (rowIndex >= gameBoard.GetLength(0))
            {
                growBoardVertically(ref gameBoard);
                growBoardVertically(ref gameBoardXCoords);
                growBoardVertically(ref gameBoardYCoords);
            }
            gameBoard[rowIndex, colIndex] = _boardGameSpace;
            //Primarily used so the selected space is shown on start up.
            if (rowIndex == selectedSpaceRowIndex && colIndex == selectedSpaceColIndex) gameBoard[rowIndex, colIndex].Selected = true;
            setBoardXYCoords();
            _boardGameSpace.UpdateZoom(currentZoomOption);
        }

        private void growBoardHorizontally<T>(ref T[,] _board)
        {
            T[,] newBoard = new T[_board.GetLength(0), _board.GetLength(1) * 2];
            for (int rowNum = 0; rowNum < _board.GetLength(0); ++rowNum)
            {
                for (int colNum = 0; colNum < _board.GetLength(1); ++colNum)
                {
                    if (_board[rowNum, colNum] != null)
                    {
                        newBoard[rowNum, colNum] = _board[rowNum, colNum];
                    }
                }
            }
            _board = newBoard;
        }

        private void growBoardVertically<T>(ref T[,] _board)
        {
            T[,] newBoard = new T[_board.GetLength(0) * 2, _board.GetLength(1)];
            for (int rowNum = 0; rowNum < _board.GetLength(0); ++rowNum)
            {
                for (int colNum = 0; colNum < _board.GetLength(1); ++colNum)
                {
                    if (_board[rowNum, colNum] != null)
                    {
                        newBoard[rowNum, colNum] = _board[rowNum, colNum];
                    }
                }
            }
            _board = newBoard;
        }

        internal void ClearBoardOfSpaces()
        {
            gameBoard = new BoardSpace[1, 1];
            gameBoardXCoords = new int[1, 1];
            gameBoardYCoords = new int[1, 1];
        }
        
        #endregion

        #region Data Updating Methods

        private void setBoardXYCoords()
        {
            switch (boardSpaceOffsetType)
            {
                case BoardSpaceOffsetType.ToggleWithUpStart:
                    {
                        int offset = baseHexSpaceHeight / 2;
                        for (int colNum = 0; colNum < gameBoard.GetLength(1); ++colNum)
                        {
                            offset = offset == 0 ? baseHexSpaceHeight / 2 : 0;
                            for (int rowNum = 0; rowNum < gameBoard.GetLength(0); ++rowNum)
                            {
                                if (gameBoard[rowNum, colNum] != null)
                                {
                                    //int casting where it is to make uniform spaces between board pieces
                                    gameBoard[rowNum, colNum].X = x + colNum * (int)(baseHexSpaceWidth * 3 / 4 * currentZoomOption);
                                    gameBoard[rowNum, colNum].Y = y + rowNum * (int)(baseHexSpaceHeight * currentZoomOption) + (int)(offset * currentZoomOption);
                                }
                            }
                        }
                        break;
                    }
                case BoardSpaceOffsetType.ToggleWithDownStart:
                    {
                        int offset = 0;
                        for (int colNum = 0; colNum < gameBoard.GetLength(1); ++colNum)
                        {
                            offset = offset == 0 ? baseHexSpaceHeight / 2 : 0;
                            for (int rowNum = 0; rowNum < gameBoard.GetLength(0); ++rowNum)
                            {
                                if (gameBoard[rowNum, colNum] != null)
                                {
                                    //int casting where it is to make uniform spaces between board pieces
                                    gameBoard[rowNum, colNum].X = x + colNum * (int)(baseHexSpaceWidth * 3 / 4 * currentZoomOption);
                                    gameBoard[rowNum, colNum].Y = y + rowNum * (int)(baseHexSpaceHeight * currentZoomOption) + (int)(offset * currentZoomOption);
                                }
                            }
                        }
                        break;
                    }
                //case BoardSpaceOffsetType.Ascending:
                //    {

                //        break;
                //    }
                //case BoardSpaceOffsetType.Descending:
                //    {

                //        break;
                //    }
                default:
                    {
                        throw new Exception("Unknown board space offset type: " + boardSpaceOffsetType);
                    }
            }
        }

        private void updateZoomOfBoardSpaces()
        {
            for (int rowNum = 0; rowNum < gameBoard.GetLength(0); ++rowNum)
            {
                for (int colNum = 0; colNum < gameBoard.GetLength(1); ++colNum)
                {
                    if (gameBoard[rowNum, colNum] != null)
                    {
                        gameBoard[rowNum, colNum].UpdateZoom(currentZoomOption);
                    }
                }
            }
        }

        private void updateSelectedSpace()
        {
            for (int rowIndex = 0; rowIndex < gameBoard.GetLength(0); ++rowIndex)
            {
                for (int colIndex = 0; colIndex < gameBoard.GetLength(1); ++colIndex)
                {
                    if (gameBoard[rowIndex, colIndex] != null)
                    {
                        if (rowIndex == selectedSpaceRowIndex && colIndex == selectedSpaceColIndex)
                        {
                            gameBoard[rowIndex, colIndex].Selected = true;
                        }
                        else
                        {
                            gameBoard[rowIndex, colIndex].Selected = false;
                        }
                    }
                }
            }
        }

        #endregion

        #region Data Retrieving Methods

        internal BoardSpace GetClosestBoardSpace(Tuple<int, int> _coordinate)
        {
            double currentClosestDistance = double.MaxValue;
            BoardSpace currentClosestSpace = null;

            for (int row = 0; row < gameBoard.GetLength(0); ++row)
            {
                for (int col = 0; col < gameBoard.GetLength(1); ++col)
                {
                    if(gameBoard[row,col] != null)
                    {
                        double dist = getDistanceBetweenPoints(_coordinate, gameBoard[row, col].Center);
                        if (dist < currentClosestDistance)
                        {
                            currentClosestDistance = dist;
                            currentClosestSpace = gameBoard[row, col];
                        }
                    }
                }
            }
            return currentClosestSpace;
        }

        private double getDistanceBetweenPoints(Tuple<int, int> _point1, Tuple<int, int> _point2)
        {
            return Math.Sqrt(Math.Pow(_point1.Item1 - _point2.Item1, 2) + Math.Pow(_point1.Item2 - _point2.Item2, 2));
        }

        #endregion

        internal void Draw(SpriteBatch _spriteBatch)
        {
            for (int rowNum = 0; rowNum < gameBoard.GetLength(0); ++rowNum)
            {
                for (int colNum = 0; colNum < gameBoard.GetLength(1); ++colNum)
                {
                    if (gameBoard[rowNum, colNum] != null)
                    {
                        gameBoard[rowNum, colNum].Draw(_spriteBatch);
                    }
                }
            }
        }

        #region View Changing Methods

        internal void IncreaseZoom()
        {
            currentZoomOption = zoomOptions[Math.Min(zoomOptions.IndexOf(currentZoomOption)+1, zoomOptions.Count-1)];
            setBoardXYCoords();
            updateZoomOfBoardSpaces();
            
        }

        internal void DecreaseZoom()
        {
            currentZoomOption = zoomOptions[Math.Max(zoomOptions.IndexOf(currentZoomOption) - 1, 0)];
            setBoardXYCoords();
            updateZoomOfBoardSpaces();
        }

        internal void MoveSelectedSpaceRight()
        {
            switch(boardSpaceOffsetType)
            {
                case BoardSpaceOffsetType.ToggleWithUpStart:
                    {
                        if (selectedSpaceColIndex + 1 < gameBoard.GetLength(1))
                        {
                            if (gameBoard[selectedSpaceRowIndex , selectedSpaceColIndex+1] != null)
                            {
                                ++selectedSpaceColIndex;
                                updateSelectedSpace();
                            }
                            else
                            {
                                if (selectedSpaceRowIndex % 2 != 0)
                                {
                                    if (selectedSpaceRowIndex + 1 < gameBoard.GetLength(0))
                                    {
                                        if (gameBoard[selectedSpaceRowIndex + 1, selectedSpaceColIndex + 1] != null)
                                        {
                                            ++selectedSpaceRowIndex;
                                            ++selectedSpaceColIndex;
                                            updateSelectedSpace();
                                        }
                                    }
                                }
                                else
                                {
                                    if (selectedSpaceRowIndex - 1 >= 0)
                                    {
                                        if (gameBoard[selectedSpaceRowIndex - 1, selectedSpaceColIndex + 1] != null)
                                        {
                                            --selectedSpaceRowIndex;
                                            ++selectedSpaceColIndex;
                                            updateSelectedSpace();
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Behavior not implemented yet.");
                    }
            }
        }

        internal void MoveSelectedSpaceLeft()
        {
            switch (boardSpaceOffsetType)
            {
                case BoardSpaceOffsetType.ToggleWithUpStart:
                    {
                        if (selectedSpaceColIndex - 1 >= 0)
                        {
                            if (gameBoard[selectedSpaceRowIndex, selectedSpaceColIndex - 1] != null)
                            {
                                --selectedSpaceColIndex;
                                updateSelectedSpace();
                            }
                            else
                            {
                                if (selectedSpaceRowIndex % 2 != 0)
                                {
                                    if (selectedSpaceRowIndex + 1 < gameBoard.GetLength(0))
                                    {
                                        if (gameBoard[selectedSpaceRowIndex + 1, selectedSpaceColIndex - 1] != null)
                                        {
                                            ++selectedSpaceRowIndex;
                                            --selectedSpaceColIndex;
                                            updateSelectedSpace();
                                        }
                                    }
                                }
                                else
                                {
                                    if (selectedSpaceRowIndex - 1 >= 0)
                                    {
                                        if (gameBoard[selectedSpaceRowIndex - 1, selectedSpaceColIndex - 1] != null)
                                        {
                                            --selectedSpaceRowIndex;
                                            --selectedSpaceColIndex;
                                            updateSelectedSpace();
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Behavior not implemented yet.");
                    }
            }
        }

        internal void MoveSelectedSpaceUp()
        {
            switch (boardSpaceOffsetType)
            {
                case BoardSpaceOffsetType.ToggleWithUpStart:
                    {
                        if (selectedSpaceRowIndex - 1 >= 0)
                        {
                            if (gameBoard[selectedSpaceRowIndex - 1, selectedSpaceColIndex] != null)
                            {
                                --selectedSpaceRowIndex;
                                updateSelectedSpace();
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Behavior not implemented yet.");
                    }
            }
        }

        internal void MoveSelectedSpaceDown()
        {
            switch (boardSpaceOffsetType)
            {
                case BoardSpaceOffsetType.ToggleWithUpStart:
                    {
                        if (selectedSpaceRowIndex + 1 < gameBoard.GetLength(0))
                        {
                            if (gameBoard[selectedSpaceRowIndex + 1, selectedSpaceColIndex] != null)
                            {
                                ++selectedSpaceRowIndex;
                                updateSelectedSpace();
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Behavior not implemented yet.");
                    }
            }
        }

        internal void MoveSelectedSpace(int _row, int _col)
        {
            if (this.gameBoard[_row, _col] != null)
            {
                this.selectedSpaceRowIndex = _row;
                this.selectedSpaceColIndex = _col;
                this.updateSelectedSpace();
            }
        }

        internal void SelectPiece(int _rowIndex, int _colIndex)
        {
            gameBoard[_rowIndex, _colIndex].Selected = true;
        }

        internal void DeselectPiece(int _rowIndex, int _colIndex)
        {
            gameBoard[_rowIndex, _colIndex].Selected = false;
        }

        internal void ShiftBoardRight()
        {
            this.x += (int)(1200* currentZoomOption);
            setBoardXYCoords();
        }

        internal void ShiftBoardLeft()
        {
            this.x -= (int)(1200 * currentZoomOption);
            setBoardXYCoords();
        }

        internal void ShiftBoardDown()
        {
            this.y += (int)(800* currentZoomOption);
            setBoardXYCoords();
        }

        internal void ShiftBoardUp()
        {
            this.y -= (int)(800 * currentZoomOption);
            setBoardXYCoords();
        }

        #endregion

        private List<Tuple<int, int>> getConnectedSpaces(Tuple<int, int> _gameBoardSpace)
        {
            List<Tuple<int, int>> connectedSpaces = new List<Tuple<int, int>>();
            int row = _gameBoardSpace.Item1;
            int col = _gameBoardSpace.Item2;
            switch (boardSpaceOffsetType)
            {
                case BoardSpaceOffsetType.ToggleWithUpStart:
                    {
                        if (row - 1 >= 0 && gameBoard[row-1,col] != null)
                        {
                            connectedSpaces.Add(new Tuple<int, int>(row - 1, col));
                        }
                        if (row + 1 < gameBoard.GetLength(0) && gameBoard[row + 1, col] != null)
                        {
                            connectedSpaces.Add(new Tuple<int, int>(row + 1, col));
                        }
                        if (col % 2 == 0)
                        {
                            if (col - 1 >= 0)
                            {
                                if (row - 1 >= 0)
                                {
                                    if (gameBoard[row - 1, col - 1] != null)
                                    {
                                        connectedSpaces.Add(new Tuple<int, int>(row - 1, col - 1));
                                    }
                                }
                                if (gameBoard[row, col - 1] != null)
                                {
                                    connectedSpaces.Add(new Tuple<int, int>(row, col - 1));
                                }
                            }
                            if (col + 1 < gameBoard.GetLength(1))
                            {
                                if (row - 1 >= 0)
                                {
                                    if (gameBoard[row - 1, col + 1] != null)
                                    {
                                        connectedSpaces.Add(new Tuple<int, int>(row - 1, col + 1));
                                    }
                                }
                                if (gameBoard[row, col + 1] != null)
                                {
                                    connectedSpaces.Add(new Tuple<int, int>(row, col + 1));
                                }
                            }
                        }
                        else
                        {
                            if (gameBoard[row, col - 1] != null)
                            {
                                connectedSpaces.Add(new Tuple<int, int>(row, col - 1));
                            }
                            if (row + 1 < gameBoard.GetLength(0))
                            {
                                if (gameBoard[row + 1, col - 1] != null)
                                {
                                    connectedSpaces.Add(new Tuple<int, int>(row + 1, col - 1));
                                }
                            }
                            if (col + 1 < gameBoard.GetLength(1))
                            {
                                if (gameBoard[row, col + 1] != null)
                                {
                                    connectedSpaces.Add(new Tuple<int,int>(row,col+1));
                                }
                                if (row + 1 < gameBoard.GetLength(0))
                                {
                                    if (gameBoard[row + 1, col + 1] != null)
                                    {
                                        connectedSpaces.Add(new Tuple<int, int>(row + 1, col + 1));
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Current board game type is unsupported for funtion.");
                    }
            }
            return connectedSpaces;
        }

        internal void SelectConnectedSpaces(Tuple<int, int> _gameBoardSpace)
        {
            foreach (Tuple<int, int> boardSpaceCoords in getConnectedSpaces(_gameBoardSpace))
            {
                int row = boardSpaceCoords.Item1;
                int col = boardSpaceCoords.Item2;
                gameBoard[row, col].Selected = true;
            }
        }

        internal void SelectConnectedSpaces(BoardSpace _gameBoardSpace)
        {
            int row;
            int col;
            for (row = 0; row < gameBoard.GetLength(0); ++row)
            {
                for (col = 0; col < gameBoard.GetLength(1); ++col)
                {
                    if (gameBoard[row, col] != null)
                    {
                        if (gameBoard[row, col].X == _gameBoardSpace.X && gameBoard[row, col].Y == _gameBoardSpace.Y)
                        {
                            SelectConnectedSpaces(new Tuple<int, int>(row, col));
                            return;
                        }
                    }
                }
            }
            
        }
    }
}
