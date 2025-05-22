//using System;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//namespace LIB_BDD {
//    public static class KeyboardHook {
//        private const int WH_KEYBOARD_LL = 13;
//        private const int WM_KEYDOWN = 0x0100;
//        private const int VK_TAB = 0x09;
//        private const int VK_F4 = 0x73;
//        private const int VK_ESCAPE = 0x1B;
//        private const int VK_LALT = 0xA4;
//        private const int VK_RALT = 0xA5;

//        private static IntPtr _hookID = IntPtr.Zero;
//        private static LowLevelKeyboardProc _proc = HookCallback;

//        public static void Start() {
//            _hookID = SetHook(_proc);
//        }

//        public static void Stop() {
//            UnhookWindowsHookEx(_hookID);
//        }

//        private static IntPtr SetHook(LowLevelKeyboardProc proc) {
//            using(var curProcess = Process.GetCurrentProcess())
//            using(var curModule = curProcess.MainModule) {
//                return SetWindowsHookEx(WH_KEYBOARD_LL,proc,
//                    GetModuleHandle(curModule.ModuleName),0);
//            }
//        }

//        private delegate IntPtr LowLevelKeyboardProc(int nCode,IntPtr wParam,IntPtr lParam);

//        private static IntPtr HookCallback(int nCode,IntPtr wParam,IntPtr lParam) {
//            if(nCode >= 0) {
//                int vkCode = Marshal.ReadInt32(lParam);

//                // Interception spécifique pour ALT + F4
//                if((wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)0x104) && vkCode == VK_F4 &&
//                    (GetAsyncKeyState(VK_LALT) < 0 || GetAsyncKeyState(VK_RALT) < 0)) {
//                    return (IntPtr)1; // Bloque l'événement clavier
//                }

//                if(wParam == (IntPtr)WM_KEYDOWN && vkCode == VK_ESCAPE) {
//                    return (IntPtr)1;
//                }
//            }
//            return CallNextHookEx(_hookID,nCode,wParam,lParam);
//        }

//        private static bool IsKeyBlocked(int vkCode) {
//            // Vérifie si Alt est enfoncé et une des touches bloquées
//            bool isAltPressed = (GetAsyncKeyState(VK_LALT) < 0 || GetAsyncKeyState(VK_RALT) < 0);
//            return (isAltPressed && (vkCode == VK_TAB || vkCode == VK_F4)) || vkCode == VK_ESCAPE;
//        }

//        [DllImport("user32.dll",CharSet = CharSet.Auto,SetLastError = true)]
//        private static extern IntPtr SetWindowsHookEx(int idHook,LowLevelKeyboardProc lpfn,
//            IntPtr hMod,uint dwThreadId);

//        [DllImport("user32.dll",CharSet = CharSet.Auto,SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

//        [DllImport("user32.dll",CharSet = CharSet.Auto)]
//        private static extern IntPtr CallNextHookEx(IntPtr hhk,int nCode,
//            IntPtr wParam,IntPtr lParam);

//        [DllImport("kernel32.dll",CharSet = CharSet.Auto,SetLastError = true)]
//        private static extern IntPtr GetModuleHandle(string lpModuleName);

//        [DllImport("user32.dll")]
//        private static extern short GetAsyncKeyState(int vKey);
//    }
//}