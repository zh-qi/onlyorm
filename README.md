# ONLYORM

写这个的初衷主要是在腾讯课堂上听了eleven老师的orm的简单封装，为了自己学习，所以就根据eleven老师的思路手写了一个简单的ORM框架。

为什么叫OnlyORM：

> 因为仅仅是一个ORM，不像EF等框架那么复杂，没有数据跟踪等特性，仅仅是为了帮助访问数据库不写SQL，实现统一访问方式的框架。

实现原理：

- 使用泛型和特性获取对应的数据库表名和字段名称
- 使用表达式目录树解析复杂的Find
- 使用策略模式实现表达式目录树的方法解析，增加可拓展性
- 使用参数化防止SQL注入
- ...

待实现：

- 暂时不支持连表查询 （待实现）
- 暂时不支持非操作（待实现）
- 暂时不支持批量复杂的Update，Delete（待实现）
- ...

# 使用步骤

1. 在配置文件中添加如下配置

```json
"OnlyOrm":{
        "ConnectString":"server=127.0.0.1;userid=root;password=1234567;database=study;",
    	/* 数据库类型，mysql或者sqlserver，大小写不敏感，暂时只支持Mysql*/
        "SqlType":"Mysql"
    }
```

2. 要调用的实体类，示例：

   - 必须继承自OnlyOrmBaseModel类,

   - TableMappingAttribute:  绑定数据库中对应的表名
   - PrppertyMappingAttribute：绑定数据库中对应的字段名
   - PrimaryKeyAttribute： 主键需要绑定该特性,true/false代表是否自增

```c#
[TableMapping("user")]
public class User : OrmBaseModel
{
    [PrimaryKey(true)]
    [PropertyMapping("Id")]
    public int Id { get; set; }

    [PropertyMapping("Name")]
    public string Name { get; set; }

    [PropertyMapping("Email")]
    public string Email { get; set; }

    [PropertyMapping("Mobile")]
    public string Mobile { get; set; }
}
```

3. 调用方式：

```C#
static void Main(string[] args)
{
	{
        // 查找主键为1的用户
        var user1 = Orm.Find<SuperUser>("1");
    }
    {
        // 组合条件查询，
        // 暂时不支持联表查询
        var user = Orm.FindWhere<SuperUser>(u => (u.NickName.Contains("zhang") || 2 == u.Id));
        Console.WriteLine(user.Count);
    }
    {
         // 暂时不支持同个条件的或，比如：（2 == u.Id || u.Id == 3）
         // 如果需要该方式查询，使用InList拓展方法，如下所示：
         var ids = new List<int>()
         {
             1,2,3,4,5
         };
         var user = Orm.FindWhere<SuperUser>(u => u.Id.InList<int>(ids));
    }
    {
        // 插入一个User实例，如果Id是主键，会被过滤掉
        var user1 = new SuperUser
        {
            Id = 3,
            NickName = "test",
            Mobile = "456",
            Email = "344@qq.com"
        };

        Orm.Insert<SuperUser>(user1);
    }
    {
        // 更新主键是3dUser
        var user1 = new SuperUser
        {
            Id = 3,
            NickName = "test1",
            Mobile = "456",
            Email = "344@qq.com"
        };
        Orm.Update<SuperUser>(user1);
    }
    {
        // 删除主键是3的User
        // Orm.Deleate<User>("3");
    }
}
```

