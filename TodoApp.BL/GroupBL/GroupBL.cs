using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.DL;

namespace TodoApp.BL
{
    public class GroupBL : BaseBL, IGroupBL
    {
        public GroupBL(IGroupDL dl) : base(dl)
        {
        }
    }
}
