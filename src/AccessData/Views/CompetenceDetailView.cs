using AccessData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Views
{
	public class CompetenceDetailView
	{
		/// <summary>
		/// Compétence
		/// </summary>
		public Competence Competence { get; set; }

		/// <summary>
		/// Liste des formations pour cette compétence
		/// </summary>
		public List<CatalogueFormations> Formations { get; set; }
	}
}
