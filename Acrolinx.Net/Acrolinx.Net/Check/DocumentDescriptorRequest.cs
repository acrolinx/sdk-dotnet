using System.Collections.Generic;

namespace Acrolinx.Net.Check
{
    public class DocumentDescriptorRequest
    {
        public string Reference { get; set; }
        public List<CustomField> CustomFields { get; set; }

        public DocumentDescriptorRequest(string reference, List<CustomField> customFields)
        {
            Reference = reference;
            CustomFields = customFields;
        }
    }
}
