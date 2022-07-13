using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluskMasterUpdate.m2;

namespace FluskMasterUpdate.Models
{
    public class BlogsBLL:GenericRepository<Blogs>
    {
    }
    public partial class Blogs
    {
        public string SEOLink
        {
            get
            {
                return GenelAraclarBLL.Duzelt(Adi);
            }
        }
    }
}