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
            if (param != null)
            {
               
                cmd.Parameters.Add(param);
            }
            return cmd;
        }
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
        #endregion
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