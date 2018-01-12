using System.Collections.Generic;

namespace EnumLister.Model
{
    public class EnumModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<EnumItemModel> Items { get; set; }
    }
}
