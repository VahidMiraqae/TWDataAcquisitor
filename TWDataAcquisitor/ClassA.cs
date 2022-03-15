using System;
using System.Drawing;
using System.Windows.Forms;
using TWDataAcquisitor;

public static class ClassA
{
    public static DateTime StartOfDay(this DateTime datetime)
    {
        return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    private static DateTime? Prompt(byte[] imageBytes, string text)
    {
        var position = Cursor.Position;

        DateTime? dtn = null;

        while (dtn == null)
        {
            if (DateTime.TryParse(text, out var dt))
            {
                dtn = dt;
                break;
            }
            else
            {
                var win = new EditDateTimeWindow(imageBytes, text);
                win.ShowDialog();
                text = win.EditedText;
            }
        }

        Cursor.Position = position;
        LeftMouseClick(position.X, position.Y);

        return dtn;
    }



    public static DateTime? TextToDateTime(string lastDatetimeStr, byte[] imageBytes)
    {
        DateTime? lastDatetime = null;

        if (DateTime.TryParse(lastDatetimeStr, out var dtt))
        {
            lastDatetime = dtt;
        }

        if (lastDatetime == null)
        {
            lastDatetime = Prompt(imageBytes, lastDatetimeStr);
            System.Threading.Thread.Sleep(250);
        }

        return lastDatetime;
    }


    //This is a replacement for Cursor.Position in WinForms
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern bool SetCursorPos(int x, int y);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    public const int MOUSEEVENTF_LEFTDOWN = 0x02;
    public const int MOUSEEVENTF_LEFTUP = 0x04;

    //This simulates a left mouse click
    public static void LeftMouseClick(int xpos, int ypos)
    {
        SetCursorPos(xpos, ypos);
        mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
    }


}