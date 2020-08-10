using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Models
{
	public class FormateurView
	{
		/// <summary>
		/// Pour avoir les ID Personnel et ID Formation
		/// </summary>
		public string IdPersonnel { get; set; }

		public string Nom { get; set; }

		public string Prenom { get; set; }

		public string Login { get; set; }

		public string Service { get; set; }

		public bool EstExterne { get; set; }

		/// <summary>
		/// Liste des formations dont il est formateur.
		/// </summary>
		public List<FormationView> Formations { get; set; }
	}

	public class FormationView
	{
		public int IdFormation { get; set; }

		public string TitreFormation { get; set; }
	}
}
