using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace NNetTut
{
    class MouseFilter
    {
        internal bool LeftDoubleClick
        {
            get { return LeftButtonInfo.DoubleClick; }
        }
        internal bool LeftSingleClick
        {
            get { return LeftButtonInfo.SingleClick; }
        }
        internal bool RightDoubleClick
        {
            get { return RightButtonInfo.DoubleClick; }
        }
        internal bool RightSingleClick
        {
            get { return RightButtonInfo.SingleClick; }
        }
        internal Tuple<int, int> LeftStartingCoords
        {
            get { return LeftButtonInfo.StartingCoordinates; }
        }
        internal Tuple<int, int> LeftEndingCoordinates
        {
            get { return LeftButtonInfo.EndingCoordinates; }
        }
        internal Tuple<int, int> RightStartingCoords
        {
            get { return RightButtonInfo.StartingCoordinates; }
        }
        internal Tuple<int, int> RightEndingCoords
        {
            get { return RightButtonInfo.EndingCoordinates; }
        }
        //TODO make this shit private yo!
        internal LeftClickInfo LeftButtonInfo;
        RightClickInfo RightButtonInfo;

        internal MouseFilter()
        {
            LeftButtonInfo = new LeftClickInfo();
            RightButtonInfo = new RightClickInfo();
        }

        internal void Update(MouseState _mouseState)
        {
            LeftButtonInfo.Update(_mouseState);
            RightButtonInfo.Update(_mouseState);
        }
    }

    //TODO Set private.  Made public for debugging.
    internal abstract class ButtonInfo
    {
        bool lastUpdateIsDown;
        int stateChangeCount;
        internal bool DoubleClick;
        internal bool SingleClick;
        internal Tuple<int, int> StartingCoordinates;
        internal Tuple<int, int> EndingCoordinates;
        const int resetTimerInTicks = 10;
        //TODO set private.  Made public for debugging.
        internal int currentTimer;

        internal ButtonInfo()
        {

        }

        internal abstract bool isButtonDown(MouseState _mouseState);

        internal void Update(MouseState _mouseState)
        {
            bool currentIsDown = isButtonDown(_mouseState);

            //Start of tracking condition
            if (currentIsDown && !lastUpdateIsDown && stateChangeCount == 0)
            {
                stateChangeCount = 1;
                currentTimer = resetTimerInTicks;
                StartingCoordinates = new Tuple<int, int>(_mouseState.X, _mouseState.Y);
            }
            //Tracks changes in up/down states
            else if (currentIsDown != lastUpdateIsDown)
            {
                ++stateChangeCount;
                currentTimer += 5;
                //Ending coords of single click
                if (stateChangeCount == 2)
                {
                    EndingCoordinates = new Tuple<int, int>(_mouseState.X, _mouseState.Y);
                }
            }
            lastUpdateIsDown = currentIsDown;
            --currentTimer;
            SingleClick = false;
            DoubleClick = false;
            //End of tracking condition
            if (currentTimer <= 0 || stateChangeCount >= 4)
            {
                if (stateChangeCount >= 4)
                {
                    DoubleClick = true;
                    SingleClick = false;
                    //Ending coords of double click. Overrides single click ending coords.
                    EndingCoordinates = new Tuple<int, int>(_mouseState.X, _mouseState.Y);
                }
                else if (stateChangeCount >= 2)
                {
                    SingleClick = true;
                    DoubleClick = false;
                }
                stateChangeCount = 0;
                currentTimer = 0;
            }
        }
    }

    class LeftClickInfo : ButtonInfo
    {
        internal override bool isButtonDown(MouseState _mouseState)
        {
            return _mouseState.LeftButton == ButtonState.Pressed;
        }
    }

    class RightClickInfo : ButtonInfo
    {
        internal override bool isButtonDown(MouseState _mouseState)
        {
            return _mouseState.RightButton == ButtonState.Pressed;
        }
    }
}
