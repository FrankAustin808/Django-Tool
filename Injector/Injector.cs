using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Django.Injector;

namespace Django
{
    internal class Injector
    {
        public enum InjectionResult
        {
            dllNotFound,
            gameProcessNotFound,
            injectionFailed,
            Success,
        }

        public sealed class DllInjector
        {
            private static readonly IntPtr INTPTR_ZERO = (IntPtr)0;
            private static Injector.DllInjector _instance;

            public static Injector.DllInjector GetInstance
            {
                get
                {
                    if (Injector.DllInjector._instance == null)
                        Injector.DllInjector._instance = new Injector.DllInjector();
                    return Injector.DllInjector._instance;
                }
            }

            private DllInjector()
            {
            }

            private bool bInject(uint pToBeInjected, string sDllPath)
            {
                IntPtr num1 = Injector.DllInjector.OpenProcess(1082U, 1, pToBeInjected);
                if (num1 == Injector.DllInjector.INTPTR_ZERO)
                    return false;
                IntPtr procAddress = Injector.DllInjector.GetProcAddress(Injector.DllInjector.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (procAddress == Injector.DllInjector.INTPTR_ZERO)
                    return false;
                IntPtr num2 = Injector.DllInjector.VirtualAllocEx(num1, (IntPtr)0, (IntPtr)sDllPath.Length, 12288U, 64U);
                if (num2 == Injector.DllInjector.INTPTR_ZERO)
                    return false;
                byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);
                if (Injector.DllInjector.WriteProcessMemory(num1, num2, bytes, (uint)bytes.Length, 0) == 0 || Injector.DllInjector.CreateRemoteThread(num1, (IntPtr)0, Injector.DllInjector.INTPTR_ZERO, procAddress, num2, 0U, (IntPtr)0) == Injector.DllInjector.INTPTR_ZERO)
                    return false;
                Injector.DllInjector.CloseHandle(num1);
                return true;
            }

            [DllImport("kernel32.dll", SetLastError = true)]

            private static extern int CloseHandle(IntPtr hObject);
            [DllImport("kernel32.dll", SetLastError = true)]

            private static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttribute,
            IntPtr dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            public Injector.InjectionResult Inject(string sProcName, string sDllPath)
            {
                if (!File.Exists(sDllPath))
                    return Injector.InjectionResult.dllNotFound;
                uint pToBeInjected = 0;
                Process[] processes = Process.GetProcesses();
                for (int index = 0; index < processes.Length; ++index)
                {
                    if (!(processes[index].ProcessName != sProcName))
                    {
                        pToBeInjected = (uint)processes[index].Id;
                        break;
                    }
                }
                if (pToBeInjected == 0U)
                    return Injector.InjectionResult.gameProcessNotFound;
                return !this.bInject(pToBeInjected, sDllPath) ? Injector.InjectionResult.injectionFailed : Injector.InjectionResult.Success;
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr OpenProcess(
              uint dwDesiredAccess,
              int bInheritHandle,
              uint dwProcessId);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr VirtualAllocEx(
              IntPtr hProcess,
              IntPtr lpAddress,
              IntPtr dwSize,
              uint flAllocationType,
              uint flProtect);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern int WriteProcessMemory(
              IntPtr hProcess,
              IntPtr lpBaseAddress,
              byte[] buffer,
              uint size,
              int lpNumberOfBytesWritten);
        }
    }
}