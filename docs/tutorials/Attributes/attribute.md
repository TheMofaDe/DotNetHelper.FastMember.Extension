# Custom Attributes
In the secnarios where you need to build Update,Delete, or Upsert Statements. Attributes are use to generate the where clause. This library has its own custom attributes and can also work with the common DataAnnotation attributes. With the support of DataAnnotation this means this library could be paired with your favorite orm like Dapper or Enitity Framework 

##### Mark a property as an identity fields. 
```csharp
[SqlColumn(SetIsIdentityKey = true)]
OR 
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
```

##### Mark a property as a key field. 
```csharp
[SqlColumn(SetIsIdentityKey = true)]
OR 
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
```






<!-- 
#### Storing Columns As CSV, XML, & JSON

```csharp
[SqlColumn(SerializableType = SerializableType.Json)]
or 
[SqlColumn(SerializableType = SerializableType.Xml)]
or 
[SqlColumn(SerializableType = SerializableType.Csv)]
```  -->