using FluskMasterUpdate.m2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FluskMasterUpdate.Models
{
    public class IletisimBLL:GenericRepository<Iletisim>
    {
    }
    public partial class Iletisim
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