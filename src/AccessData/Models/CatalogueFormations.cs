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

	}
}
