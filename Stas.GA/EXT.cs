using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using V3 = System.Numerics.Vector3;
using V2 = System.Numerics.Vector2;
using System.Threading;

namespace Stas.GA {
    internal class UnsafeMath {
       
        //// Copyright (c) 2008-2013 Hafthor Stefansson// Distributed under the MIT/X11 software
        //public static unsafe bool Compare(byte[] a1, byte[] a2) {
        //    if (a1 == null || a2 == null || a1.Length != a2.Length)
        //        return false;
        //    fixed (byte* p1 = a1, p2 = a2) {
        //        byte* x1 = p1, x2 = p2;
        //        int l = a1.Length;
        //        for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
        //            if (*((long*)x1) != *((long*)x2))
        //                return false;
        //        if ((l & 4) != 0) {
        //            if (*((int*)x1) != *((int*)x2)) return false;
        //            x1 += 4;
        //            x2 += 4;
        //        }
        //        if ((l & 2) != 0) {
        //            if (*((short*)x1) != *((short*)x2)) return false;
        //            x1 += 2;
        //            x2 += 2;
        //        }
        //        if ((l & 1) != 0)
        //            if (*((byte*)x1) != *((byte*)x2))
        //                return false;
        //        return true;
        //    }
        //}
    }
    internal static partial class EXT {
        public static bool less_or_equal (this V2 left, V2 right) {
            return left.X < right.X && left.Y <= right.Y;
        }
        public static bool greater_or_equal(this V2 left, V2 right) {
            return left.X >= right.X && left.Y >= right.Y;
        }
        public static byte[] ToByteArr(this Keys k) {
            var res = BitConverter.GetBytes((int)k);
            return res;
        }
        public static float Lerp(float firstFloat, float secondFloat, float by) {
            return firstFloat * (1 - by) + secondFloat * by;
        }
        public static bool Less(this V2 v, V2 v2) {
            return v.X < v2.X && v.Y < v2.Y;
        }
        public static bool Greater(this V2 v, V2 v2) {
            return v.X > v2.X && v.Y > v2.Y;
        }
        public static float Round(this float f, int count) {
            return (float)Math.Round(f, count);
        }
        /// <summary>
        /// Convert an unsigned int (32bit) to <see cref="Color"/>
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// </returns>
        public static Color ToColor(this uint color) {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
        public static byte[] ToByte(this V2 v) {
            return BYTE.Concat(BitConverter.GetBytes(v.X), BitConverter.GetBytes(v.Y));
        }

        public static byte ToByte(this bool bv) {
            if (bv)
                return 1;
            else
                return 0;
        }
        /// <summary>
        /// Gets the Point that specifies the center of the rectangle.
        /// </summary>
        /// <value>
        /// The center.
        /// </value>
        public static V2 GetCenter(this RectangleF rect) {
            return new V2(rect.X + (rect.Width / 2),
                rect.Y + (rect.Height / 2));
        }
        public static bool PointInRectangle(this V2 point, Rectangle rect) {
            return rect.Contains((int)point.X, (int)point.Y);
        }
        public static V3 ToV3(this byte[] ba, int start) {
            var data = new byte[ba.Length - start];
            Array.Copy(ba, start, data, 0, ba.Length - start);
            var res = V3.Zero;
            res.X = BitConverter.ToSingle(data, 0);
            res.Y = BitConverter.ToSingle(data, 4);
            res.Z = BitConverter.ToSingle(data, 8);
            return res;
        }
        public static void SetWindowToTop(IntPtr handle) {
            while (GetForegroundWindow() != handle) {
                Thread.Sleep(250);
                SetForegroundWindow(handle);
            }
        }
        [DllImport("user32.dll")]
        static extern int SetForegroundWindow(IntPtr hWnd);

        public static string To_UTF8_String(this byte[] ba, int start_index = 0) {
            var sl = BitConverter.ToInt16(ba, start_index); //string lenght
            Debug.Assert(sl < 256);
            var res = Encoding.UTF8.GetString(ba, start_index + 2, sl);
            return res;
        }
        public static string To_UTF8_String(this BinaryReader br) {
            var sl = br.ReadInt16(); //string lenght
            if (sl > 256) {
                ui.AddToLog("To_UTF8_String sl=[" + sl + "]", MessType.Error);
                return "error";
            }
            var res = Encoding.UTF8.GetString(br.ReadBytes(sl));
            return res;
        }
        public static Vector4 ToSysNumV4(this Color4 info) {
            return new Vector4(info.X, info.Y, info.Z, info.W);
        }

        public static V3 ToSysNumV3(this byte[] ba, int offset = 0) {
            return new V3(BitConverter.ToSingle(ba, 0 + offset),
                BitConverter.ToSingle(ba, 4 + offset),
                BitConverter.ToSingle(ba, 8 + offset));
        }
        public static V2 ToSysNumV2(this byte[] ba, int offs = 0) {
            return new V2(BitConverter.ToSingle(ba, offs),
                BitConverter.ToSingle(ba, offs + 4));
        }
        public static bool ToBool(this byte b) {
            if (b == 0)
                return false;
            else
                return true;
        }

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();
        ///// <summary>
        ///// Gets the Point that specifies the center of the rectangle.
        ///// </summary>
        ///// <value>
        ///// The center.
        ///// </value>
        public static V2 Center(this Rectangle rect) {
            return new V2(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }
        public static V3 ToV3(this StdTuple3D<float> tuple) {
            return new V3(tuple.X, tuple.Y, tuple.Z);
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        public static Rectangle GetWindowRectangle(IntPtr ptr) {
            RECT rect;
            GetWindowRect(ptr, out rect);
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
        /// <summary>
        /// It does not correct the screen percentage change
        /// </summary>
        /// <returns></returns>
        public static Rectangle GetActiveWindowRectangle() {
            return GetWindowRectangle(GetForegroundWindow());
        }
        /// The GetForegroundWindow function returns a handle to the  foreground window.
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        public static float GetScreenScalingFactor() {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            return ScreenScalingFactor; // 1.25 = 125%
        }
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }
        public static double GetCoordinate(this V2 point, Axis axis) {
            switch (axis) {
                case Axis.X:
                    return point.X;
                case Axis.Y:
                    return point.Y;
                default:
                    throw new ArgumentException();
            }
        }
        public static byte[] To_UTF8_Byte(this string str) {
            if (str == null)
                return new byte[] { 0, 0 };
            var ba = Encoding.UTF8.GetBytes(str);
            Debug.Assert(ba.Length < 256);
            return BYTE.Concat(BitConverter.GetBytes((short)ba.Length), ba);
        }
        public static string ToElapsedString(this Stopwatch sw) {
            return Math.Round(sw.Elapsed.TotalMilliseconds, 0) + "ms";
        }

        public static string ToRoundStr(this double f, int round = 3) {
            return (Math.Round(f, round)).ToString();
        }
        public static string ToRoundStr(this float f, int round = 3) {
            return (Math.Round(f, round)).ToString();
        }

        public static string ToIntString(this V2 v2) {
            return (int)v2.X + " " + (int)v2.Y;
        }

        public static string ToIntString(this V3 v3) {
            return (int)v3.X + " " + (int)v3.Y + " " + (int)v3.Z; ;
        }
       
        public static Vector4 ToImguiVec4(this Color c) {
            return new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        }

        public static Vector4 ToImguiVec4(this Color c, byte alpha) {
            return new Vector4(c.R, c.G, c.B, alpha);
        }

        public static float Max(params float[] values) {
            var max = values.First();
            for (var i = 1; i < values.Length; i++) {
                max = Math.Max(max, values[i]);
            }
            return max;
        }
        public static V2 Subtract(this V2 source, V2 targ) {
            return new V2(source.X - targ.X, source.Y - targ.Y);
        }
        public static V2 Increase(this V2 source, V2 targ) {
            return new V2(source.X + targ.X, source.Y + targ.Y);
        }
        public static V2 Increase(this V2 vector, float dx = 0f, float dy = 0f) {
            return new V2(vector.X + dx, vector.Y + dy);
        }

        public static V3 Increase(this V3 vector, float dx, float dy, float dz) {
            return new V3(vector.X + dx, vector.Y + dy, vector.Z + dz);
        }
        public static uint ToUint32Hash(this string strTex) {
            if (strTex == null)
                return 0;
            if (strTex.Length == 0)
                return 1;
            return MurmurHash2Simple.Hash(Encoding.UTF8.GetBytes(strTex));
        }
        public static string ToHexString(this byte[] ba) {
            var sb = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba) {
                // can be "x2" if you want lowercase & X2 for upper
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        
        public static byte[] ToByte(this V3 v3) {
            return Concat(BitConverter.GetBytes(v3.X),
                BitConverter.GetBytes(v3.Y),
                BitConverter.GetBytes(v3.Z));
        }
        public static byte[] Concat(byte[] a, byte[] b, byte[] c) {
            return Concat(Concat(a, b), c);
        }
        public static byte[] Concat(byte[] a, byte[] b) {
            Debug.Assert(a != null && b != null, Environment.StackTrace);
            byte[] buffer1 = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, buffer1, 0, a.Length);
            Buffer.BlockCopy(b, 0, buffer1, a.Length, b.Length);
            return buffer1;
        }
        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>A packed integer containing all four color components.</returns>
        public static uint ToImgui(this Color c) {
            int value = c.R;
            value |= c.G << 8;
            value |= c.B << 16;
            value |= c.A << 24;

            return (uint)value;
        }
        public static float GetDistance(this V2 a, V2 b) {
            return V2.Distance(a, b);
        }

        public static float GetDistance(this V3 a, V3 b) {
            return V3.Distance(a, b);
        }

        public static string ElapsedTostring(this Stopwatch sw) {
            return Math.Round(sw.Elapsed.TotalMilliseconds, 0).ToString();
        }
        /// <summary>
        /// Writes to a file in a thread-safe manner using a Mutex lock.
        /// Overwrites files by default.
        /// </summary>
        /// <remarks>Inspired by https://stackoverflow.com/a/229567 .</remarks>
        /// <param name="output">Input string to write to the file.</param>
        /// <param name="filePath">Path of file to write to.</param>
        /// <param name="overwrite">Whether to overwrite pre-existing files.</param>
        public static void SafelyWriteToFile(this string output, string filePath, bool overwrite = true) {

            // Unique id for global mutex - Global prefix means it is global to the machine
            // We use filePath to ensure the mutex is only held for the particular file
            string mutexId = string.Format("Global\\{{{0}}}", Path.GetFileNameWithoutExtension(filePath));

            // We create/query the Mutex
            using (var mutex = new Mutex(false, mutexId)) {

                var hasHandle = false;

                try {

                    // We wait for lock to release
                    hasHandle = mutex.WaitOne(Timeout.Infinite, false);

                    // Write to file
                    if (overwrite)
                        File.WriteAllText(filePath, output);
                    else
                        File.AppendAllText(filePath, output);

                }
                finally {

                    // If we have the Mutex, we release it
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
    }
}
