using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace NeuLis.DataBase
{
    public class OracleHelp
    {
		/// <summary>
		/// 数据库连接串
		/// </summary>
		private static string _conn;
		private static string GetConn()
		{
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.ini");//在当前程序路径创建
            string path = Neusoft.HisDecrypt.Decrypt(ReadIni.Read("LISPATH", "path", "", filePath));//NeuLis.DataBase.ReadIni.Read("LISPATH", "path", "", filePath);
            //设置数据库连接串到全局变量 
            return path;

        }

        #region 执行SQL语句,返回受影响行数
        public static int ExecuteNonQuery(string sql )
        {
            if(string.IsNullOrEmpty(_conn))
            {
                _conn = GetConn();
            }
            using (OracleConnection conn = new OracleConnection(_conn))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region 执行SQL语句,返回DataTable;只用来执行查询结果比较少的情况
        public static DataTable Query(string sql )
        {
            if (string.IsNullOrEmpty(_conn))
            {
                _conn = GetConn();
            }
            using (OracleConnection conn = new OracleConnection(_conn))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable datatable = new DataTable();
                    adapter.Fill(datatable);
                    return datatable;
                }
            }
        }
        #endregion

        /// <summary>
        /// 扩展查询 返回实体(反射) 适用于数据量小
        /// {95FBCF77-F950-46C1-8591-D580B2B07632}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static List<T> QueryListByReflect<T>(string strSql) where T : new()
        {
            List<T> ts = new List<T>();
            try
            {

                DataTable dt = Query(strSql);
                if (dt == null || dt.Rows.Count == 0)
                    return ts;

                string tempName = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    T t = new T();
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        tempName = pi.Name;
                        try
                        {
                            if (dt.Columns.Contains(tempName))
                            {
                                if (pi.PropertyType.Name.ToLower() == "string")
                                    pi.SetValue(t, dr[tempName].ToString(), null);
                                else if (pi.PropertyType.Name.ToLower() == "int32")
                                {
                                    pi.SetValue(t, int.Parse(dr[tempName].ToString()), null);
                                }
                                else
                                    pi.SetValue(t, dr[tempName], null);

                            }

                        }
                        catch (Exception ex)
                        {
                            string aa = ex.Message;
                        }
                    }
                    ts.Add(t);
                }

                return ts;
            }
            catch (Exception e)
            {
                return ts;
            }
        }

        #region 动态加载

        //把DataRow转换为对象的委托声明
        private delegate T Load<T>(DataRow dataRecord);

        //用于构造Emit的DataRow中获取字段的方法信息
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });

        //用于构造Emit的DataRow中判断是否为空行的方法信息
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });

        //使用字典存储实体的类型以及与之对应的Emit生成的转换方法
        private static Dictionary<Type, Delegate> rowMapMethods = new Dictionary<Type, Delegate>();
        /// <summary>
        /// 扩展查询 返回实体(动态加载) 试用于数据量大 需要查询出来的与实体的字段类型一致否则不能转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSql"></param>
        public static List<T> QueryListByEmit<T>(string strSql)
        {
            List<T> list = new List<T>();
            try
            {
                DataTable dt = Query(strSql);
                if (dt == null || dt.Rows.Count == 0)
                    return list;
                //声明 委托Load<T>的一个实例rowMap
                Load<T> rowMap = null;


                //从rowMapMethods查找当前T类对应的转换方法，没有则使用Emit构造一个。
                if (!rowMapMethods.ContainsKey(typeof(T)))
                {
                    DynamicMethod method = new DynamicMethod("DynamicCreateEntity_" + typeof(T).Name, typeof(T), new Type[] { typeof(DataRow) }, typeof(T), true);
                    ILGenerator generator = method.GetILGenerator();
                    LocalBuilder result = generator.DeclareLocal(typeof(T));
                    generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                    generator.Emit(OpCodes.Stloc, result);

                    for (int index = 0; index < dt.Columns.Count; index++)
                    {
                        PropertyInfo propertyInfo = typeof(T).GetProperty(dt.Columns[index].ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        System.Reflection.Emit.Label endIfLabel = generator.DefineLabel();
                        if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                        {
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Ldc_I4, index);
                            generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                            generator.Emit(OpCodes.Brtrue, endIfLabel);
                            generator.Emit(OpCodes.Ldloc, result);
                            generator.Emit(OpCodes.Ldarg_0);
                            generator.Emit(OpCodes.Ldc_I4, index);
                            generator.Emit(OpCodes.Callvirt, getValueMethod);
                            generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                            generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                            generator.MarkLabel(endIfLabel);
                        }
                    }
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ret);

                    //构造完成以后传给rowMap
                    rowMap = (Load<T>)method.CreateDelegate(typeof(Load<T>));
                }
                else
                {
                    rowMap = (Load<T>)rowMapMethods[typeof(T)];
                }

                //遍历Datatable的rows集合，调用rowMap把DataRow转换为对象（T）
                foreach (DataRow info in dt.Rows)
                    list.Add(rowMap(info));
            }
            catch(Exception ex)
            { }
            return list;

        }



        #endregion

        #region 新转换方法

        //数据类型和对应的强制转换方法的methodinfo，供实体属性赋值时调用
        private static Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
       {
           {typeof(int),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})},
           {typeof(Int16),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})},
           {typeof(Int64),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})},
           {typeof(DateTime),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})},
           {typeof(decimal),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})},
           {typeof(Double),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
           {typeof(Boolean),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
           {typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})}
       };
        //把datarow转换为实体的方法的委托定义
        public delegate T LoadDataRow<T>(DataRow dr);
        //emit里面用到的针对datarow的元数据信息
        private static readonly AssembleInfo dataRowAssembly = new AssembleInfo(typeof(DataRow));
        /// <summary>
        /// 构造转换动态方法（核心代码），根据assembly可处理datarow和datareader两种转换
        /// </summary>
        /// <typeparam name="T">返回的实体类型</typeparam>
        /// <param name="assembly">待转换数据的元数据信息</param>
        /// <returns>实体对象</returns>
        private static DynamicMethod BuildMethod<T>(AssembleInfo assembly)
        {
            DynamicMethod method = new DynamicMethod(assembly.MethodName + typeof(T).Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(T),
                    new Type[] { assembly.SourceType }, typeof(T).Module, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                System.Reflection.Emit.Label endIfLabel = generator.DefineLabel();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Callvirt, assembly.CanSettedMethod);
                generator.Emit(OpCodes.Brfalse, endIfLabel);
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Callvirt, assembly.GetValueMethod);
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                    generator.Emit(OpCodes.Call, ConvertMethods[property.PropertyType]);
                else
                    generator.Emit(OpCodes.Castclass, property.PropertyType);
                generator.Emit(OpCodes.Callvirt, property.GetSetMethod());
                generator.MarkLabel(endIfLabel);
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            return method;
        }
        /// <summary>
        /// 从Cache获取委托 LoadDataRow<T>的方法实例，没有则调用BuildMethod构造一个。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static LoadDataRow<T> GetDataRowMethod<T>()
        {
            //string key = dataRowAssembly.MethodName + typeof(T).Name;
            LoadDataRow<T> load = null;
            //if (HttpRuntime.Cache[key] == null)
            //{
            //    load = (LoadDataRow<T>)BuildMethod<T>(dataRowAssembly).CreateDelegate(typeof(LoadDataRow<T>));
            //    HttpRuntime.Cache[key] = load;
            //}
            //else
            //{
            //    load = HttpRuntime.Cache[key] as LoadDataRow<T>;
            //}
            return load;
        }
        public static List<T> QueryList<T>(string strSql)
        {
            List<T> list = new List<T>();
            DataTable dt = Query(strSql);
            if (dt == null || dt.Rows.Count == 0)
                return list;
            LoadDataRow<T> load = GetDataRowMethod<T>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(load(dr));
            }
            return list;
        }

        #endregion

        public class AssembleInfo
        {
            public AssembleInfo(Type type)
            {
                SourceType = type;
                MethodName = "Convert" + type.Name + "To";
                CanSettedMethod = this.GetType().GetMethod("CanSetted", new Type[] { type, typeof(string) });
                GetValueMethod = type.GetMethod("get_Item", new Type[] { typeof(string) });
            }
            public string MethodName;
            public Type SourceType;
            public MethodInfo CanSettedMethod;
            public MethodInfo GetValueMethod;

            /// <summary>
            /// 判断datareader是否存在某字段并且值不为空
            /// </summary>
            /// <param name="dr">当前的datareader</param>
            /// <param name="name">字段名</param>
            /// <returns></returns>
            public static bool CanSetted(IDataRecord dr, string name)
            {
                bool result = false;
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (dr.GetName(i).Equals(name, StringComparison.CurrentCultureIgnoreCase) && !dr[i].Equals(DBNull.Value))
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }

            /// <summary>
            /// 判断datarow所在的datatable是否存在某列并且值不为空
            /// </summary>
            /// <param name="dr">当前datarow</param>
            /// <param name="name">字段名</param>
            /// <returns></returns>
            public static bool CanSetted(DataRow dr, string name)
            {
                return dr.Table.Columns.Contains(name) && !dr.IsNull(name);
            }
        }
    }
}
