using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace AuthExam.Models
{
    public class UserResourceViewModel
    {
        public IEnumerable<string> FileNames { get; set; }
        public string Directory { get; set; }
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }
}