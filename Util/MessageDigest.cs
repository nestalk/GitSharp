﻿/*
 * Copyright (C) 2009, Kevin Thompson <kevin.thompson@theautomaters.com>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * - Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above
 *   copyright notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 * - Neither the name of the Git Development Community nor the
 *   names of its contributors may be used to endorse or promote
 *   products derived from this software without specific prior
 *   written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Gitty.Core.Util
{
    internal class MessageDigest : ICloneable
    {
        public HashAlgorithm Algorithm { get; private set; }

        private MemoryStream _stream;
        private BinaryWriter _writer;

        private MessageDigest(HashAlgorithm algorithm)
        {
            this.Algorithm = algorithm;
            this.Reset();
        }

        private MessageDigest(byte[] buffer, HashAlgorithm algorithm)
        {
            this.Algorithm = algorithm;
            _stream = new MemoryStream(buffer, true);
            _writer = new BinaryWriter(_stream);
        }

        #region ICloneable Members

        public object Clone()
        {
            return new MessageDigest(this._stream.ToArray(), this.Algorithm);
        }

        #endregion

        public byte[] Digest()
        {
            return this.Algorithm.ComputeHash(_stream);
        }

        public byte[] Digest(byte[] input)
        {
            Update(input);
            return Digest();
        }

        public void Reset()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream);
        }

        public void Update(byte input)
        {
            _writer.Write(input);
        }

        public void Update(byte[] input)
        {
            _writer.Write(input);
        }

        public void Update(byte[] input, int index, int count)
        {
            _writer.Write(input, index, count);
        }


        public static MessageDigest GetInstance(string algorithm)
        {
            switch (algorithm.ToLower())
            {
                case "sha1":
                    return new MessageDigest(new SHA1CryptoServiceProvider());
                default:
                    throw new NotSupportedException(string.Format("The requested algorithm \"{0}\" is not supported.", algorithm));
                    break;
            }
        }
    }
}