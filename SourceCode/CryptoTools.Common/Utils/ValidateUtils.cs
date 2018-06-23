using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTools.Common.Utils
{
	public class ValidateUtils
	{
		public static bool IsAlphaNumeric(string input)
		{
			return input.All(char.IsLetterOrDigit);
		}
	}
}
