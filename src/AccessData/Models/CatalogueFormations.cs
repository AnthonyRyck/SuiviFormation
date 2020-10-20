using System;

namespace AccessData.Models
{
	public class CatalogueFormations
	{
		public int IdFormation { get; set; }

		public string Titre
		{
			get { return _titre; }
			set { _titre = value.Replace("'", "’"); }
		}
		private string _titre;

		public string Description
		{
			get { return _description; }
			set { _description = value.Replace("'", "’"); }
		}
		private string _description;

		public DateTime? DateDeFin { get; set; }

		public byte[] ContenuFormationN { get; set; }

		public string NomDuFichier
		{
			get { return _nomDuFichier; }
			set { _nomDuFichier = value.Replace("'", "’"); }
		}
		private string _nomDuFichier;


		public bool EstInterne { get; set; }

		public double Duree { get; set; }

		/// <summary>
		/// Type de formation.
		/// Normalement c'est un ID, mais comme il n'y a qu'une donnée
		/// dans l'autre table.
		/// </summary>
		public string TypeFormation { get; set; }


		public int TypeFormationId { get; set; }

	}
}
