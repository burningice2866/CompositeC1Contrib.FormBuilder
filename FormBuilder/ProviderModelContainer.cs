using System;

using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder
{
    public class ProviderModelContainer
    {
        public Type Source { get; set; }
        public string Type { get; set; }
        public IFormModel Model { get; set; }
        public IFunction Function { get; set; }
    }
}