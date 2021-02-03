using AccessData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ModelsValidation
{
	public class HistoriqueModelValidation
	{
		[Required(ErrorMessage = "Il faut une date de session")]
		public DateTime? DateDeSession { get; set; }

		[Required(ErrorMessage = "Il faut au moins un participants")]
		public List<Personnel> PersonnelsInscrit { get; set; }

		[Required(ErrorMessage = "Il faut une formation")]
		public CatalogueFormations Formation { get; set; }

		[Required(ErrorMessage ="Il faut choisir une salle")]
		public Salle Salle { get; set; }

		[Required(ErrorMessage ="Il faut choisir un formateur")]
		public FormateurView Formateur { get; set; }

		public HistoriqueModelValidation()
		{
			
		}
	}
}
