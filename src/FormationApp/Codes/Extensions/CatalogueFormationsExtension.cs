using AccessData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Codes.Extensions
{
	public static class CatalogueFormationsExtension
	{

		public static FormationView ToFormationView(this CatalogueFormations catalogue)
		{
			return new FormationView() { IdFormation = catalogue.IdFormation, TitreFormation = catalogue.Titre };
		}

	}
}
