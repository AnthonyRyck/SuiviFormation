using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Models
{
	/// <summary>
	/// Utilisé pour la visu sur le Scheduler
	/// </summary>
	public class DataSession
	{
		public DateTime Start { get; set; }
		
		public DateTime End { get; set; }
		
		public string Text { get; set; }

	}
}
