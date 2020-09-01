using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Models
{
	public class SessionInscritUserView
	{
		public int IdSession { get; set; }

		public string NomFormateur { get; set; }

		public string PrenomFormateur { get; set; }
		public bool IsExterne { get; set; }

		public string TitreFormation { get; set; }

		public double NombreJourFormation { get; set; }

		public DateTime DateDeLaFormation { get; set; }

		public bool IsFormationValide { get; set; }

		public int? Note { get; set; }

		public string? Commentaire { get; set; }

	}
}
