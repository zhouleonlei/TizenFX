﻿/*
 *  Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;
using System.Runtime.InteropServices;
using static Interop;

namespace Tizen.Security.SecureRepository
{
    /// <summary>
    /// Class that represents a key.
    /// </summary>
    public class Key : SafeHandle
    {
        /// <summary>
        /// A constructor of Key that takes the binary, its type, and optional password of binary.
        /// </summary>
        /// <param name="binary">The binary value of a key. This binary may be encrypted with binaryPassword.</param>
        /// <param name="type">The key's type.</param>
        /// <param name="binaryPassword">The password used to decrypt binary when binary is encrypted.</param>
        public Key(byte[] binary, KeyType type, string binaryPassword) : base(IntPtr.Zero, true)
        {
            this.SetHandle(IntPtr.Zero);
            Binary = binary;
            Type = type;
            BinaryPassword = binaryPassword;
        }

        internal Key(IntPtr ptr, bool ownsHandle = true) : base(IntPtr.Zero, ownsHandle)
        {
            base.SetHandle(ptr);

            CkmcKey ckmcKey = Marshal.PtrToStructure<CkmcKey>(handle);
            Binary = new byte[ckmcKey.size];
            Marshal.Copy(ckmcKey.rawKey, Binary, 0, Binary.Length);
            Type = (KeyType)ckmcKey.keyType;
            BinaryPassword = Marshal.PtrToStringAnsi(ckmcKey.password);
        }

        /// <summary>
        /// The binary value of a key.
        /// </summary>
        public byte[] Binary
        {
            get; set;
        }

        /// <summary>
        /// The key's type.
        /// </summary>
        public KeyType Type
        {
            get; set;
        }

        /// <summary>
        /// The password used to decrypt binary when binary is encrypted. It's optional.
        /// </summary>
        public string BinaryPassword
        {
            get; set;
        }

        internal CkmcKey ToCkmcKey()
        {
            return new Interop.CkmcKey(new PinnedObject(Binary),
                                       Binary.Length,
                                       (int)Type,
                                       new PinnedObject(BinaryPassword));
        }

        /// <summary>
        /// Gets a value that indicates whether the handle is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        /// <summary>
        /// When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>true if the handle is released successfully</returns>
        protected override bool ReleaseHandle()
        {
            if (IsInvalid) // do not release
                return true;

            Interop.CkmcTypes.KeyFree(handle);
            this.SetHandle(IntPtr.Zero);
            return true;
        }
    }
}
