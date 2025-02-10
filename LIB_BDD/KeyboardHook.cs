using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB_BDD {
    using System;
    using System.Runtime.InteropServices;

    public static class KeyboardHook {
        [DllImport("user32.dll",SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd,int id,uint fsModifiers,uint vk);

        [DllImport("user32.dll",SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd,int id);

        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CTRL = 0x0002;
        private const uint MOD_WIN = 0x0008;
        private const int VK_TAB = 0x09;
        private const int VK_F4 = 0x73;

        private const int HOTKEY_ID = 9000;

        public static void DisableSystemShortcuts(IntPtr windowHandle) {
            RegisterHotKey(windowHandle,HOTKEY_ID,MOD_ALT,VK_TAB);  // Désactive Alt + Tab
            RegisterHotKey(windowHandle,HOTKEY_ID + 1,MOD_ALT,VK_F4); // Désactive Alt + F4
        }

        public static void ReleaseHotKeys(IntPtr windowHandle) {
            UnregisterHotKey(windowHandle,HOTKEY_ID);
            UnregisterHotKey(windowHandle,HOTKEY_ID + 1);
        }
    }
}
