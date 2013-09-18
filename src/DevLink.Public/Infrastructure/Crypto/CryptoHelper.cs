/*
 * Copyright (c) Brock Allen.  All rights reserved.
 * see license.txt
 * https://github.com/brockallen/BrockAllen.MembershipReboot/blob/master/src/BrockAllen.MembershipReboot/Crypto/CryptoHelper.cs
 * 
 * New BSD License
---------------
Copyright (c)2012, Brock Allen. All Rights Reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. The name of the author may not be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY Brock Allen "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 */

using System;
using System.Linq;

namespace DevLink.Public.Infrastructure.Crypto
{
	public static class CryptoHelper
	{
		internal const char PasswordHashingIterationCountSeparator = '.';
		internal static Func<int> GetCurrentYear = () => DateTime.Now.Year;

		internal static string Hash(string value)
		{
			return Crypto.Hash(value);
		}

		internal static string Hash(string value, string key)
		{
			if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");
			if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");

			var valueBytes = System.Text.Encoding.UTF8.GetBytes(key);
			var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

			var alg = new System.Security.Cryptography.HMACSHA512(keyBytes);
			var hash = alg.ComputeHash(valueBytes);

			var result = Crypto.BinaryToHex(hash);
			return result;
		}

		internal static string GenerateNumericCode(int digits)
		{
			// 18 is good size for a long
			if (digits > 18) digits = 18;
			if (digits <= 0) digits = 6;

			var bytes = Crypto.GenerateSaltInternal(sizeof(long));
			var val = BitConverter.ToInt64(bytes, 0);
			var mod = (int)Math.Pow(10, digits);
			val %= mod;
			val = Math.Abs(val);

			return val.ToString("D" + digits);
		}

		internal static string GenerateSalt()
		{
			return Crypto.GenerateSalt();
		}

		public static string HashPassword(string password)
		{
			var count = GetIterationsFromYear(GetCurrentYear());
			var result = Crypto.HashPassword(password, count);
			return EncodeIterations(count) + PasswordHashingIterationCountSeparator + result;
		}

		public static bool VerifyHashedPassword(string hashedPassword, string password)
		{
			if (hashedPassword.Contains(PasswordHashingIterationCountSeparator))
			{
				var parts = hashedPassword.Split(PasswordHashingIterationCountSeparator);
				if (parts.Length != 2) return false;

				int count = DecodeIterations(parts[0]);
				if (count <= 0) return false;

				hashedPassword = parts[1];

				return Crypto.VerifyHashedPassword(hashedPassword, password, count);
			}
			else
			{
				return Crypto.VerifyHashedPassword(hashedPassword, password);
			}
		}

		internal static string EncodeIterations(int count)
		{
			return count.ToString("X");
		}

		internal static int DecodeIterations(string prefix)
		{
			int val;
			if (Int32.TryParse(prefix, System.Globalization.NumberStyles.HexNumber, null, out val))
			{
				return val;
			}
			return -1;
		}

		// from OWASP : https://www.owasp.org/index.php/Password_Storage_Cheat_Sheet
		const int StartYear = 2000;
		const int StartCount = 1000;
		internal static int GetIterationsFromYear(int year)
		{
			if (year > StartYear)
			{
				var diff = (year - StartYear) / 2;
				var mul = (int)Math.Pow(2, diff);
				int count = StartCount * mul;
				// if we go negative, then we wrapped (expected in year ~2044). 
				// Int32.Max is best we can do at this point
				if (count < 0) count = Int32.MaxValue;
				return count;
			}
			return StartCount;
		}
	}
}