namespace Stimulsoft.System.Windows.Forms
{

    public enum Keys
    {
        KeyCode = 0x0000FFFF,
        Modifiers = unchecked((int)0xFFFF0000),
        None = 0x00,
        LButton = 0x01,
        RButton = 0x02,
        Cancel = 0x03,
        MButton = 0x04,
        XButton1 = 0x05,
        XButton2 = 0x06,
        Back = 0x08,
        Tab = 0x09,
        LineFeed = 0x0A,
        Clear = 0x0C,
        Return = 0x0D,
        Enter = Return,
        ShiftKey = 0x10,
        ControlKey = 0x11,
        Menu = 0x12,
        Pause = 0x13,
        Capital = 0x14,
        CapsLock = 0x14,
        KanaMode = 0x15,
        HanguelMode = 0x15,
        HangulMode = 0x15,
        JunjaMode = 0x17,
        FinalMode = 0x18,
        HanjaMode = 0x19,
        KanjiMode = 0x19,
        Escape = 0x1B,
        IMEConvert = 0x1C,
        IMENonconvert = 0x1D,
        IMEAccept = 0x1E,
        IMEAceept = IMEAccept,
        IMEModeChange = 0x1F,
        Space = 0x20,
        Prior = 0x21,
        PageUp = Prior,
        Next = 0x22,
        PageDown = Next,
        End = 0x23,
        Home = 0x24,
        Left = 0x25,
        Up = 0x26,
        Right = 0x27,
        Down = 0x28,
        Select = 0x29,
        Print = 0x2A,
        Execute = 0x2B,
        Snapshot = 0x2C,
        PrintScreen = Snapshot,
        Insert = 0x2D,
        Delete = 0x2E,
        Help = 0x2F,
        D0 = 0x30, // 0
        D1 = 0x31, // 1
        D2 = 0x32, // 2
        D3 = 0x33, // 3
                   /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D4"]/*' />


        ///       The 4 key.


        D4 = 0x34, // 4
                   /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D5"]/*' />


        ///       The 5 key.


        D5 = 0x35, // 5
                   /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D6"]/*' />


        ///       The 6 key.


        D6 = 0x36, // 6
                   /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D7"]/*' />


        ///       The 7 key.


        D7 = 0x37, // 7
                   /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D8"]/*' />


        ///       The 8 key.


        D8 = 0x38, // 8
                   /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D9"]/*' />


        ///       The 9 key.


        D9 = 0x39, // 9
                   /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.A"]/*' />


        ///       The A key.


        A = 0x41,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.B"]/*' />


        ///       The B key.


        B = 0x42,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.C"]/*' />


        ///       The C key.


        C = 0x43,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.D"]/*' />


        ///       The D key.


        D = 0x44,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.E"]/*' />


        ///       The E key.


        E = 0x45,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F"]/*' />


        ///       The F key.


        F = 0x46,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.G"]/*' />


        ///       The G key.


        G = 0x47,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.H"]/*' />


        ///       The H key.


        H = 0x48,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.I"]/*' />


        ///       The I key.


        I = 0x49,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.J"]/*' />


        ///       The J key.


        J = 0x4A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.K"]/*' />


        ///       The K key.


        K = 0x4B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.L"]/*' />


        ///       The L key.


        L = 0x4C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.M"]/*' />


        ///       The M key.


        M = 0x4D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.N"]/*' />


        ///       The N key.


        N = 0x4E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.O"]/*' />


        ///       The O key.


        O = 0x4F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.P"]/*' />


        ///       The P key.


        P = 0x50,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Q"]/*' />


        ///       The Q key.


        Q = 0x51,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.R"]/*' />


        ///       The R key.


        R = 0x52,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.S"]/*' />


        ///       The S key.


        S = 0x53,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.T"]/*' />


        ///       The T key.


        T = 0x54,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.U"]/*' />


        ///       The U key.


        U = 0x55,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.V"]/*' />


        ///       The V key.


        V = 0x56,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.W"]/*' />


        ///       The W key.


        W = 0x57,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.X"]/*' />


        ///       The X key.


        X = 0x58,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Y"]/*' />


        ///       The Y key.


        Y = 0x59,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Z"]/*' />


        ///       The Z key.


        Z = 0x5A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LWin"]/*' />


        ///       The left Windows logo key (Microsoft Natural Keyboard).


        LWin = 0x5B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RWin"]/*' />


        ///       The right Windows logo key (Microsoft Natural Keyboard).


        RWin = 0x5C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Apps"]/*' />


        ///       The Application key (Microsoft Natural Keyboard).


        Apps = 0x5D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Sleep"]/*' />


        ///       The Computer Sleep key.


        Sleep = 0x5F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad0"]/*' />


        ///       The 0 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad0 = 0x60,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad1"]/*' />


        ///       The 1 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad1 = 0x61,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad2"]/*' />


        ///       The 2 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad2 = 0x62,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad3"]/*' />


        ///       The 3 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad3 = 0x63,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad4"]/*' />


        ///       The 4 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad4 = 0x64,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad5"]/*' />


        ///       The 5 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad5 = 0x65,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad6"]/*' />


        ///       The 6 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad6 = 0x66,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad7"]/*' />


        ///       The 7 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad7 = 0x67,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad8"]/*' />


        ///       The 8 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad8 = 0x68,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumPad9"]/*' />


        ///       The 9 key on the numeric keypad.


        // PM team has reviewed and decided on naming changes already

        NumPad9 = 0x69,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Multiply"]/*' />


        ///       The Multiply key.


        Multiply = 0x6A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Add"]/*' />


        ///       The Add key.


        Add = 0x6B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Separator"]/*' />


        ///       The Separator key.


        Separator = 0x6C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Subtract"]/*' />


        ///       The Subtract key.


        Subtract = 0x6D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Decimal"]/*' />


        ///       The Decimal key.


        Decimal = 0x6E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Divide"]/*' />


        ///       The Divide key.


        Divide = 0x6F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F1"]/*' />


        ///       The F1 key.


        F1 = 0x70,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F2"]/*' />


        ///       The F2 key.


        F2 = 0x71,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F3"]/*' />


        ///       The F3 key.


        F3 = 0x72,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F4"]/*' />


        ///       The F4 key.


        F4 = 0x73,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F5"]/*' />


        ///       The F5 key.


        F5 = 0x74,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F6"]/*' />


        ///       The F6 key.


        F6 = 0x75,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F7"]/*' />


        ///       The F7 key.


        F7 = 0x76,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F8"]/*' />


        ///       The F8 key.


        F8 = 0x77,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F9"]/*' />


        ///       The F9 key.


        F9 = 0x78,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F10"]/*' />


        ///       The F10 key.


        F10 = 0x79,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F11"]/*' />


        ///       The F11 key.


        F11 = 0x7A,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F12"]/*' />


        ///       The F12 key.


        F12 = 0x7B,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F13"]/*' />


        ///       The F13 key.


        F13 = 0x7C,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F14"]/*' />


        ///       The F14 key.


        F14 = 0x7D,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F15"]/*' />


        ///       The F15 key.


        F15 = 0x7E,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F16"]/*' />


        ///       The F16 key.


        F16 = 0x7F,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F17"]/*' />


        ///       The F17 key.


        F17 = 0x80,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F18"]/*' />


        ///       The F18 key.


        F18 = 0x81,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F19"]/*' />


        ///       The F19 key.


        F19 = 0x82,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F20"]/*' />


        ///       The F20 key.


        F20 = 0x83,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F21"]/*' />


        ///       The F21 key.


        F21 = 0x84,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F22"]/*' />


        ///       The F22 key.


        F22 = 0x85,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F23"]/*' />


        ///       The F23 key.


        F23 = 0x86,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.F24"]/*' />


        ///       The F24 key.


        F24 = 0x87,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NumLock"]/*' />


        ///       The NUM LOCK key.


        // PM team has reviewed and decided on naming changes already

        NumLock = 0x90,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Scroll"]/*' />


        ///       The SCROLL LOCK key.


        Scroll = 0x91,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LShiftKey"]/*' />


        ///       The left SHIFT key.


        LShiftKey = 0xA0,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RShiftKey"]/*' />


        ///       The right SHIFT key.


        RShiftKey = 0xA1,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LControlKey"]/*' />


        ///       The left CTRL key.


        LControlKey = 0xA2,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RControlKey"]/*' />


        ///       The right CTRL key.


        RControlKey = 0xA3,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LMenu"]/*' />


        ///       The left ALT key.


        LMenu = 0xA4,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.RMenu"]/*' />


        ///       The right ALT key.


        RMenu = 0xA5,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserBack"]/*' />


        ///       The Browser Back key.


        BrowserBack = 0xA6,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserForward"]/*' />


        ///       The Browser Forward key.


        BrowserForward = 0xA7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserRefresh"]/*' />


        ///       The Browser Refresh key.


        BrowserRefresh = 0xA8,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserStop"]/*' />


        ///       The Browser Stop key.


        BrowserStop = 0xA9,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserSearch"]/*' />


        ///       The Browser Search key.


        BrowserSearch = 0xAA,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserFavorites"]/*' />


        ///       The Browser Favorites key.


        BrowserFavorites = 0xAB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.BrowserHome"]/*' />


        ///       The Browser Home key.


        BrowserHome = 0xAC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.VolumeMute"]/*' />


        ///       The Volume Mute key.


        VolumeMute = 0xAD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.VolumeDown"]/*' />


        ///       The Volume Down key.


        VolumeDown = 0xAE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.VolumeUp"]/*' />


        ///       The Volume Up key.


        VolumeUp = 0xAF,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaNextTrack"]/*' />


        ///       The Media Next Track key.


        MediaNextTrack = 0xB0,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaPreviousTrack"]/*' />


        ///       The Media Previous Track key.


        MediaPreviousTrack = 0xB1,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaStop"]/*' />


        ///       The Media Stop key.


        MediaStop = 0xB2,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.MediaPlayPause"]/*' />


        ///       The Media Play Pause key.


        MediaPlayPause = 0xB3,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LaunchMail"]/*' />


        ///       The Launch Mail key.


        LaunchMail = 0xB4,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.SelectMedia"]/*' />


        ///       The Select Media key.


        SelectMedia = 0xB5,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LaunchApplication1"]/*' />


        ///       The Launch Application1 key.


        LaunchApplication1 = 0xB6,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.LaunchApplication2"]/*' />


        ///       The Launch Application2 key.


        LaunchApplication2 = 0xB7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemSemicolon"]/*' />


        ///       The Oem Semicolon key.


        OemSemicolon = 0xBA,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem1"]/*' />


        ///       The Oem 1 key.


        Oem1 = OemSemicolon,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oemplus"]/*' />


        ///       The Oem plus key.


        Oemplus = 0xBB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oemcomma"]/*' />


        ///       The Oem comma key.


        Oemcomma = 0xBC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemMinus"]/*' />


        ///       The Oem Minus key.


        OemMinus = 0xBD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemPeriod"]/*' />


        ///       The Oem Period key.


        OemPeriod = 0xBE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemQuestion"]/*' />


        ///       The Oem Question key.


        OemQuestion = 0xBF,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem2"]/*' />


        ///       The Oem 2 key.


        Oem2 = OemQuestion,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oemtilde"]/*' />


        ///       The Oem tilde key.


        Oemtilde = 0xC0,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem3"]/*' />


        ///       The Oem 3 key.


        Oem3 = Oemtilde,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemOpenBrackets"]/*' />


        ///       The Oem Open Brackets key.


        OemOpenBrackets = 0xDB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem4"]/*' />


        ///       The Oem 4 key.


        Oem4 = OemOpenBrackets,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemPipe"]/*' />


        ///       The Oem Pipe key.


        OemPipe = 0xDC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem5"]/*' />


        ///       The Oem 5 key.


        Oem5 = OemPipe,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemCloseBrackets"]/*' />


        ///       The Oem Close Brackets key.


        OemCloseBrackets = 0xDD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem6"]/*' />


        ///       The Oem 6 key.


        Oem6 = OemCloseBrackets,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemQuotes"]/*' />


        ///       The Oem Quotes key.


        OemQuotes = 0xDE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem7"]/*' />


        ///       The Oem 7 key.


        Oem7 = OemQuotes,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem8"]/*' />


        ///       The Oem8 key.


        Oem8 = 0xDF,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemBackslash"]/*' />


        ///       The Oem Backslash key.


        OemBackslash = 0xE2,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Oem102"]/*' />


        ///       The Oem 102 key.


        Oem102 = OemBackslash,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.ProcessKey"]/*' />


        ///       The PROCESS KEY key.


        ProcessKey = 0xE5,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Packet"]/*' />


        ///       The Packet KEY key.


        Packet = 0xE7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Attn"]/*' />


        ///       The ATTN key.


        Attn = 0xF6,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Crsel"]/*' />


        ///       The CRSEL key.


        Crsel = 0xF7,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Exsel"]/*' />


        ///       The EXSEL key.


        Exsel = 0xF8,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.EraseEof"]/*' />


        ///       The ERASE EOF key.


        EraseEof = 0xF9,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Play"]/*' />


        ///       The PLAY key.


        Play = 0xFA,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Zoom"]/*' />


        ///       The ZOOM key.


        Zoom = 0xFB,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.NoName"]/*' />


        ///       A constant reserved for future use.


        NoName = 0xFC,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Pa1"]/*' />


        ///       The PA1 key.


        Pa1 = 0xFD,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.OemClear"]/*' />


        ///       The CLEAR key.


        OemClear = 0xFE,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Shift"]/*' />


        ///       The SHIFT modifier key.


        Shift = 0x00010000,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Control"]/*' />


        ///       The
        ///       CTRL modifier key.
        ///


        Control = 0x00020000,
        /// <include file='doc\Keys.uex' path='docs/doc[@for="Keys.Alt"]/*' />


        ///       The ALT modifier key.
        ///


        Alt = 0x00040000
    }
}
