using System;
using System.Collections.Generic;
using System.Text;

namespace VVVV.Nodes.jQuery
{
	class Method : IScriptGenerator
	{
		private string FName;

		public string PName
		{
			set { FName = value; }
		}

		public string PScript
		{
			get
			{
				return FName;
			}
		}

	}
}
