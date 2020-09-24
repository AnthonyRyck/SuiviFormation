using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class DetailFormationPage
	{
		[Parameter]
		public int IdFormation { get; set; }
	}
}
