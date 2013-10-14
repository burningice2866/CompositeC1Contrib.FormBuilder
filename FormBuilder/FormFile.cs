using System.IO;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormFile
    {
        public string Key { get; set; }
        public Stream InputStream { get; set; }
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
