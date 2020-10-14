using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Models
{
	public class CompetenceUserView
	{

		public int IdCompetence { get; set; }

		public string Titre { get; set; }

		public string Description { get; set; }

		/// <summary>
		/// Tous les niveaux SAME pour cette compétence
		/// </summary>
		public List<SameView> Same { get; set; }

	}
}
