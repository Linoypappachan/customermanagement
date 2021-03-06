﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVC_CustomerManagement_Utilities.Helper
{
    //public class LowercaseJsonSerializer
    //{
    //    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //    {
    //        ContractResolver = new LowercaseContractResolver()
    //    };

    //    public static string SerializeObject(object o)
    //    {
    //        return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
    //    }

    //    public class LowercaseContractResolver : DefaultContractResolver
    //    {
    //        protected override string ResolvePropertyName(string propertyName)
    //        {
    //            return propertyName.ToLower();
    //        }
    //    }
    //}

    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
