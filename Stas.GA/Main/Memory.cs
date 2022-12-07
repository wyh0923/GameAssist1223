using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;
namespace Stas.GA; 

/// <summary>
///     Handle to a process[05.12.2022].
///     <Copyright>https://github.com/zaafar</Copyright>
/// </summary>
public class Memory : SafeHandleZeroOrMinusOneIsInvalid {
   
    /// <summary>
    ///     Initializes a new instance of the <see cref="Memory" /> class.
    /// </summary>
    /// <param name="pid">processId you want to access.</param>
    internal Memory(int pid ) : base(true) {
        var handle = NativeWrapper.OpenProcess(ProcessAccessFlags.VirtualMemoryRead, pid);
        if (NativeWrapper.HasError) {
            ui.AddToLog($"Failed to open a new handle 0x{handle:X}" +
                              $" due to ErrorNo: {NativeWrapper.LastError}", MessType.Critical);
        }
        else {
            ui.AddToLog($"Dll started and using handle [" +handle.ToString()+"] from pid=["+ pid + "]", MessType.Warning);
        }
        SetHandle(handle);
    }
    internal T Read<T>(long address) where T : unmanaged {
        return Read<T>(new IntPtr(address));
    }
    int rpm_count = 0; //read prosses memoty
    int aw_count = 0; //adress wrong
    int max_add = 36;
    /// <summary>
    ///     Reads the process memory as type T.
    /// </summary>
    /// <typeparam name="T">type of data structure to read.</typeparam>
    /// <param name="address">address to read the data from.</param>
    /// <returns>data from the process in T format.</returns>
    internal T Read<T>(IntPtr address, string from = null) where T : unmanaged {
        string add = !string.IsNullOrEmpty(from) ? " " + from : "";

        if(add.Length > max_add)
            add = add.Substring(0, max_add) +"..skip..";
        T result = default;
      
        if (IsInvalid || address.ToInt64() <= 0) {
            ui.AddToLog("Mem.Read adres wrong: [" + (aw_count += 1) + "]"+ add, MessType.Critical);
            if (aw_count > 50) { 
            }
            return result;
        }
        //Thread.Sleep(5); //=>try here like cpu laags. each thred must be have self reader
        try {
            if (!NativeWrapper.ReadProcessMemory(handle, address, ref result)) {
                var err = (NtStatus)NativeWrapper.LastError;

                ui.AddToLog("Mem.Read err..."+  add + err, MessType.Critical);
                if (rpm_count > 50) { 
                }//<==set BP here for see who is
            }
            return result;
        }
        catch (Exception e) {
            ui.AddToLog("Mem.Read exept: " + e.Message + add, MessType.Critical);
            return default;
        }
    }

    internal T Read<T>(long addr, params int[] offsets) where T : unmanaged {
        return Read<T>(new IntPtr(addr), offsets);
    }
    internal T Read<T>(IntPtr addr, params int[] offsets) where T : unmanaged {
        if (addr == IntPtr.Zero) return default;
        var num = Read<long>(addr);
        var result = num;

        for (var index = 0; index < offsets.Length - 1; index++) {
            if (result == 0)
                return default;

            var offset = offsets[index];
            result = Read<long>(result + offset);
        }

        if (result == 0)
            return default;

        return Read<T>(result + offsets[offsets.Length - 1]);
    }

    /// <summary>
    ///     Reads the std::vector into an array.
    /// </summary>
    /// <typeparam name="T">Object type to read.</typeparam>
    /// <param name="nativeContainer">StdVector address to read from.</param>
    /// <returns>An array of elements of type T.</returns>
    internal T[] ReadStdVector<T>(StdVector nativeContainer) where T : unmanaged {
        var typeSize = Marshal.SizeOf<T>();
        var length = nativeContainer.Last.ToInt64() - nativeContainer.First.ToInt64();
        if (length <= 0 || length % typeSize != 0) {
            return Array.Empty<T>();
        }

        return ReadMemoryArray<T>(nativeContainer.First, (int)length / typeSize);
    }

    /// <summary>
    ///     Reads the process memory as an array.
    /// </summary>
    /// <typeparam name="T">Array type to read.</typeparam>
    /// <param name="address">memory address to read from.</param>
    /// <param name="nsize">total array elements to read.</param>
    /// <returns>
    ///     An array of type T and of size nsize. In case or any error it returns empty array.
    /// </returns>
    internal T[] ReadMemoryArray<T>(IntPtr address, int nsize) where T : unmanaged {
        if (IsInvalid || address.ToInt64() <= 0 || nsize <= 0) {
            return Array.Empty<T>();
        }

        var buffer = new T[nsize];
        try {
            if (!NativeWrapper.ReadProcessMemoryArray(handle, address, buffer, out var numBytesRead)) {
                ui.AddToLog("Mem.ReadArray err...", MessType.Critical);
                ui.AddToLog("Mem err last info: addr=[" + address.ToInt64() + "] size=[" + nsize + "]", MessType.Critical);
            }

            if (numBytesRead.ToInt32() < nsize) {
                ui.AddToLog($"Mem.ReadArray: wrong num Bytes Read", MessType.Critical);
            }
            return buffer;
        }
        catch (Exception e) {
            ui.AddToLog($"Mem.ReadArray err: {e.Message}", MessType.Critical);
            return Array.Empty<T>();
        }
    }

    /// <summary>
    ///     Reads the std::map into a List.
    /// </summary>
    /// <typeparam name="TKey">key type of the stdmap.</typeparam>
    /// <typeparam name="TValue">value type of the stdmap.</typeparam>
    /// <param name="nativeContainer">native object of the std::map.</param>
    /// <param name="keyfilter">Filter the keys based on the function return value.</param>
    /// <returns>a list containing the keys and the values of the stdmap as named tuple.</returns>
    internal List<(TKey Key, TValue Value)> ReadStdMapAsList<TKey, TValue>(StdMap nativeContainer,
         Func<TKey, bool> keyfilter = null) where TKey : unmanaged where TValue : unmanaged {
        const int MaxAllowed = 10000;
        var collection = new List<(TKey Key, TValue Value)>();
        if (nativeContainer.Size <= 0 || nativeContainer.Size > MaxAllowed) {
            return collection;
        }

        var childrens = new Stack<StdMapNode<TKey, TValue>>();
        var head = this.Read<StdMapNode<TKey, TValue>>(nativeContainer.Head);
        var parent = this.Read<StdMapNode<TKey, TValue>>(head.Parent);
        childrens.Push(parent);
        var counter = 0;
        while (childrens.Count != 0) {
            var cur = childrens.Pop();
            if (counter++ > nativeContainer.Size + 5) {
                childrens.Clear();
                return collection;
            }

            if (!cur.IsNil && (keyfilter == null || keyfilter(cur.Data.Key))) {
                collection.Add((cur.Data.Key, cur.Data.Value));
            }

            var left = this.Read<StdMapNode<TKey, TValue>>(cur.Left);
            if (!left.IsNil) {
                childrens.Push(left);
            }

            var right = this.Read<StdMapNode<TKey, TValue>>(cur.Right);
            if (!right.IsNil) {
                childrens.Push(right);
            }
        }

        return collection;
    }

    /// <summary>
    ///     Reads the StdList into a List.
    /// </summary>
    /// <typeparam name="TValue">StdList element structure.</typeparam>
    /// <param name="nativeContainer">native object of the std::list.</param>
    /// <returns>List containing TValue elements.</returns>
    internal List<TValue> ReadStdList<TValue>(StdList nativeContainer) where TValue : unmanaged {
        var retList = new List<TValue>();
        var currNodeAddress = Read<StdListNode>(nativeContainer.Head).Next;
        while (currNodeAddress != nativeContainer.Head) {
            var currNode = Read<StdListNode<TValue>>(currNodeAddress);
            if (currNodeAddress == IntPtr.Zero) {
                ui.AddToLog("Terminating reading of list next nodes because of" +
                                  "unexpected 0x00 found. This is normal if it happens " +
                                  "after closing the game, otherwise report it.", MessType.Critical);
                break;
            }

            retList.Add(currNode.Data);
            currNodeAddress = currNode.Next;
        }

        return retList;
    }

    /// <summary>
    ///     Reads the std::bucket into a list.
    /// </summary>
    /// <typeparam name="TValue">value type that the std bucket contains.</typeparam>
    /// <param name="nativeContainer">native object of the std::bucket.</param>
    /// <returns>a list containing all the valid values found in std::bucket.</returns>
    internal List<TValue> ReadStdBucket<TValue>(StdBucket nativeContainer) where TValue : unmanaged {
        if (nativeContainer.Data == IntPtr.Zero ||
            nativeContainer.Capacity <= 0x00) {
            return new List<TValue>();
        }

        var size = ((int)nativeContainer.Capacity + 1) / 8;
        var ret = new List<TValue>();
        var dataArray = ReadMemoryArray<StdBucketNode<TValue>>(nativeContainer.Data, size);
        for (var i = 0; i < dataArray.Length; i++) {
            var data = dataArray[i];
            if (data.Flag0 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer0);
            }

            if (data.Flag1 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer1);
            }

            if (data.Flag2 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer2);
            }

            if (data.Flag3 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer3);
            }

            if (data.Flag4 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer4);
            }

            if (data.Flag5 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer5);
            }

            if (data.Flag6 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer6);
            }

            if (data.Flag7 != StdBucketNode<TValue>.InValidPointerFlagValue) {
                ret.Add(data.Pointer7);
            }
        }

        return ret;
    }


    /// <summary>
    ///     Reads the std::wstring. String read is in unicode format.
    /// </summary>
    /// <param name="nativecontainer">native object of std::wstring.</param>
    /// <returns>string.</returns>
    internal string ReadStdWString(StdWString nativecontainer) {
        const int MaxAllowed = 1000;
        if (nativecontainer.Length <= 0 ||
            nativecontainer.Length > MaxAllowed ||
            nativecontainer.Capacity <= 0 ||
            nativecontainer.Capacity > MaxAllowed) {
            return string.Empty;
        }

        if (nativecontainer.Capacity <= 8) {
            try {
                var buffer = BitConverter.GetBytes(nativecontainer.Buffer.ToInt64());
                var ret = Encoding.Unicode.GetString(buffer);
                buffer = BitConverter.GetBytes(nativecontainer.ReservedBytes.ToInt64());
                ret += Encoding.Unicode.GetString(buffer);
                if(nativecontainer.Length < ret.Length)
                    return ret[..nativecontainer.Length];
                else
                    return "err: "+ ret.Substring(0, Math.Min(20, ret.Length));
            }
            catch (Exception ex) {
                return "Error";
            }
        }
        else {
            var buffer = ReadMemoryArray<byte>(nativecontainer.Buffer, nativecontainer.Length * 2);
            return Encoding.Unicode.GetString(buffer);
        }
    }
    internal string ReadString(long address, int size = 128) {
        return ReadString(new IntPtr(address), size);
    }
    /// <summary>
    ///     Reads the string.
    /// </summary>
    /// <param name="address">pointer to the string.</param>
    /// <returns>string read.</returns>
    internal string ReadString(IntPtr address, int size = 128) {
        var buffer = ReadMemoryArray<byte>(address, size);
        var count = Array.IndexOf<byte>(buffer, 0x00, 0);
        if (count > 0) {
            return Encoding.ASCII.GetString(buffer, 0, count);
        }

        return string.Empty;
    }
    internal string ReadStringU(long addr, int length = 256, bool replaceNull = true) {
        if (addr <= 0 || length > 5120 || length <= 0) {
            ui.AddToLog("ReadStringU... err addr", MessType.Critical);
            return string.Empty;
        }
        var mem = ReadMemoryArray<byte>(new IntPtr(addr), length);
        if (mem[0] == 0 && mem[1] == 0) {
            ui.AddToLog("ReadStringU... err array", MessType.Warning);
            return string.Empty;
        }
        var _str = Encoding.Unicode.GetString(mem);
        return replaceNull ? RTrimNull(_str) : _str;
    }
    /// <summary>
    ///     Reads Unicode string when string length isn't know.
    ///     Use  <see cref="ReadStdWString" /> if string length is known.
    /// </summary>
    /// <param name="address">points to the Unicode string pointer.</param>
    /// <returns>string read from the memory.</returns>
    internal string ReadUnicodeString(IntPtr address) {
        var buffer = ReadMemoryArray<byte>(address, 256);
        var count = 0x00;
        for (var i = 0; i < buffer.Length - 2; i++) {
            if (buffer[i] == 0x00 && buffer[i + 1] == 0x00 && buffer[i + 2] == 0x00) {
                count = i % 2 == 0 ? i : i + 1;
                break;
            }
        }

        // let's not return a string if null isn't found.
        if (count == 0) {
            return string.Empty;
        }

        var ret = Encoding.Unicode.GetString(buffer, 0, count);
        return ret;
    }
    public string ReadNativeString(long address) {
        var Size = Read<uint>(address + 0x8);
        var Reserved = Read<uint>(address + 0x10);

        //var size = Size;
        //if (size == 0)
        //    return string.Empty;
        if (Reserved == 0)
            return string.Empty;

        if ( /*8 <= size ||*/ 8 <= Reserved) //Have no idea how to deal with this
        {
            var readAddr = Read<long>(address);
            return ReadStringU(readAddr);
        }

        return ReadStringU(address);
    }
   
    string RTrimNull(string text) {
        var num = text.IndexOf('\0');
        return num > 0 ? text.Substring(0, num) : text;
    }
    public int GetOffset<T>(string name) where T : struct {
        try {
            var type = typeof(T);

            var offset = (int)type.GetFields().First(x => x.Name == name).GetCustomAttributesData()
                .First(x => x.AttributeType.Name.Equals("FieldOffsetAttribute", StringComparison.Ordinal))
                .ConstructorArguments.First().Value;

            return offset;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }


    /// <summary>
    ///     When overridden in a derived class, executes the code required to free the handle.
    /// </summary>
    /// <returns>
    ///     true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false.
    ///     In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
    /// </returns>
    protected override bool ReleaseHandle() {
        ui.AddToLog($"Releasing handle on 0x{handle:X}\n", MessType.Warning);
        return NativeWrapper.CloseHandle(handle);
    }
}

