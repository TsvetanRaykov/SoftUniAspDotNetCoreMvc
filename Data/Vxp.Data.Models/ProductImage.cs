﻿namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class ProductImage : BaseModel<int>
    {
        public string Url { get; set; }

        public string Alt { get; set; }

        public string Title { get; set; }
    }
}