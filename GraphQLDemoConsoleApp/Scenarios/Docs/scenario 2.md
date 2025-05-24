# S2 Code Styles

在 **GraphQL.NET** 中，**Code First** 和 **Schema First** 是两种不同的开发方式，用于定义 GraphQL 模式（Schema）和解析器（Resolver）。

| 特性               | Schema First                                                                                  | Code First                                           |
| :----------------- | :-------------------------------------------------------------------------------------------- | :--------------------------------------------------- |
| **定义方式** | 使用 GraphQL 的模式定义语言（Schema Definition Language， SDF）来定义模式，然后编写解析器代码 | 通过编写代码（通常是 C# 类和方法）来定义模式和解析器 |
| **开发效率** | 高，维护 SDL 文件即可                                                                         | 低，需要维护代码                                     |
| **类型安全** | 否，SDL 是文本格式，无法在编译时检查类型错误                                                  | 是，编译时检查类型错误，减少运行时错误               |
| **学习曲线** | 低，SDL 语法直观，易于理解                                                                    | 适中，需要熟悉代码优先方式                           |
| **模式变更** | 在解析器适配灵活的前提下，只需要更新 SDL，无需编译 ；否则解析器可能也需要更新，且需要编译     | 需要修改代码，重新提交编译                           |

## 1 Schema First

首先使用 GraphQL 的模式定义语言（SDL）来定义模式，然后编写解析器代码。

- 定义Query的解析

```c#
public class QueryS2
{
    // Resolver Method : This method will be called when the "hero" query is executed.
    [GraphQLMetadata("hero")]
    public Droid GetHero()
    {
        return new Droid { Id = "1", Name = "R2-D2" };
    }
}
```

- 定义Schema并实现查询

```c#
public static async Task _02Query_SchemaFirst()
{
    ISchema schema = Schema.For(@"
type Droid {
id: String!
name: String!
}

type Query {
hero: Droid
}
", _ =>
    {
        _.Types.Include<Query>();
    });

    string query = "{ hero { id name } }";

    var json = await schema.ExecuteAsync(_ =>
        {
            _.Query = query;
        });


    Console.WriteLine($"Query: {query}");
    Console.WriteLine("---------");
    Console.WriteLine(json);

}
```

### **`Schema.For` 方法**

`Schema.For` 是一个用于创建GraphQL模式的方法。它接受两个参数：

- **GraphQL模式的SDL字符串**：定义了GraphQL模式中的类型和字段。
- **一个配置委托**：用于进一步配置GraphQL模式，例如注册类型、中间件等。

### **SDL字符串**

- **`type Droid`**：定义了一个GraphQL对象类型 `Droid`，它有两个字段：`id` 和 `name`。`id` 和 `name` 都是 `String` 类型，并且是必填字段（`!` 表示非空）。
- **`type Query`**：定义了查询类型 `Query`，它有一个字段 `hero`，返回类型是 `Droid`。

### **配置委托中的 _.Types.Include`<Query>`()**

- **`_.Types`**：表示GraphQL模式中的类型集合。
- **`Include<Query>()`**：将 `Query` 类型注册到GraphQL模式中。

在GraphQL中，`Query` 类型是**根查询类型**，它定义了客户端可以发起的查询操作。`_.Types.Include<Query>()` 的作用是将 `Query` 类型注册到GraphQL模式中，使得GraphQL运行时能够识别和处理 `Query` 类型中的字段。

### 为什么需要注册 `Query` 类型？

在GraphQL中，客户端通过查询类型（`Query`）来请求数据。查询类型中的每个字段都对应一个解析器（resolver），解析器负责从后端数据源中获取数据并返回给客户端。

如果你不注册 `Query` 类型，GraphQL运行时将无法识别 `Query` 类型中的字段，也就无法执行查询操作。因此，`_.Types.Include<Query>()` 是确保 `Query` 类型及其字段能够被正确解析和处理的关键步骤。

### 运行示例

```cmd
dotnet run 2
```

执行结果

```bash
# Run _02Query_SchemaFirst
Query: query { hero { id name } }
---------
{
  "data": {
    "hero": {
      "id": "1",
      "name": "R2-D2"
    }
  }
}
```

## 2 Code First

`Code Frist`需要根据对象类型定义GraphQL的数据模型，然后再实现类型的解析。

### **Droid 类**

定义数据模型

```c#
public class Droid
{
    public string Id { get; set; }
    public string Name { get; set; }
}
```

定义基本的Droid数据类型。

### `DroidType` 类

定义Graph数据类型， 需要继承 `ObjectGraphType<T>`

```c#

public class DroidType : ObjectGraphType<Droid>
{
    public DroidType()
    {
        // 解析字段
        Field(x => x.Id).Description("The Id of the Droid.");
        Field(x => x.Name).Description("The name of the Droid.");
    }
}
```

继承 `ObjectGraphType<Droid>`

- **`ObjectGraphType<Droid>`**：`DroidType` 继承自 `ObjectGraphType<T>`，其中 `T` 是 `Droid` 类型。这表示 `DroidType` 是一个GraphQL对象类型，用于描述 `Droid` 类型的字段和解析逻辑。

构造函数

- **`Field(x => x.Id).Description("The Id of the Droid.")`**：
  - **`Field`**：定义了一个GraphQL字段 `id`。
  - **`x => x.Id`**：指定字段 `id` 的值从 `Droid` 对象的 `Id` 属性中获取。
  - **`.Description("The Id of the Droid.")`**：为字段 `id` 添加描述，方便客户端理解字段的含义。
- **`Field(x => x.Name).Description("The name of the Droid.")`**：
  - **`Field`**：定义了一个GraphQL字段 `name`。
  - **`x => x.Name`**：指定字段 `name` 的值从 `Droid` 对象的 `Name` 属性中获取。
  - **`.Description("The name of the Droid.")`**：为字段 `name` 添加描述。

### `StarWarsQuery` 类

```c#
public class StarWarsQuery : ObjectGraphType
{
    public StarWarsQuery()
    {
        // 解析模型
        Field<DroidType>("hero")
            .Resolve(context => new Droid { Id = "1", Name = "R2-D2" });
    }
}
```

继承 `ObjectGraphType`

- **`ObjectGraphType`**：`StarWarsQuery` 继承自 `ObjectGraphType`，表示这是一个GraphQL查询类型。查询类型是GraphQL模式中的根类型，用于定义客户端可以发起的查询操作。

构造函数

- **`Field<DroidType>("hero")`**：
  - **`Field`**：定义了一个GraphQL字段 `hero`。
  - **`DroidType`**：指定字段 `hero` 的返回类型是 `DroidType`，即 `Droid` 类型。
  - **`"hero"`**：字段的名称，客户端可以通过这个名称来请求数据。
- **`.Resolve(context => new Droid { Id = "1", Name = "R2-D2" })`**：
  - **`Resolve`**：定义了字段 `hero` 的解析逻辑。
  - **`context`**：表示当前字段解析的上下文，提供了关于请求的上下文信息。
  - **`new Droid { Id = "1", Name = "R2-D2" }`**：解析器返回一个 `Droid` 对象，其 `Id` 为 `"1"`，`Name` 为 `"R2-D2"`。

关于 `context`, 后面会带来更多的解释和示例。

### 运行示例

```cmd
dotnet run 3
```

执行结果

```bash
# Run _03Query_CodeFirst
Query: { hero { id name } }
---------
{
  "data": {
    "hero": {
      "id": "1",
      "name": "R2-D2"
    }
  }
}
```

## 3 Annotation First

Annotation First是Code First的一种特殊形式. 通过属性标记的方式声明了Graph 类型。

### DroidType2 类

```c#
[GraphQLMetadata("Droid", IsTypeOf = typeof(Droid))]
public class DroidType2
{
    public string Id([FromSource] Droid droid) => droid.Id;
    public string Name([FromSource] Droid droid) => droid.Name;

    // these two parameters are optional
    // IResolveFieldContext provides contextual information about the field
    public Character Friend(IResolveFieldContext context, [FromSource] Droid source)
    {
        return new Character { Name = "C3-PO" };
    }
}

```

- **`[GraphQLMetadata]`**：这是一个用于标记Graph类型元数据的属性。

    **`"Droid"`**：指定这个类型在GraphQL模式中的名称;

    **`IsTypeOf = typeof(Droid)`**：指定这个类型对应的CLR类型是 `Droid`。这告诉GraphQL运行时，当处理GraphQL类型 `Droid` 时，应该使用这个 `DroidType2` 类来解析字段。

- **`DroidType2`**：这是一个GraphQL类型类，用于定义GraphQL类型 `Droid` 的字段和解析逻辑。
- **`[FromSource] Droid droid`**：`FromSource` 属性表示这个参数是从GraphQL的上下文(IResolveFieldContext )中获取的Source源对象。在这个例子中，`droid` 是一个 `Droid` 类型的对象，表示当前正在解析的 `Droid` 实例。
- **`Friend`**：定义了一个GraphQL字段 `friend`，返回类型是 `Character`。
- **`IResolveFieldContext context`**：`IResolveFieldContext` 提供了关于当前字段解析的上下文信息，例如请求的参数、上下文环境等。虽然在这个例子中没有使用，但它在复杂场景中非常有用。

```c#
public static async Task _04Query_Multi_TopLevel_SchemaFirst()
{
    var schema = Schema.For(@"
  type Droid {
    id: String!
    name: String!
    friend: Character
  }

  type Character {
    name: String!
  }

  type Query {
    hero: Droid
  }
", builder =>
    {
        builder.Types.Include<DroidType2>();
        builder.Types.Include<Query>();
    });

    string query = "{ hero { id name friend { name } } }";
    var json = await schema.ExecuteAsync(_ =>
    {
        _.Query = query;
    });

    Console.WriteLine($"Query: {query}");
    Console.WriteLine("---------");
    Console.WriteLine(json);
}
```

### 运行示例

```cmd
dotnet run 4
```

执行结果

```bash
# Run _04Query_Multi_TopLevel_SchemaFirst
Query: { hero { id name friend { name } } }
---------
{
  "data": {
    "hero": {
      "id": "1",
      "name": "R2-D2",
      "friend": {
        "name": "C3-PO"
      }
    }
  }
}
```
