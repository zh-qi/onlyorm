# ONLYORM

初衷主要是在腾讯课堂上听了eleven老师的orm的简单封装，为了自己学习，所以就根据eleven老师的思路手写了一个简单的ORM框架。

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
	var user = Orm.Find<User>(1);
}
```

