﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.DL;

namespace TodoApp.BL
{
    public class UserBL : BaseBL, IUserBL
    {
        public UserBL(IUserDL dl) : base(dl)
        {
        }
    }
}
