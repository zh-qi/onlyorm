using OnlyOrm.Attributes;

namespace OnlyOrm.Test
{
    [TableMappingAttribute("user")]
    public class User : OrmBaseModel
    {
        [PrimaryKeyAttribute(true)]
        [PropertyMappingAttribute("Id")]
        public int Id { get; set; }

        [PropertyMappingAttribute("Name")]
        public string Name { get; set; }

        [PropertyMappingAttribute("Email")]
        public string Email { get; set; }

        [PropertyMappingAttribute("Mobile")]
        public string Mobile { get; set; }
    }
}