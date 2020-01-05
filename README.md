# ONLYORM

初衷主要是在腾讯课堂上听了eleven老师的orm的简单封装，为了自己学习，所以就根据eleven老师的思路手写了一个简单的ORM框架。

# 使用步骤

1. 在配置文件中添加如下配置：

```json
"OnlyOrm":{
    	// 链接字符串
        "ConnectString":"",
    	// 数据库类型，mysql或者sqlserver，大小写不敏感
        "SqlType":"Mysql"
    }
```

2. 要调用的实体类，必须继承自OnlyOrmBaseModel类

```c#
public class User:OnlyOrmBaseModel
{
	//...
}
```

