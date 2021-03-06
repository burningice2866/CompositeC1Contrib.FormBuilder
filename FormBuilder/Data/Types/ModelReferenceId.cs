﻿using Composite.Data;

namespace CompositeC1Contrib.FormBuilder.Data.Types
{
    public class ModelReferenceId : IDataId
    {
        public string Source { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
