namespace WindowsHook.Impl
{
    
    internal enum WindowsEvents
    {
        WM_APP = 0x8000,
        WM_SETTEXT = 0xc,
        Message = WM_APP,
        Activating = Message + 1,
        TitleChanging = WM_SETTEXT,
        IconChanging = Message + 3,
        Closed = Message + 4,
        Maximized = Message + 5,
        SizeChanging = Message + 6,
        LocationChanging = Message + 7,
        ManualUpdate = Message + 8,
        DeActivating = Message + 9,
        VisibleChanging = Message + 10,
        EnabledChanging = Message + 11,
        SizeMoveBegins = Message + 12,
        SizeMoveEnds = Message + 13,
        SysCommandDrag = Message + 14,
        WindowDetected = Message + 15
    }
}
