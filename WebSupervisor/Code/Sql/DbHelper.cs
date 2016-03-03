using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Reflection;
using WebSupervisor.Models;
namespace WebDAL
{
    public class DBHelper
    {
        #region 属性
        //项目中添加如下引用
        //System.Configuration
        //System.Data.OracleClient
        //请在您的web.config配置文件中增加如下节点：
        //<appSettings>
        //<add key="DBType" value="SQLServer"/>配置数据库类型SQLServer||OleDb||ODBC||Oracle
        //<add key="SQLServer" value="连库字符串"/>配置该数据库类型对应的连库字符串
        //<add key="assemblyName" value="Entity"/>指定实体类的命名空间
        //</appSettings>
        //从Web.config文件中动态获取连接字符串和数据库类型
        private static string conType = ConfigurationManager.AppSettings["DBType"].ToString();
        private static string constr = ConfigurationManager.AppSettings[conType].ToString();

        #endregion
        #region 私有方法
        /// <summary>
        /// 根据数据库类型，获取对应数据库的连接
        /// </summary>
        /// <returns>连接接口</returns>
        private static IDbConnection GetConnection()
        {
            IDbConnection con = null;
            if (conType == DBType.SQLServer.ToString())
            {
                con = new SqlConnection(constr);
            }
            else if (conType == DBType.OleDb.ToString())
            {
                con = new OleDbConnection(constr);
            }
            else if (conType == DBType.ODBC.ToString())
            {
                con = new OdbcConnection(constr);
            }
            else
            {
                con = new SqlConnection(constr);
            }
            return con;
        }
        /// <summary>
        /// 根据数据库类型，获取对应Command对象
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdType">SQL命令类型</param>
        /// <param name="con">连接对象</param>
        /// <param name="param">SQL命令参数数组</param>
        /// <returns>Command接口对象</returns>
        private static IDbCommand GetCommand(string commandText, CommandType commandType, IDbConnection con, params IDbDataParameter[] param)
        {
            IDbCommand cmd = null;
            if (conType == DBType.SQLServer.ToString())
            {
                cmd = new SqlCommand(commandText, con as SqlConnection);
            }
            else if (conType == DBType.OleDb.ToString())
            {
                cmd = new OleDbCommand(commandText, con as OleDbConnection);
            }
            else if (conType == DBType.ODBC.ToString())
            {
                cmd = new OdbcCommand(commandText, con as OdbcConnection);
            }
            else
            {
                cmd = new SqlCommand(commandText, con as SqlConnection);
            }
            cmd.CommandType = commandType;

            //for(int i=0;i<param.Length;i++)
            //{
            if (param != null)
            {
                foreach (IDbDataParameter Idbparam in param)
                {
                    cmd.Parameters.Add(Idbparam);
                }

            }
            //}
            return cmd;
        }
        //private static IDbCommand GetCommand1(string commandText, CommandType commandType, IDbConnection con, params IDbDataParameter[] param)
        //{
        //    IDbCommand cmd = null;
        //    if (conType == DBType.SQLServer.ToString())
        //    {
        //        cmd = new SqlCommand(commandText, con as SqlConnection);
        //    }
        //    else if (conType == DBType.OleDb.ToString())
        //    {
        //        cmd = new OleDbCommand(commandText, con as OleDbConnection);
        //    }
        //    else if (conType == DBType.ODBC.ToString())
        //    {
        //        cmd = new OdbcCommand(commandText, con as OdbcConnection);
        //    }
        //    else
        //    {
        //        cmd = new SqlCommand(commandText, con as SqlConnection);
        //    }
        //    cmd.CommandType = commandType;
        //    for (int i = 0; i < param.Length; i++)
        //    {
        //        if (param[i] != null)
        //        {
        //            cmd.Parameters.Add(param[i]);
        //        }
        //    }
        //    return cmd;
        //}
        /// <summary>
        /// 执行返回一条记录的泛型集合对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="reader">Reader对象，用于读取数据结果集</param>
        /// <returns>泛型对象</returns>
        private static T ExexuteDataReader<T>(IDataReader reader)
        {
            T obj = default(T);
            try
            {
                Type type = typeof(T);
                obj = (T)Activator.CreateInstance(type);//使用默认构造器初始化对象

                PropertyInfo[] propertyInfos = type.GetProperties();
                foreach (PropertyInfo propertyinfo in propertyInfos)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string filedName = reader.GetName(i);
                        if (filedName.ToLower() == propertyinfo.Name.ToLower())
                        {
                            Object value = reader[propertyinfo.Name];
                            if (value != null && value != DBNull.Value)
                            {
                                propertyinfo.SetValue(obj, value, null);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }
        #endregion
        #region 公有方法
        /// <summary>
        /// 执行返回一行一列的数据库操作
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdType">SQL命令类型</param>
        /// <param name="param">SQL命令参数数组</param>
        /// <returns>第一行第一列记录</returns>
        public static int ExecuteScalar(string commandText, CommandType commandType, params IDbDataParameter[] param)
        {
            int result = 0;
            try
            {
                IDbConnection con = GetConnection();
                IDbCommand cmd = GetCommand(commandText, commandType, con, param);
                using (con)
                {
                    using (cmd)
                    {
                        con.Open();
                        result = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 执行非查询的数据库操作
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="cmdType">SQL命令类型</param>
        /// <param name="param">SQL命令参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] param)
        {
            int result = 0;
            try
            {
                IDbConnection con = GetConnection();
                IDbCommand cmd = GetCommand(commandText, commandType, con, param);
                using (con)
                {
                    using (cmd)
                    {
                        con.Open();
                        IDbTransaction tr = con.BeginTransaction();
                        cmd.Transaction = tr;
                        try
                        {
                            result = Convert.ToInt32(cmd.ExecuteNonQuery());
                            tr.Commit();
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 执行返回一条记录的泛型对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="commandText">SQL语句或存储过程名</param>
        /// <param name="commandType">SQL命令类型</param>
        /// <param name="param">SQL命令参数数组</param>
        /// <returns>实体对象</returns>
        public static T ExexuteEntity<T>(string commandText, CommandType commandType, params IDbDataParameter[] param)
        {
            T obj = default(T);
            try
            {
                IDbConnection con = GetConnection();
                IDbCommand cmd = GetCommand(commandText, commandType, con, param);
                using (con)
                {
                    using (cmd)
                    {
                        con.Open();
                        IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (reader.Read())
                        {
                            obj = DBHelper.ExexuteDataReader<T>(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }

        /// <summary>
        /// 批量插入对象数组(仅限sqlserver)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">对象数组集合</param>
        public static void BulkInsert<T>(List<T> list)
        {
            T obj = default(T);
            try
            {

                Type type = typeof(T);
                obj = (T)Activator.CreateInstance(type);//使用默认构造器初始化对象
                string tablename = type.Name.ToLower().ToString().Substring(0, type.Name.ToLower().IndexOf("model"));
                PropertyInfo[] propertyInfos = type.GetProperties();
                string commandText = "select * from " + tablename + "";
                DataSet dataSet = new DataSet();
                SqlConnection con = (SqlConnection)GetConnection();
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, con);
                adapter.Fill(dataSet);
                if (dataSet.Tables[0] != null && dataSet.Tables.Count != 0)
                {
                    DataTable dt = dataSet.Tables[0];
                    foreach (T model in list)
                    {

                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < propertyInfos.Length; i++)
                        {

                            dr[i] = propertyInfos[i].GetValue(model, null);

                        }
                        dt.Rows.Add(dr);
                    }
                    using (con)
                    {

                        con.Open();
                        using (SqlBulkCopy bulk = new SqlBulkCopy(con))
                        {
                            bulk.BatchSize = dt.Rows.Count;
                            bulk.DestinationTableName = tablename;
                            foreach (DataColumn dcl in dt.Columns)
                            {
                                bulk.ColumnMappings.Add(dcl.ColumnName, dcl.ColumnName);
                            }

                            bulk.WriteToServer(dt);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行返回多行记录的泛型集合对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="commandText">SQL语句或村存储过程名</param>
        /// <param name="commandType">SQL命令类型</param>
        /// <param name="param">SQL命令参数数组</param>
        /// <returns>泛型集合对象</returns>
        public static List<T> ExecuteList<T>(string commandText, CommandType commandType, params IDbDataParameter[] param)
        {
            List<T> list = new List<T>();
            try
            {
                IDbConnection con = GetConnection();
                IDbCommand cmd = GetCommand(commandText, commandType, con, param);
                using (con)
                {
                    using (cmd)
                    {
                        con.Open();
                        IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (reader.Read())
                        {
                            T obj = DBHelper.ExexuteDataReader<T>(reader);
                            list.Add(obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        /// <summary>
        /// 更新一个指定的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">对象实例</param>
        public static void Update<T>(T model)
        {
            T obj = default(T);

            try
            {

                Type type = typeof(T);
                obj = (T)Activator.CreateInstance(type);//使用默认构造器初始化对象
                string tablename = type.Name.ToLower().ToString().Substring(0, type.Name.ToLower().IndexOf("model"));
                PropertyInfo[] propertyInfos = type.GetProperties();

                string commandText = "update " + tablename + " set {0}";
                string param = "";

                SqlParameter[] arrayparams = new SqlParameter[propertyInfos.Length];
                for (int i = 0; i < propertyInfos.Length; i++)
                {

                    arrayparams[i] = new SqlParameter("@" + propertyInfos[i].Name.ToLower().ToString(), propertyInfos[i].GetValue(model, null));



                    param = param + propertyInfos[i].Name.ToLower().ToString() + "=" + "@" + propertyInfos[i].Name.ToLower().ToString() + ",";
                }
                param = param.Substring(0, param.Length - 1);
                commandText = string.Format(commandText, param);
                if (propertyInfos[0].PropertyType.IsPrimitive)
                {
                    commandText = string.Format(commandText + " where {0}={1}", propertyInfos[0].Name.ToLower().ToString(), propertyInfos[0].GetValue(model, null));

                }
                else
                {
                    commandText = string.Format(commandText + " where {0}='{1}'", propertyInfos[0].Name.ToLower().ToString(), propertyInfos[0].GetValue(model, null));
                }

                ExecuteNonQuery(commandText, CommandType.Text, arrayparams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        /// <summary>
        /// 用于指定对象的全属性插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">实例化的对象</param>
        public static void Insert<T>(T model)
        {
            T obj = default(T);

            try
            {

                Type type = typeof(T);
                obj = (T)Activator.CreateInstance(type);//使用默认构造器初始化对象
                string tablename = type.Name.ToLower().ToString().Substring(0, type.Name.ToLower().IndexOf("model"));
                PropertyInfo[] propertyInfos = type.GetProperties();

                string commandText = "insert into " + tablename + " values({0})";
                string param = "";

                SqlParameter[] arrayparams = new SqlParameter[propertyInfos.Length];
                for (int i = 0; i < propertyInfos.Length; i++)
                {

                    arrayparams[i] = new SqlParameter("@" + propertyInfos[i].Name.ToLower().ToString(), propertyInfos[i].GetValue(model, null));



                    param = param + "@" + propertyInfos[i].Name.ToLower().ToString() + ",";
                }
                param = param.Substring(0, param.Length - 1);
                commandText = string.Format(commandText, param);
                ExecuteNonQuery(commandText, CommandType.Text, arrayparams);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        /// <summary>
        /// 查询指定行数记录
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="n">开始的记录位置</param>
        /// <param name="m">记录条数</param>
        /// <param name="tablename">数据表名</param>
        /// <returns></returns>
        public static List<T> DataResult<T>(int n, int m, string tablename)
        {
            string end = (n + m).ToString();
            //T obj = default(T);
            string selectsql = string.Format("select top {0} * from {1} where id not in (select top {2} * from {1})", end, tablename, n.ToString());
            List<T> lst = new List<T>();
            lst = ExecuteList<T>(selectsql, CommandType.Text, null);
            return lst;
        }
        /// <summary>
        /// 返回指定的数据表的页数
        /// </summary>
        /// <typeparam name="T">表格对应的model</typeparam>
        /// <param name="list">存储查询后的结果，便于以后的取数据</param>
        /// <param name="pagesize">页面大小（一页多少条记录）</param>
        /// <returns></returns>
        public int TotalPage<T>(List<T> list,int pagesize)//返回总的页数
        {
            Type type = typeof(T);
            string tablename = type.Name.ToLower().ToString().Substring(0, type.Name.ToLower().IndexOf("model"));
            string commandtext = "select * ftom " + tablename;
            list= ExecuteList<T>(commandtext, CommandType.Text, null);
            int Linenumber = list.Count;
            if ((Linenumber % pagesize) > 0)
            {
                int allpage = (Linenumber / pagesize) + 1;
                return allpage;
            }
            else
            {
                int allpage = Linenumber / pagesize;
                return allpage;
            }

        }
        /// <summary>
        /// 获取指定页的数据
        /// </summary>
        /// <typeparam name="T">数据表</typeparam>
        /// <param name="list">数据源</param>
        /// <param name="currentpage">当前页</param>
        /// <param name="pagesize">页面尺寸</param>
        /// <returns></returns>
        private List<T> GetCurrentData<T>(List<T> list, int currentpage, int pagesize)//返回指定页数的记录
        {
            if (currentpage == 0)
                return list;
            List<T> NewList = new List<T>();
            foreach (T t in list)
            {
                NewList.Add(t);
            }
            int rowbegin = (currentpage - 1) * pagesize;
            int rowend = currentpage * pagesize;

            if (rowbegin >= list.Count)
            {
                return NewList;
            }
               

            if (rowend > list.Count)
            {
                rowend = list.Count;
                NewList.Clear();
            }
              
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
              
                NewList.Add(list[i]);
            }

            return NewList;
        }
    }

    #region 数据库类型枚举
    /// <summary>
    /// 该枚举类型用于创建合适的数据库访问对象
    /// </summary>
    public enum DBType
    {
        SQLServer,
        OleDb,
        ODBC,
        Oracle
    }
    #endregion
}