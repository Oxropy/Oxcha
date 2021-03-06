﻿using ORM.Attributes;
using ORM.DataContract;
using ORM.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchat.Logic.DataContract
{
    [DbTable(Name = "user_auth_key")]
    public class UserAuthKey : IDataContract
    {
        [DbField(Name = "user", Type = BaseType.Text)]
        public string User { get; set; }
        [DbField(Name = "access_token", Type = BaseType.Text)]
        public string Token { get; set; }
    }
}
