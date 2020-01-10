using OnlyOrm.Attributes;

namespace OnlyOrm.Test
{
    [TableMapping("user")]
    public class SuperUser : OrmBaseModel
    {
        [PrimaryKey(true)]
        [PropertyMapping("Id")]
        public int Id { get; set; }

        [PropertyMapping("Name")]
        public string NickName { get; set; }

        [PropertyMapping("Email")]
        public string Email { get; set; }

        [PropertyMapping("Mobile")]
        public string Mobile { get; set; }
    }
}