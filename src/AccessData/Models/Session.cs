using System;

namespace AccessData.Models
{
	public class Session
	{
		public int IdSession { get; set; }

		public int IdFormation { get; set; }
				
		public int IdSalle { get; set; }
		
		public string IdFormateur { get; set; }

		public DateTime DateSession { get; set; }

		public int PlaceDispo { get; set; }

		public byte[] ScanEmargement { get; set; }

		public string? NomFichierEmargement { get; set; }

	}
}
