namespace Acrolinx.Net.Check
{
    public class CheckRequest
    {
        public string Content { get; set; }
        public string ContentEncoding { get; set; }
        public CheckOptions CheckOptions { get; set; }
        public DocumentDescriptorRequest Document { get; set; }
    }
}
