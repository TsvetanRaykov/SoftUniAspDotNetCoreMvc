﻿using Newtonsoft.Json;

namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class ProductDetail : BaseModel<int>
    {

        public string Value { get; set; }

        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }

        public int CommonDetailId { get; set; }
        public virtual CommonProductDetail CommonDetail { get; set; }
    }
}