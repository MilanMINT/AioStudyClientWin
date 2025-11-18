using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace AioStudy.UI.WpfServices
{
    public class GlobalHotKeyService : IDisposable
    {
        // Windows API
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Modifier keys
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        // Virtual Key Codes
        private const uint VK_RETURN = 0x0D;    // Enter
        private const uint VK_BACK = 0x08;       // Backspace ✅
        private const uint VK_SPACE = 0x20;      // Space

        private const int WM_HOTKEY = 0x0312;

        private IntPtr _windowHandle;
        private HwndSource _source;

        // HotKey IDs
        private const int HOTKEY_ID_TOGGLE = 9000;
        private const int HOTKEY_ID_RESET = 9001;

        public event EventHandler? ToggleTimerRequested;
        public event EventHandler? ResetTimerRequested;

        public void Initialize(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);

            // Registriere Alt+Enter für Toggle
            RegisterHotKey(_windowHandle, HOTKEY_ID_TOGGLE, MOD_ALT, VK_RETURN);
            
            // Registriere Alt+Backspace für Reset ✅
            RegisterHotKey(_windowHandle, HOTKEY_ID_RESET, MOD_ALT, VK_BACK);

            System.Diagnostics.Debug.WriteLine("🎹 GlobalHotKeys registriert: Alt+Enter (Toggle), Alt+Backspace (Reset)");
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                
                switch (id)
                {
                    case HOTKEY_ID_TOGGLE:
                        System.Diagnostics.Debug.WriteLine("⌨️ Alt+Enter gedrückt - Toggle Timer");
                        ToggleTimerRequested?.Invoke(this, EventArgs.Empty);
                        handled = true;
                        break;
                        
                    case HOTKEY_ID_RESET:
                        System.Diagnostics.Debug.WriteLine("⌨️ Alt+Backspace gedrückt - Reset Timer");
                        ResetTimerRequested?.Invoke(this, EventArgs.Empty);
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, HOTKEY_ID_TOGGLE);
                UnregisterHotKey(_windowHandle, HOTKEY_ID_RESET);
                _source?.RemoveHook(HwndHook);
                
                System.Diagnostics.Debug.WriteLine("🎹 GlobalHotKeys deregistriert");
            }
        }
    }
}
