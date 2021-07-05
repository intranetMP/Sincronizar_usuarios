using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ComprobarRegistros();
        }

        public void ComprobarRegistros()
        {
            string strCon = "Provider=VFPOLEDB.1;Data Source=\\\\10.0.5.1\\sim\\Fox\\nomina\\";
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = strCon;
            con.Open();
            string consulta = "Select NUMERO, NOMBRE, PATERNO, MATERNO, ACTIVO from nomina WHERE CVEPLANTA !='B'";


            //System.Diagnostics.Debug.WriteLine(consulta);

            OleDbDataAdapter adapter = new OleDbDataAdapter(consulta, con);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "Empleados");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int numeroSIM = int.Parse(ds.Tables[0].Rows[i]["NUMERO"].ToString().Trim());
                string nombreSIM = ds.Tables[0].Rows[i]["NOMBRE"].ToString().Trim();
                string paternoSIM = ds.Tables[0].Rows[i]["PATERNO"].ToString().Trim();
                string maternoSIM = ds.Tables[0].Rows[i]["MATERNO"].ToString().Trim();
                string ActivoSim = ds.Tables[0].Rows[i]["ACTIVO"].ToString().Trim();

                ExisteComoUsuario(numeroSIM, strCon);
            }
        }

        public bool ExisteComoUsuario(int nomina, string strCon)
        {
            bool existe = false;
            OleDbConnection con2 = new OleDbConnection();
            con2.ConnectionString = strCon;
            con2.Open();
            string consulta = "Select NUMERO, CLAVE from usuarios WHERE NUMERO=" + nomina;

            OleDbDataAdapter adapter = new OleDbDataAdapter(consulta, con2);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "Usuarios");
            string connString = "Server=10.0.5.229;Database=DESARROLLO;User Id=desarrollo;Password=Desa2019*;";
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();

            if (ds.Tables[0].Rows.Count > 0) //compueba si el registro existe en la tabla de usuarios
            {
                int numeroSIM = int.Parse(ds.Tables[0].Rows[0]["NUMERO"].ToString().Trim());
                string claveSIM = ds.Tables[0].Rows[0]["CLAVE"].ToString().Trim();
                SqlCommand command = new SqlCommand("select idNomina, Password, Activo, COUNT(idNomina) conteo from catUsuariosMerca WHERE idNomina=" + numeroSIM + " GROUP BY idNomina, Password, Activo", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        if (int.Parse(reader["conteo"].ToString()) > 0)
                        {
                            string contrasena = reader["conteo"].ToString();
                            if (claveSIM != contrasena)
                            {
                                reader.Close();
                                SqlCommand command2 = new SqlCommand("UPDATE catUsuariosMerca SET Password=\'" + claveSIM + "\' WHERE idNomina=" + numeroSIM, conn);
                                command2.ExecuteReader();
                                command2.Dispose();

                            }
                        }
                        //System.Diagnostics.Debug.WriteLine("idNomina: " + reader["conteo"]);
                    }
                    else
                    {
                        //INSERTAR REGISTRO SI NO EXISTE EN LA BD DE SQL SERVER
                        reader.Close();
                        string SQL = "INSERT INTO catUsuariosMerca VALUES(" + nomina + ",\'" + claveSIM + "\',1)";
                        SqlCommand command31 = new SqlCommand(SQL, conn);
                        command31.ExecuteReader();
                        command31.Dispose();
                    }
                }
            }
            else
            {
                try
                {
                    SqlCommand command3 = new SqlCommand("INSERT INTO catUsuariosMerca VALUES(" + nomina + "," + nomina + ",1)", conn);
                    command3.ExecuteReader();
                    command3.Dispose();
                }
                catch
                {

                }

            }

            conn.Close();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int numeroUsuarioSim = int.Parse(ds.Tables[0].Rows[i]["NUMERO"].ToString().Trim());
                string claveUsuarioSim = ds.Tables[0].Rows[i]["CLAVE"].ToString().Trim();

            }
            return existe;

        }

        public void conexionSql()
        {
            string connString = "Server=10.0.5.229;Database=DESARROLLO;User Id=desarrollo;Password=Desa2019*;";
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();

            SqlCommand command = new SqlCommand("Select * from catUsuariosMerca", conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Debug.WriteLine("idNomina: " + reader["idNomina"]);
                }
            }

            conn.Close();
        }

        protected void cambiarContrasena(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string pass = txtPass.Text;
            Debug.WriteLine("Usuario: " + usuario);
            Debug.WriteLine("Contraseña: " + pass);
            //OleDbConnection dbConn = new OleDbConnection(@"Provider=VFPOLEDB.1;Data Source=\\\\10.0.5.1\\sim\\Fox\\nomina\\");
            OleDbConnection dbConn = new OleDbConnection(@"Provider=VFPOLEDB.1;Data Source=C:\\tablasDBF\\");

            OleDbCommand command = dbConn.CreateCommand();
            dbConn.Open();

            //AGREGAR QUERY PARA EL UPDATE Y AGREGAR PARAMETROS
            string sql = "UPDATE usuarios set CLAVE=\"" + pass + "\" WHERE NUMERO=" + usuario;
            command.CommandText = sql;

            int effected = command.ExecuteNonQuery();
            dbConn.Close();
        }
    }
}