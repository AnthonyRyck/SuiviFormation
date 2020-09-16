using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Models
{
	public class CompetenceView
	{
		/// <summary>
		/// Compétence
		/// </summary>
		public Competence Competence { get; set; }

		/// <summary>
		/// Liste des formations pour cette compétence
		/// </summary>
		public List<FormationView> FormationViews { get; set; }
	}
}
