using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Net;

namespace checador
{
    /// <summary>
    /// Summary description for ReaderAndWriter
    /// </summary>
    public class ReaderAndWriter
    {
        public ReaderAndWriter()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private static int sConnectionStringk;
        public static int ConnectionStringk
        {
            set
            {
                sConnectionStringk = value;
            }
            get
            {
                return sConnectionStringk;
            }
        }

        private static string sConnectionString;
        public static string ConnectionString
        {
            set
            {
                sConnectionString = value;
            }
            get
            {
                return sConnectionString;
            }
        }

        public static DataSet OneParameterStoreProcCaller(string sStoreProcName, string pstrXMLPeticion)
        {


            DataSet dtsDataSet = new DataSet();
            string strValor;

            System.Data.SqlClient.SqlCommand oSQLCommand = new SqlCommand();
            System.Data.SqlClient.SqlParameter oSQLParam = new SqlParameter();
            System.Data.SqlClient.SqlDataAdapter oSQLDataAdapter = new SqlDataAdapter();

            try
            {
                //Se abre la conexión
                SqlConnection con = new SqlConnection(ConnectionString); // old
                //SqlConnection con = Connection.GetConnection(ConnectionStringk);

                oSQLCommand.Connection = con;
                oSQLCommand.CommandTimeout = con.ConnectionTimeout;
                oSQLCommand.Parameters.Clear();
                oSQLCommand.CommandType = CommandType.StoredProcedure;
                oSQLCommand.CommandText = sStoreProcName;

                strValor = pstrXMLPeticion;
                oSQLParam = new SqlParameter("@param", SqlDbType.NText);
                oSQLParam.Value = strValor;
                oSQLCommand.Parameters.Add(oSQLParam);
                try
                {
                    oSQLDataAdapter = new SqlDataAdapter();
                    oSQLDataAdapter.SelectCommand = oSQLCommand;
                    oSQLDataAdapter.Fill(dtsDataSet, "Catalogo");
                }
                catch (Exception objException)
                {
                    throw objException;
                }
                finally
                {
                    oSQLCommand.Connection.Dispose();
                    oSQLCommand.Connection.Close();
                }
            }
            catch (Exception objException)
            {
                throw objException;
            }
            return (dtsDataSet);
        }

        public static DataSet OneParameterStoreProcCaller(string sStoreProcName, string pstrXMLPeticion, string connectionString)
        {
            var dtsDataSet = new DataSet();

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sStoreProcName, con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 60 })
            using (var adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@param", SqlDbType.NText) { Value = (object)pstrXMLPeticion ?? DBNull.Value });

                // Abrir explícitamente para obtener excepciones claras
                con.Open();
                adapter.Fill(dtsDataSet, "Catalogo");
            }

            return dtsDataSet;
        }
        public static DataSet TwoParametersXMLImage(string sStoreProcName, string pstrXMLPeticion, ref Byte[] FileData)
        {
            DataSet dtsDataSet = new DataSet();

            System.Data.SqlClient.SqlCommand oSQLCommand = new SqlCommand();
            System.Data.SqlClient.SqlParameter oSQLParam = new SqlParameter();
            System.Data.SqlClient.SqlDataAdapter oSQLDataAdapter = new SqlDataAdapter();

            try
            {
                //Se abre la conexión
                SqlConnection con = new SqlConnection(ConnectionString);

                oSQLCommand.Connection = con;
                oSQLCommand.CommandTimeout = con.ConnectionTimeout;
                oSQLCommand.Parameters.Clear();
                oSQLCommand.CommandType = CommandType.StoredProcedure;
                oSQLCommand.CommandText = sStoreProcName;

                oSQLParam = new SqlParameter("@param", SqlDbType.NText);
                oSQLParam.Value = pstrXMLPeticion;
                oSQLCommand.Parameters.Add(oSQLParam);

                if (FileData != null && FileData.Length > 0)
                {
                    string paramName = string.Equals(sStoreProcName, Constants.sp_catalogo_trabajadores, StringComparison.OrdinalIgnoreCase)
                        ? "@Foto2"
                        : "@img_data";

                    oSQLParam = new SqlParameter(paramName, SqlDbType.Image);
                    oSQLParam.Value = FileData;
                    oSQLCommand.Parameters.Add(oSQLParam);
                }

                try
                {
                    oSQLDataAdapter = new SqlDataAdapter();
                    oSQLDataAdapter.SelectCommand = oSQLCommand;
                    oSQLDataAdapter.Fill(dtsDataSet, "Catalogo");
                }
                catch (Exception objException)
                {
                    throw objException;
                }

                oSQLCommand.Connection.Close();
            }
            catch (Exception objException)
            {
                throw objException;
            }
            return (dtsDataSet);
        }
    }
}