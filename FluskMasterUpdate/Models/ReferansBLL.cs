using FluskMasterUpdate.m2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FluskMasterUpdate.Models
{
    public class ReferansBLL : GenericRepository<Referans>
    {
    }
    public partial class Referans
    {
        public string SEOLink
        {
            get
            {
                return GenelAraclarBLL.Duzelt(AdiSoyadi);
            }
        }
    }
}