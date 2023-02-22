using System.Drawing;

namespace EZAutoclickerWPF.Mouse
{
    internal class Click
    {
        public static void DoLeftClick()
        {
            //Call the imported function with the cursor's current position
            Point mousePos = Extern.GetMousePosition();
            Extern.mouse_event((int)Extern.MouseAction.LeftDown | (int)Extern.MouseAction.LeftUp, (uint)mousePos.X, (uint)mousePos.Y, 0, 0);
        }

        public static void DoMiddleClick()
        {
            Point mousePos = Extern.GetMousePosition();
            Extern.mouse_event((int)Extern.MouseAction.MiddleDown | (int)Extern.MouseAction.MiddleUp, (uint)mousePos.X, (uint)mousePos.Y, 0, 0);
        }

        public static void DoMouseRightClick()
        {
            Point mousePos = Extern.GetMousePosition();
            Extern.mouse_event((int)Extern.MouseAction.RightDown | (int)Extern.MouseAction.RightUp, (uint)mousePos.X, (uint)mousePos.Y, 0, 0);
        }
    }
}