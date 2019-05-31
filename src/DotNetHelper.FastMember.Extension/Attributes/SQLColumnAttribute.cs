//using System;

//namespace DotNetHelper.FastMember.Extension.Attributes
//{
//    /// <inheritdoc />
//    /// <summary>
//    /// This specifies that the following property is also an SQL table
//    /// </summary>
//    /// <seealso cref="T:System.Attribute" />
//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
//    public class SqlTableAttribute : System.Attribute
//    {

//        /// <summary>
//        /// The Sql Table name that this class data belongs to.
//        /// </summary>
//        /// <value>The map to.</value>
//        public string TableName { get; set; } = null;

//        public Type XReferenceTable { get; set; } = null;

//        /// <summary>
//        /// Gets or sets a value indicating whether [x reference on delete cascade].
//        /// </summary>
//        /// <value><c>null</c> if [x reference on delete cascade] contains no value, <c>true</c> if [x reference on delete cascade]; otherwise, <c>false</c>.</value>
//        public SQLJoinType JoinType { get; set; }
//        /// <summary>
//        /// Gets or sets a value indicating whether [setx reference on delete cascade].
//        /// </summary>
//        /// <value><c>true</c> if [setx reference on delete cascade]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public SQLJoinType SetJoinType
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => JoinType = value;
//        }



//    }


//    /// <summary>
//    /// Enum SqlColumnAttritubeMembers
//    /// </summary>
//    public enum SqlTableAttritubeMembers
//    {

//        JoinType
//        , TableName
//        , XReferenceTable
//    }


//    /// <inheritdoc />
//    /// <summary>
//    /// Class SqlColumnAttritube.
//    /// </summary>
//    /// <seealso cref="T:System.Attribute" />
//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
//    public class SqlColumnAttribute : System.Attribute
//    {
//        /// <summary>
//        /// Defaults To MAX used for creating table
//        /// </summary>
//        /// <value>The maximum size of the column.</value>
//        public int? MaxColumnSize { get; set; }
//        /// <summary>
//        /// Gets or sets the size of the set maximum column.
//        /// </summary>
//        /// <value>The size of the set maximum column.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public int SetMaxColumnSize
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => MaxColumnSize = value;
//        }

//        /// <summary>
//        /// Defaults To MAX used for creating table
//        /// </summary>
//        /// <value>The maximum size of the column.</value>
//        public SerializableType SerializableType { get; set; } = SerializableType.NONE;
//        ///// <summary>
//        ///// Gets or sets the size of the set maximum column.
//        ///// </summary>
//        ///// <value>The size of the set maximum column.</value>
//        ///// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        //public SerializableType SetSerializableType
//        //{
//        //    get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//        //    set => SerializableType = value;
//        //}

//        /// <summary>
//        /// Gets or sets the automatic increment by. If this value is set then the property will be treated as an IDENTITY column
//        /// </summary>
//        /// <value>The automatic increment by.</value>
//        public int? AutoIncrementBy { get; set; } = null;
//        /// <summary>
//        /// Gets or sets the set automatic increment by.
//        /// </summary>
//        /// <value>The set automatic increment by.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public int SetAutoIncrementBy
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => AutoIncrementBy = value;
//        }

//        /// <summary>
//        /// Gets or sets the start increment at.
//        /// </summary>
//        /// <value>The start increment at.</value>
//        public int? StartIncrementAt { get; set; }
//        /// <summary>
//        /// Gets or sets the set start increment at.
//        /// </summary>
//        /// <value>The set start increment at.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public int SetStartIncrementAt
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => StartIncrementAt = value;
//        }

//        /// <summary>
//        /// Gets or sets a value indicating whether [UTC date time].
//        /// </summary>
//        /// <value><c>null</c> if [UTC date time] contains no value, <c>true</c> if [UTC date time]; otherwise, <c>false</c>.</value>
//        public bool? UtcDateTime { get; set; } = null;
//        /// <summary>
//        /// Gets or sets a value indicating whether [set UTC date time].
//        /// </summary>
//        /// <value><c>true</c> if [set UTC date time]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetUtcDateTime
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => UtcDateTime = value;
//        }
//        /// <summary>
//        /// Gets or sets a value indicating whether [primary key].
//        /// </summary>
//        /// <value><c>null</c> if [primary key] contains no value, <c>true</c> if [primary key]; otherwise, <c>false</c>.</value>
//        public bool? PrimaryKey { get; set; } = null;
//        /// <summary>
//        /// Gets or sets a value indicating whether [set primary key].
//        /// </summary>
//        /// <value><c>true</c> if [set primary key]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetPrimaryKey
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => PrimaryKey = value;
//        }


//        /// <summary>
//        /// Gets or sets the type of the x reference table.
//        /// </summary>
//        /// <value>The type of the x reference table.</value>
//        public Type xRefTableType { get; set; } = null;
//        /// <summary>
//        /// Gets or sets the type of the setx reference table.
//        /// </summary>
//        /// <value>The type of the setx reference table.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public Type SetxRefTableType
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => xRefTableType = value;
//        }




//        ///// <summary>
//        ///// Gets or sets the mappings for keys to join with.
//        ///// </summary>
//        ///// <value>The x reference table schema.</value>
//        //internal Dictionary<string, string> KeysToJoinWith { get; set; } = null;

//        ///// <summary>
//        ///// Gets or sets the mappings for keys to join with.
//        ///// </summary>
//        ///// <value>The type of the setx reference table.</value>
//        ///// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        //public Dictionary<string, string> SetKeysToJoinWith
//        //{
//        //    get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//        //    set => KeysToJoinWith = value;
//        //}


//        /// <summary>
//        /// Gets or sets the x reference table schema.
//        /// </summary>
//        /// <value>The x reference table schema.</value>
//        public string xRefTableSchema { get; set; } = null;


//        /// <summary>
//        /// Gets or sets the name of the x reference table.
//        /// </summary>
//        /// <value>The name of the x reference table.</value>
//        public string xRefTableName { get; set; } = null;


//        /// <summary>
//        /// Gets or sets the x reference join on column.
//        /// </summary>
//        /// <value>The x reference join on column.</value>
//        public string xRefJoinOnColumn { get; set; } = null;


//        /// <summary>
//        /// Gets or sets a value indicating whether [x reference on update cascade].
//        /// </summary>
//        /// <value><c>null</c> if [x reference on update cascade] contains no value, <c>true</c> if [x reference on update cascade]; otherwise, <c>false</c>.</value>
//        public bool? xRefOnUpdateCascade { get; set; } = null;

//        /// <summary>
//        /// Gets or sets a value indicating whether [setx reference on update cascade].
//        /// </summary>
//        /// <value><c>true</c> if [setx reference on update cascade]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetxRefOnUpdateCascade
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            internal set => xRefOnUpdateCascade = value;
//        }


//        /// <summary>
//        /// Gets or sets a value indicating whether [x reference on delete cascade].
//        /// </summary>
//        /// <value><c>null</c> if [x reference on delete cascade] contains no value, <c>true</c> if [x reference on delete cascade]; otherwise, <c>false</c>.</value>
//        public bool? xRefOnDeleteCascade { get; set; } = null;
//        /// <summary>
//        /// Gets or sets a value indicating whether [setx reference on delete cascade].
//        /// </summary>
//        /// <value><c>true</c> if [setx reference on delete cascade]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetxRefOnDeleteCascade
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            internal set => xRefOnDeleteCascade = value;
//        }


//        /// <summary>
//        /// Gets or sets a value indicating whether this <see cref="SqlColumnAttritube"/> is nullable.
//        /// </summary>
//        /// <value><c>null</c> if [nullable] contains no value, <c>true</c> if [nullable]; otherwise, <c>false</c>.</value>
//        public bool? Nullable { get; set; } = null;
//        /// <summary>
//        /// Gets or sets a value indicating whether [set nullable].
//        /// </summary>
//        /// <value><c>true</c> if [set nullable]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetNullable
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => Nullable = value;
//        }
//        /// <summary>
//        /// Gets or sets a value indicating whether [API identifier].
//        /// </summary>
//        /// <value><c>null</c> if [API identifier] contains no value, <c>true</c> if [API identifier]; otherwise, <c>false</c>.</value>
//        public bool? ApiId { get; set; } = null;
//        /// <summary>
//        /// Gets or sets a value indicating whether [set API identifier].
//        /// </summary>
//        /// <value><c>true</c> if [set API identifier]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetApiId
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => ApiId = value;
//        }
//        /// <summary>
//        /// When A Record Is Be Inserted Or Updated This Column Value Will Be DateTime.Now
//        /// </summary>
//        /// <value><c>null</c> if [synchronize time] contains no value, <c>true</c> if [synchronize time]; otherwise, <c>false</c>.</value>
//        public bool? SyncTime { get; set; } = null;
//        /// <summary>
//        /// Gets or sets a value indicating whether [set synchronize time].
//        /// </summary>
//        /// <value><c>true</c> if [set synchronize time]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetSyncTime
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => SyncTime = value;
//        }

//        /// <summary>
//        /// If true property will be use when the class is being used by a DATASOURCE Object
//        /// </summary>
//        /// <value><c>null</c> if [ignore] contains no value, <c>true</c> if [ignore]; otherwise, <c>false</c>.</value>
//        public bool? Ignore { get; set; } = null;
//        /// <summary>
//        /// Gets or sets a value indicating whether [set ignore].
//        /// </summary>
//        /// <value><c>true</c> if [set ignore]; otherwise, <c>false</c>.</value>
//        /// <exception cref="Exception">Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial</exception>
//        public bool SetIgnore
//        {
//            get => throw new Exception("Nooo...  Your Using SqlColumnAttritube wrong do not try to get from the Set Property use the orignial ");
//            set => Ignore = value;
//        }

//        /// <summary>
//        /// Gets or sets the default value.
//        /// </summary>
//        /// <value>The default value.</value>
//        public object DefaultValue { get; set; }

//        /// <summary>
//        /// Gets or sets the default value THIS IS ONLY WHEN THIS LIBRARY IS CREATING A TABLE SCRIPT 
//        /// </summary>
//        /// <value>The default value.</value>
//        public string TSQLDefaultValue { get; set; }

//        /// <summary>
//        /// If true property will be use when the class is being used by a DATASOURCE Object
//        /// </summary>
//        /// <value>The map to.</value>
//        public string MapTo { get; set; } = null;



//        /// <summary>
//        /// Gets or sets the mappings for keys to join with.
//        /// </summary>
//        /// <value>an array of ids, that will join a column to another table.</value>
//        public string[] MappingIds = null;

//    }
//}
