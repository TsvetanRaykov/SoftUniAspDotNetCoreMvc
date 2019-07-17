using System.Collections.Generic;

namespace Vxp.Web.Areas.Administration.ViewModels.Dashboard
{
    public class IndexViewModel 
    {
        public int SettingsCount { get; set; }

        public IEnumerable<UserViewModel> Users { get; set; }
    }
}
