using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSendMail.App_Code
{
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

                if (FileData.Length > 0)
                {
                    oSQLParam = new SqlParameter("@img_data", SqlDbType.Image);
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

        /// <summary>
        /// Ejecuta un procedimiento almacenado con un único parámetro @param (NText) y devuelve un SqlDataReader.
        /// El DataReader debe ser cerrado por el consumidor (Dispose/Close) para liberar la conexión.
        /// </summary>
        /// <param name="sStoreProcName">Nombre del procedimiento almacenado.</param>
        /// <param name="pstrXMLPeticion">Valor para @param (puede ser null).</param>
        /// <returns>SqlDataReader con CommandBehavior.CloseConnection (cerrará la conexión al cerrar el reader).</returns>
        public static SqlDataReader ExecuteReaderOneParameter(string sStoreProcName, string pstrXMLPeticion)
        {
            var con = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand(sStoreProcName, con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            cmd.Parameters.Add(new SqlParameter("@param", SqlDbType.NText)
            {
                Value = (object)pstrXMLPeticion ?? DBNull.Value
            });

            con.Open();
            try
            {
                // CommandBehavior.CloseConnection asegura que al cerrar el reader se cierre la conexión.
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                con.Close();
                throw;
            }
        }

        /// <summary>
        /// Variante que permite especificar explícitamente el connectionString.
        /// </summary>
        public static SqlDataReader ExecuteReaderOneParameter(string sStoreProcName, string pstrXMLPeticion, string connectionString)
        {
            var con = new SqlConnection(connectionString);
            var cmd = new SqlCommand(sStoreProcName, con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            cmd.Parameters.Add(new SqlParameter("@param", SqlDbType.NText)
            {
                Value = (object)pstrXMLPeticion ?? DBNull.Value
            });

            con.Open();
            try
            {
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                con.Close();
                throw;
            }
        }
    }
}
